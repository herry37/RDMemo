using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using ShoppingListAPI.Models;

namespace ShoppingListAPI.Services.WebSocket;

/// <summary>
/// WebSocket 處理器，負責管理 WebSocket 連接和消息傳遞
/// </summary>
public class WebSocketHandler
{
    // 儲存所有活動中的 WebSocket 連線
    private readonly ConcurrentDictionary<string, WebSocketConnection> _sockets = new();
    
    // 日誌服務
    private readonly ILogger<WebSocketHandler> _logger;
    
    // 訊息緩衝區大小
    private const int BufferSize = 4 * 1024; // 4KB

    /// <summary>
    /// 建構函式
    /// </summary>
    /// <param name="logger">日誌服務</param>
    public WebSocketHandler(ILogger<WebSocketHandler> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// 處理 WebSocket 連線請求
    /// </summary>
    /// <param name="context">HTTP 上下文</param>
    /// <returns>處理任務</returns>
    public async Task HandleWebSocketConnection(HttpContext context)
    {
        try
        {
            using var webSocket = await context.WebSockets.AcceptWebSocketAsync();
            var connectionId = Guid.NewGuid().ToString();
            
            var connection = new WebSocketConnection
            {
                Socket = webSocket,
                ConnectionId = connectionId,
                ConnectedAt = DateTime.UtcNow
            };

            if (_sockets.TryAdd(connectionId, connection))
            {
                _logger.LogInformation($"新的 WebSocket 連接已建立: {connectionId}");
                
                // 發送歡迎訊息
                await SendMessageAsync(connectionId, new WebSocketMessage
                {
                    Type = "welcome",
                    Data = new { message = "連接成功" }
                });

                try
                {
                    await HandleWebSocketCommunicationAsync(connection);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"處理 WebSocket 通訊時發生錯誤: {connectionId}");
                }
                finally
                {
                    // 移除連接
                    if (_sockets.TryRemove(connectionId, out _))
                    {
                        _logger.LogInformation($"WebSocket 連接已關閉: {connectionId}");
                    }
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "處理 WebSocket 連接時發生錯誤");
        }
    }

    /// <summary>
    /// 處理新的 WebSocket 連線
    /// </summary>
    /// <param name="socket">WebSocket 連線</param>
    /// <param name="connectionId">連線 ID</param>
    public async Task HandleConnectionAsync(System.Net.WebSockets.WebSocket socket, string connectionId)
    {
        try
        {
            // 建立新的連線物件
            var connection = new WebSocketConnection
            {
                Socket = socket,
                ConnectionId = connectionId,
                ConnectedAt = DateTime.UtcNow
            };

            // 將連線加入集合中
            if (_sockets.TryAdd(connectionId, connection))
            {
                _logger.LogInformation("新的 WebSocket 連線已建立: {ConnectionId}", connectionId);

                // 發送歡迎訊息
                await SendMessageAsync(connectionId, new WebSocketMessage
                {
                    Type = "welcome",
                    Data = new { message = "已連接到 WebSocket 伺服器" }
                });

                // 開始接收訊息
                await ReceiveMessagesAsync(connection);
            }
            else
            {
                _logger.LogWarning("無法建立 WebSocket 連線: {ConnectionId}", connectionId);
                await CloseConnectionAsync(connection, WebSocketCloseStatus.InternalServerError, "無法建立連線");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "處理 WebSocket 連線時發生錯誤: {ConnectionId}", connectionId);
            if (socket.State == WebSocketState.Open)
            {
                await socket.CloseAsync(WebSocketCloseStatus.InternalServerError, "內部伺服器錯誤", CancellationToken.None);
            }
        }
    }

    /// <summary>
    /// 接收 WebSocket 訊息
    /// </summary>
    /// <param name="connection">WebSocket 連線</param>
    private async Task ReceiveMessagesAsync(WebSocketConnection connection)
    {
        var buffer = new byte[BufferSize];
        var messageBuffer = new List<byte>();

        try
        {
            while (connection.Socket.State == WebSocketState.Open)
            {
                var result = await connection.Socket.ReceiveAsync(
                    new ArraySegment<byte>(buffer),
                    CancellationToken.None
                );

                // 處理接收到的訊息
                messageBuffer.AddRange(new ArraySegment<byte>(buffer, 0, result.Count));

                if (result.EndOfMessage)
                {
                    var message = Encoding.UTF8.GetString(messageBuffer.ToArray());
                    messageBuffer.Clear();

                    if (result.MessageType == WebSocketMessageType.Text)
                    {
                        await HandleMessageAsync(connection, message);
                    }
                    else if (result.MessageType == WebSocketMessageType.Close)
                    {
                        await CloseConnectionAsync(connection, WebSocketCloseStatus.NormalClosure, "客戶端要求關閉連線");
                        break;
                    }
                }
            }
        }
        catch (WebSocketException ex)
        {
            _logger.LogWarning(ex, "WebSocket 連線中斷: {ConnectionId}", connection.ConnectionId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "接收 WebSocket 訊息時發生錯誤: {ConnectionId}", connection.ConnectionId);
        }
        finally
        {
            await CloseConnectionAsync(connection, WebSocketCloseStatus.InternalServerError, "連線已關閉");
        }
    }

    /// <summary>
    /// 處理接收到的訊息
    /// </summary>
    /// <param name="connection">WebSocket 連線</param>
    /// <param name="messageJson">訊息內容 (JSON 格式)</param>
    private async Task HandleMessageAsync(WebSocketConnection connection, string messageJson)
    {
        try
        {
            // 解析訊息
            var message = JsonSerializer.Deserialize<WebSocketMessage>(messageJson);
            if (message == null)
            {
                _logger.LogWarning("收到無效的訊息格式: {Message}", messageJson);
                return;
            }

            _logger.LogInformation("收到訊息: {Type} 從 {ConnectionId}", message.Type, connection.ConnectionId);

            // 根據訊息類型處理
            switch (message.Type.ToLower())
            {
                case "ping":
                    await SendMessageAsync(connection.ConnectionId, new WebSocketMessage
                    {
                        Type = "pong",
                        Data = new { timestamp = DateTime.UtcNow }
                    });
                    break;

                case "subscribe":
                    if (message.Data is JsonElement dataElement &&
                        dataElement.TryGetProperty("listId", out var listIdElement) &&
                        listIdElement.ValueKind == JsonValueKind.String)
                    {
                        var listId = listIdElement.GetString();
                        await SubscribeToListAsync(connection, listId);
                    }
                    break;

                case "unsubscribe":
                    if (message.Data is JsonElement unsubDataElement &&
                        unsubDataElement.TryGetProperty("listId", out var unsubListIdElement) &&
                        unsubListIdElement.ValueKind == JsonValueKind.String)
                    {
                        var listId = unsubListIdElement.GetString();
                        await UnsubscribeFromListAsync(connection, listId);
                    }
                    break;

                default:
                    _logger.LogWarning("收到未知的訊息類型: {Type}", message.Type);
                    break;
            }
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "解析訊息時發生錯誤: {Message}", messageJson);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "處理訊息時發生錯誤");
        }
    }

    /// <summary>
    /// 訂閱購物清單更新
    /// </summary>
    /// <param name="connection">WebSocket 連線</param>
    /// <param name="listId">購物清單 ID</param>
    private async Task SubscribeToListAsync(WebSocketConnection connection, string listId)
    {
        if (string.IsNullOrEmpty(listId))
        {
            _logger.LogWarning("嘗試訂閱無效的清單 ID: {ConnectionId}", connection.ConnectionId);
            return;
        }

        // 將清單 ID 加入連線的訂閱列表
        connection.SubscribedLists.Add(listId);
        
        _logger.LogInformation("客戶端 {ConnectionId} 已訂閱清單 {ListId}", connection.ConnectionId, listId);

        // 發送確認訊息
        await SendMessageAsync(connection.ConnectionId, new WebSocketMessage
        {
            Type = "subscribed",
            Data = new { listId }
        });
    }

    /// <summary>
    /// 取消訂閱購物清單更新
    /// </summary>
    /// <param name="connection">WebSocket 連線</param>
    /// <param name="listId">購物清單 ID</param>
    private async Task UnsubscribeFromListAsync(WebSocketConnection connection, string listId)
    {
        if (string.IsNullOrEmpty(listId))
        {
            _logger.LogWarning("嘗試取消訂閱無效的清單 ID: {ConnectionId}", connection.ConnectionId);
            return;
        }

        // 從連線的訂閱列表中移除清單 ID
        connection.SubscribedLists.Remove(listId);
        
        _logger.LogInformation("客戶端 {ConnectionId} 已取消訂閱清單 {ListId}", connection.ConnectionId, listId);

        // 發送確認訊息
        await SendMessageAsync(connection.ConnectionId, new WebSocketMessage
        {
            Type = "unsubscribed",
            Data = new { listId }
        });
    }

    /// <summary>
    /// 發送訊息給指定的連線
    /// </summary>
    /// <param name="connectionId">連線 ID</param>
    /// <param name="message">要發送的訊息</param>
    public async Task SendMessageAsync(string connectionId, WebSocketMessage message)
    {
        if (!_sockets.TryGetValue(connectionId, out var connection))
        {
            _logger.LogWarning("嘗試發送訊息到不存在的連線: {ConnectionId}", connectionId);
            return;
        }

        try
        {
            var json = JsonSerializer.Serialize(message);
            var bytes = Encoding.UTF8.GetBytes(json);

            if (connection.Socket.State == WebSocketState.Open)
            {
                await connection.Socket.SendAsync(
                    new ArraySegment<byte>(bytes),
                    WebSocketMessageType.Text,
                    true,
                    CancellationToken.None
                );
                
                _logger.LogDebug("已發送訊息到 {ConnectionId}: {Type}", connectionId, message.Type);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "發送訊息時發生錯誤: {ConnectionId}", connectionId);
            await CloseConnectionAsync(connection, WebSocketCloseStatus.InternalServerError, "發送訊息失敗");
        }
    }

    /// <summary>
    /// 廣播訊息給所有訂閱特定清單的連線
    /// </summary>
    /// <param name="listId">購物清單 ID</param>
    /// <param name="message">要廣播的訊息</param>
    public async Task BroadcastToListAsync(string listId, WebSocketMessage message)
    {
        var tasks = _sockets.Values
            .Where(conn => conn.SubscribedLists.Contains(listId))
            .Select(conn => SendMessageAsync(conn.ConnectionId, message));

        await Task.WhenAll(tasks);
    }

    /// <summary>
    /// 關閉 WebSocket 連線
    /// </summary>
    /// <param name="connection">要關閉的連線</param>
    /// <param name="status">關閉狀態</param>
    /// <param name="description">關閉原因描述</param>
    private async Task CloseConnectionAsync(WebSocketConnection connection, WebSocketCloseStatus status, string description)
    {
        try
        {
            if (connection.Socket.State == WebSocketState.Open)
            {
                await connection.Socket.CloseAsync(status, description, CancellationToken.None);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "關閉 WebSocket 連線時發生錯誤: {ConnectionId}", connection.ConnectionId);
        }
        finally
        {
            _sockets.TryRemove(connection.ConnectionId, out _);
            _logger.LogInformation("WebSocket 連線已關閉: {ConnectionId}", connection.ConnectionId);
        }
    }

    // 新增：處理 WebSocket 通訊的方法
    private async Task HandleWebSocketCommunicationAsync(WebSocketConnection connection)
    {
        var buffer = new byte[BufferSize];
        WebSocketReceiveResult receiveResult;

        try
        {
            do
            {
                receiveResult = await connection.Socket.ReceiveAsync(
                    new ArraySegment<byte>(buffer), CancellationToken.None);

                if (receiveResult.MessageType == WebSocketMessageType.Text)
                {
                    var messageJson = Encoding.UTF8.GetString(buffer, 0, receiveResult.Count);
                    await HandleMessageAsync(connection, messageJson);
                }

                Array.Clear(buffer, 0, buffer.Length);

            } while (!receiveResult.CloseStatus.HasValue);

            // 正常關閉連接
            await connection.Socket.CloseAsync(
                receiveResult.CloseStatus.Value,
                receiveResult.CloseStatusDescription,
                CancellationToken.None);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"WebSocket 通訊發生錯誤: {connection.ConnectionId}");
            await CloseConnectionAsync(connection, WebSocketCloseStatus.InternalServerError, "發生錯誤");
        }
    }
}

/// <summary>
/// WebSocket 連線資訊
/// </summary>
public class WebSocketConnection
{
    /// <summary>
    /// WebSocket 連線物件
    /// </summary>
    public System.Net.WebSockets.WebSocket Socket { get; set; }

    /// <summary>
    /// 連線 ID
    /// </summary>
    public string ConnectionId { get; set; }

    /// <summary>
    /// 連線建立時間
    /// </summary>
    public DateTime ConnectedAt { get; set; }

    /// <summary>
    /// 已訂閱的購物清單 ID 集合
    /// </summary>
    public HashSet<string> SubscribedLists { get; set; } = new();
}

/// <summary>
/// WebSocket 訊息結構
/// </summary>
public class WebSocketMessage
{
    /// <summary>
    /// 訊息類型
    /// </summary>
    public string Type { get; set; }

    /// <summary>
    /// 訊息資料
    /// </summary>
    public object Data { get; set; }
}
