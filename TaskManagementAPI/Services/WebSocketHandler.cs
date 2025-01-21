using System.Net.WebSockets;
using System.Text;
using System.Collections.Generic;

namespace TodoTaskManagementAPI.Services
{
    public class WebSocketHandler
    {
        private static readonly List<WebSocket> _clients = new();

        public async Task HandleWebSocketConnection(WebSocket webSocket)
        {
            _clients.Add(webSocket);

            try
            {
                var buffer = new byte[1024 * 4];

                while (webSocket.State == WebSocketState.Open)
                {
                    var result = await webSocket.ReceiveAsync(
                        new ArraySegment<byte>(buffer), CancellationToken.None);

                    if (result.MessageType == WebSocketMessageType.Close)
                    {
                        await HandleWebSocketClosure(webSocket);
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"WebSocket error: {ex.Message}");
            }
            finally
            {
                await HandleWebSocketClosure(webSocket);
            }
        }

        private async Task HandleWebSocketClosure(WebSocket webSocket)
        {
            _clients.Remove(webSocket);

            if (webSocket.State == WebSocketState.Open)
            {
                await webSocket.CloseAsync(
                    WebSocketCloseStatus.NormalClosure,
                    "Closing connection",
                    CancellationToken.None);
            }
        }

        public static async Task NotifyClientsToRefresh()
        {
            var message = Encoding.UTF8.GetBytes("refresh");
            var deadSockets = new List<WebSocket>();

            foreach (var client in _clients)
            {
                if (client.State == WebSocketState.Open)
                {
                    try
                    {
                        await client.SendAsync(
                            new ArraySegment<byte>(message),
                            WebSocketMessageType.Text,
                            true,
                            CancellationToken.None);
                    }
                    catch (Exception ex)
                    {
                        Console.Error.WriteLine($"Error sending refresh notification: {ex.Message}");
                        deadSockets.Add(client);
                    }
                }
                else
                {
                    deadSockets.Add(client);
                }
            }

            // 清理已斷開的連接
            foreach (var deadSocket in deadSockets)
            {
                _clients.Remove(deadSocket);
            }
        }
    }
}
