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
        try
        {
            using var webSocket = await context.WebSockets.AcceptWebSocketAsync();
            var socketId = Guid.NewGuid().ToString();
            _logger.LogInformation($"WebSocket connected: {socketId}");

            try
            {
                await HandleMessages(webSocket);
            }
            catch (WebSocketException ex)
            {
                _logger.LogError(ex, "WebSocket error");
            }
            finally
            {
                if (webSocket.State == WebSocketState.Open)
                {
                    await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling WebSocket connection");
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        }
    }

    private async Task HandleMessages(System.Net.WebSockets.WebSocket webSocket)
    {
        var buffer = new byte[1024 * 4];
        while (webSocket.State == WebSocketState.Open)
        {
            var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            if (result.MessageType == WebSocketMessageType.Close)
            {
                await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Client requested close", CancellationToken.None);
            }
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
}
