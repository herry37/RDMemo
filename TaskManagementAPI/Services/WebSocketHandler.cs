using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TodoTaskManagementAPI.Services
{
    /// <summary>
    /// 處理 WebSocket 連接的類別
    /// </summary>
    public class WebSocketHandler
    {
        /// <summary>
        /// 已經連接的 WebSocket 客戶端清單
        /// </summary>
        private static readonly List<WebSocket> _clients = new();

        /// <summary>
        /// 處理 WebSocket 連接
        /// </summary>
        /// <param name="webSocket">WebSocket 連接</param>
        /// <returns>Task</returns>
        public async Task HandleWebSocketConnection(WebSocket webSocket)
        {
            Console.WriteLine("WebSocket 連接建立");

            _clients.Add(webSocket);

            try
            {
                // 設定緩衝區大小
                var buffer = new byte[1024 * 4];

                // 處理 WebSocket 連接
                while (webSocket.State == WebSocketState.Open)
                {
                    var result = await webSocket.ReceiveAsync(
                        new ArraySegment<byte>(buffer), CancellationToken.None);

                    if (result.MessageType == WebSocketMessageType.Close)
                    {
                        Console.WriteLine("WebSocket 連接關閉");
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"WebSocket 連接異常: {ex.Message}");
            }
            finally
            {
                Console.WriteLine("WebSocket 連接關閉");
                await HandleWebSocketClosure(webSocket);
            }
        }

        /// <summary>
        /// 處理 WebSocket 連接關閉
        /// </summary>
        /// <param name="webSocket">WebSocket 連接</param>
        /// <returns>Task</returns>
        private async Task HandleWebSocketClosure(WebSocket webSocket)
        {
            _clients.Remove(webSocket);

            if (webSocket.State == WebSocketState.Open)
            {
                Console.WriteLine("關閉 WebSocket 連接");
                await webSocket.CloseAsync(
                    WebSocketCloseStatus.NormalClosure,
                    "Closing connection",
                    CancellationToken.None);
            }
        }

        /// <summary>
        /// 通知所有客戶端刷新資料
        /// </summary>
        /// <returns>Task</returns>
        public static async Task NotifyClientsToRefresh()
        {
            Console.WriteLine("通知所有客戶端刷新資料");

            var message = Encoding.UTF8.GetBytes("refresh");
            var deadSockets = new List<WebSocket>();

            foreach (var client in _clients)
            {
                if (client.State == WebSocketState.Open)
                {
                    try
                    {
                        Console.WriteLine("發送刷新通知");
                        await client.SendAsync(
                            new ArraySegment<byte>(message),
                            WebSocketMessageType.Text,
                            true,
                            CancellationToken.None);
                    }
                    catch (Exception ex)
                    {
                        Console.Error.WriteLine($"發送刷新通知時發生異常: {ex.Message}");
                        deadSockets.Add(client);
                    }
                }
                else
                {
                    Console.WriteLine("客戶端已經斷開");
                    deadSockets.Add(client);
                }
            }

            // 清理已斷開的連接
            foreach (var deadSocket in deadSockets)
            {
                Console.WriteLine("清理已斷開的連接");
                _clients.Remove(deadSocket);
            }
        }
    }
}
