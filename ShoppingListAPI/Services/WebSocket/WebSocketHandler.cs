using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using ShoppingListAPI.Models;

namespace ShoppingListAPI.Services.WebSocket;

public class WebSocketHandler
{
    private readonly ConcurrentDictionary<string, System.Net.WebSockets.WebSocket> _sockets = new();
    private readonly ILogger<WebSocketHandler> _logger;

    public WebSocketHandler(ILogger<WebSocketHandler> logger)
    {
        _logger = logger;
    }

    public async Task HandleWebSocketConnection(HttpContext context)
    {
        System.Net.WebSockets.WebSocket? webSocket = null;
        string? socketId = null;
        
        try 
        {
            webSocket = await context.WebSockets.AcceptWebSocketAsync();
            socketId = Guid.NewGuid().ToString();
            
            _logger.LogInformation($"新的 WebSocket 連接: {socketId}");
            
            if (!_sockets.TryAdd(socketId, webSocket))
            {
                _logger.LogWarning($"無法添加 WebSocket 連接到集合中: {socketId}");
                await webSocket.CloseAsync(
                    WebSocketCloseStatus.InternalServerError,
                    "Unable to register connection",
                    CancellationToken.None);
                return;
            }

            // 發送歡迎消息
            var welcomeMessage = JsonSerializer.Serialize(new
            {
                type = "welcome",
                data = "Connected to WebSocket server"
            });
            await SendMessageAsync(webSocket, welcomeMessage);

            // 處理消息
            await HandleMessages(webSocket);
        }
        catch (WebSocketException ex)
        {
            _logger.LogError(ex, $"WebSocket 錯誤 (ID: {socketId})");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"處理 WebSocket 連接時發生錯誤 (ID: {socketId})");
        }
        finally
        {
            if (socketId != null)
            {
                _sockets.TryRemove(socketId, out _);
            }
            
            if (webSocket != null && webSocket.State == WebSocketState.Open)
            {
                try
                {
                    await webSocket.CloseAsync(
                        WebSocketCloseStatus.NormalClosure,
                        "Closing",
                        CancellationToken.None);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"關閉 WebSocket 時發生錯誤 (ID: {socketId})");
                }
            }
            
            _logger.LogInformation($"WebSocket 連接已結束 (ID: {socketId})");
        }
    }

    private async Task HandleMessages(System.Net.WebSockets.WebSocket webSocket)
    {
        var buffer = new byte[1024 * 4];
        WebSocketReceiveResult? receiveResult = null;

        try
        {
            while (webSocket.State == WebSocketState.Open)
            {
                receiveResult = await webSocket.ReceiveAsync(
                    new ArraySegment<byte>(buffer),
                    CancellationToken.None);

                if (receiveResult.MessageType == WebSocketMessageType.Close)
                {
                    await webSocket.CloseAsync(
                        WebSocketCloseStatus.NormalClosure,
                        "Closing",
                        CancellationToken.None);
                    break;
                }

                // 處理接收到的消息
                if (receiveResult.MessageType == WebSocketMessageType.Text)
                {
                    var message = Encoding.UTF8.GetString(buffer, 0, receiveResult.Count);
                    _logger.LogInformation($"收到消息: {message}");

                    // 發送確認消息
                    var ackMessage = JsonSerializer.Serialize(new 
                    {
                        type = "ack",
                        data = "Message received"
                    });
                    await SendMessageAsync(webSocket, ackMessage);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "處理 WebSocket 消息時發生錯誤");
            if (webSocket.State == WebSocketState.Open)
            {
                await webSocket.CloseAsync(
                    WebSocketCloseStatus.InternalServerError,
                    "Error processing message",
                    CancellationToken.None);
            }
        }
    }

    private async Task SendMessageAsync(System.Net.WebSockets.WebSocket webSocket, string message)
    {
        try
        {
            var buffer = Encoding.UTF8.GetBytes(message);
            await webSocket.SendAsync(
                new ArraySegment<byte>(buffer),
                WebSocketMessageType.Text,
                true,
                CancellationToken.None);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "發送 WebSocket 消息時發生錯誤");
            throw;
        }
    }

    public async Task SendToAllAsync(string message)
    {
        var buffer = Encoding.UTF8.GetBytes(message);
        foreach (var socket in _sockets.Values)
        {
            if (socket.State == WebSocketState.Open)
            {
                try
                {
                    await socket.SendAsync(
                        new ArraySegment<byte>(buffer),
                        WebSocketMessageType.Text,
                        true,
                        CancellationToken.None);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error sending message to WebSocket");
                }
            }
        }
    }

    public async Task BroadcastShoppingListUpdate(ShoppingList list)
    {
        var message = JsonSerializer.Serialize(new { type = "shoppinglist_update", data = list });
        await SendToAllAsync(message);
    }

    public async Task BroadcastShoppingListDelete(string listId)
    {
        var message = JsonSerializer.Serialize(new { type = "shoppinglist_delete", data = listId });
        await SendToAllAsync(message);
    }

    public async Task BroadcastMessage<T>(T message)
    {
        if (message == null) return;

        try
        {
            var json = JsonSerializer.Serialize(message);
            var bytes = Encoding.UTF8.GetBytes(json);
            var arraySegment = new ArraySegment<byte>(bytes);

            foreach (var socket in _sockets.Values)
            {
                if (socket.State == WebSocketState.Open)
                {
                    try
                    {
                        await socket.SendAsync(arraySegment, WebSocketMessageType.Text, true, CancellationToken.None);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error sending message to WebSocket");
                    }
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error broadcasting message");
        }
    }

    public async Task BroadcastAsync(string message)
    {
        var buffer = Encoding.UTF8.GetBytes(message);
        var arraySegment = new ArraySegment<byte>(buffer);
        var deadSockets = new List<string>();

        foreach (var (id, socket) in _sockets)
        {
            if (socket.State == WebSocketState.Open)
            {
                try
                {
                    await socket.SendAsync(arraySegment, WebSocketMessageType.Text, true, CancellationToken.None);
                    _logger.LogInformation($"已向 {id} 發送消息：{message}");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"向 {id} 發送消息時發生錯誤");
                    deadSockets.Add(id);
                }
            }
            else
            {
                deadSockets.Add(id);
            }
        }

        // 移除已斷開的連接
        foreach (var id in deadSockets)
        {
            _sockets.TryRemove(id, out _);
            _logger.LogInformation($"WebSocket 已斷開：{id}，目前連接數：{_sockets.Count}");
        }
    }
}
