using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using ShoppingListAPI.Models;

namespace ShoppingListAPI.Services.WebSocket;
/// <summary>
/// WebSocket 處理器，負責管理 WebSocket 連接和消息傳遞
/// </summary>
public class WebSocketHandler
{
    private static readonly List<System.Net.WebSockets.WebSocket> _clients = new();
    private static readonly object _lock = new();
    private readonly ConcurrentDictionary<string, System.Net.WebSockets.WebSocket> _connections = new();
    private readonly ILogger<WebSocketHandler> _logger;
    private readonly JsonSerializerOptions _jsonOptions;

    /// <summary>
    /// 建構函式
    /// </summary>
    public WebSocketHandler(ILogger<WebSocketHandler> logger)
    {
        _logger = logger;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };
    }

    /// <summary>
    /// 處理 WebSocket 連接
    /// </summary>
    public async Task HandleWebSocketConnection(HttpContext context)
    {
        try
        {
            using var webSocket = await context.WebSockets.AcceptWebSocketAsync();
            
            // 添加到客戶端列表
            lock (_lock)
            {
                _clients.Add(webSocket);
                _logger.LogInformation($"新的 WebSocket 連接已建立. 當前連接數: {_clients.Count}");
            }

            var socketId = Guid.NewGuid().ToString();

            _logger.LogInformation("新的 WebSocket 連接已建立. ID: {SocketId}", socketId);

            // 發送歡迎消息
            var welcomeMessage = new WebSocketMessage
            {
                Type = "welcome",
                Message = "已連接到 WebSocket 伺服器"
            };
            await SendMessageAsync(webSocket, welcomeMessage);

            // 添加到連接列表
            _connections.TryAdd(socketId, webSocket);

            try
            {
                await HandleWebSocketCommunication(webSocket, socketId);
            }
            finally
            {
                // 確保連接從字典中移除
                _connections.TryRemove(socketId, out _);
                _logger.LogInformation("WebSocket 連接已關閉. ID: {SocketId}", socketId);

                // 移除客戶端
                lock (_lock)
                {
                    _clients.Remove(webSocket);
                    _logger.LogInformation($"WebSocket 連接已關閉. 當前連接數: {_clients.Count}");
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "處理 WebSocket 連接時發生錯誤");
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        }
    }

    /// <summary>
    /// 處理接收到的消息
    /// </summary>
    private async Task HandleWebSocketCommunication(System.Net.WebSockets.WebSocket webSocket, string socketId)
    {
        var buffer = new byte[1024 * 4];
        var receiveResult = await webSocket.ReceiveAsync(
            new ArraySegment<byte>(buffer), CancellationToken.None);

        while (!receiveResult.CloseStatus.HasValue)
        {
            try
            {
                if (receiveResult.MessageType == WebSocketMessageType.Text)
                {
                    var message = Encoding.UTF8.GetString(buffer, 0, receiveResult.Count);
                    var messageObj = JsonSerializer.Deserialize<WebSocketMessage>(message, _jsonOptions);

                    switch (messageObj?.Type?.ToLower())
                    {
                        case "ping":
                            await SendMessageAsync(webSocket, new WebSocketMessage
                            {
                                Type = "pong",
                                Data = null
                            });
                            break;
                        default:
                            _logger.LogWarning($"收到未知類型的訊息: {messageObj?.Type}");
                            break;
                    }
                }

                receiveResult = await webSocket.ReceiveAsync(
                    new ArraySegment<byte>(buffer), CancellationToken.None);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "處理 WebSocket 訊息時發生錯誤");
                break;
            }
        }

        // 正常關閉連接
        if (webSocket.State == WebSocketState.Open)
        {
            await webSocket.CloseAsync(
                receiveResult.CloseStatus ?? WebSocketCloseStatus.NormalClosure,
                receiveResult.CloseStatusDescription,
                CancellationToken.None);
        }
    }

    /// <summary>
    /// 向指定的 WebSocket 發送消息
    /// </summary>
    private async Task SendMessageAsync(System.Net.WebSockets.WebSocket webSocket, WebSocketMessage message)
    {
        if (webSocket.State != WebSocketState.Open)
        {
            return;
        }

        try
        {
            var json = JsonSerializer.Serialize(message, _jsonOptions);
            var bytes = Encoding.UTF8.GetBytes(json);
            await webSocket.SendAsync(
                new ArraySegment<byte>(bytes),
                WebSocketMessageType.Text,
                true,
                CancellationToken.None);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "發送訊息時發生錯誤");
            throw;
        }
    }

    /// <summary>
    /// 向所有連接的客戶端廣播消息
    /// </summary>
    public async Task BroadcastMessage(WebSocketMessage message)
    {
        List<System.Net.WebSockets.WebSocket> deadSockets = new();

        lock (_lock)
        {
            foreach (var client in _clients)
            {
                try
                {
                    if (client.State != WebSocketState.Open)
                    {
                        deadSockets.Add(client);
                        continue;
                    }

                    _ = SendMessageAsync(client, message);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "廣播訊息時發生錯誤");
                    deadSockets.Add(client);
                }
            }

            // 移除已斷開的連接
            foreach (var deadSocket in deadSockets)
            {
                _clients.Remove(deadSocket);
            }
        }

        if (deadSockets.Any())
        {
            _logger.LogInformation($"已移除 {deadSockets.Count} 個已斷開的連接");
        }
    }

    /// <summary>
    /// 通知購物清單更新
    /// </summary>
    public async Task NotifyShoppingListUpdate(ShoppingList list)
    {
        var message = new WebSocketMessage
        {
            Type = "shoppinglist_update",
            Message = "購物清單已更新",
            Data = list
        };

        await BroadcastMessage(message);
    }
}
