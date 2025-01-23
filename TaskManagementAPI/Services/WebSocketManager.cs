// 引入必要的命名空間
using System.Collections.Concurrent; // 用於線程安全的集合類型
using System.Net.WebSockets; // 用於 WebSocket 通信功能

namespace TodoTaskManagementAPI.Services
{
    /// <summary>
    /// 瀏覽器刷新管理器
    /// 負責管理所有與前端建立的 WebSocket 連接
    /// </summary>
    /// <remarks>
    /// 此類別用於：
    /// 1. 管理多個 WebSocket 連接
    /// 2. 處理連接的生命週期
    /// 3. 實現即時通知功能
    /// 4. 確保線程安全的操作
    /// </remarks>
    public class BrowserRefreshManager
    {
        // 使用線程安全的字典存儲所有活動的 WebSocket 連接
        // Key 為連接的唯一標識符，Value 為 WebSocket 實例
        private readonly ConcurrentDictionary<string, WebSocket> _sockets = new();

        /// <summary>
        /// 添加並管理新的 WebSocket 連接
        /// </summary>
        /// <param name="socket">要添加的 WebSocket 連接</param>
        /// <remarks>
        /// 處理流程：
        /// 1. 為新連接生成唯一標識符
        /// 2. 將連接添加到管理集合中
        /// 3. 持續監聽連接狀態
        /// 4. 處理連接關閉和清理
        /// </remarks>
        public async Task AddSocket(WebSocket socket)
        {
            var id = Guid.NewGuid().ToString(); // 生成唯一的連接標識符
            _sockets.TryAdd(id, socket); // 將新的 WebSocket 連接添加到字典中

            try
            {
                // 持續監聽 WebSocket 連接，直到連接關閉
                while (socket.State == WebSocketState.Open)
                {
                    var buffer = new byte[1024 * 4]; // 創建接收緩衝區，大小為 4KB
                    // 接收客戶端發送的消息
                    var result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

                    // 如果收到關閉消息，退出監聽循環
                    if (result.MessageType == WebSocketMessageType.Close)
                    {
                        break;
                    }
                }
            }
            catch
            {
                // 忽略連接過程中的錯誤
                // 例如：客戶端突然斷開連接等異常情況
            }
            finally
            {
                // 清理資源：從管理集合中移除連接
                _sockets.TryRemove(id, out _);
                
                // 如果連接仍然開啟，主動關閉它
                if (socket.State == WebSocketState.Open)
                {
                    await socket.CloseAsync(
                        WebSocketCloseStatus.NormalClosure, // 正常關閉狀態
                        "Socket closed", // 關閉原因
                        CancellationToken.None // 不使用取消令牌
                    );
                }
            }
        }

        /// <summary>
        /// 向所有活動的 WebSocket 連接發送通知消息
        /// </summary>
        /// <param name="message">要發送的消息內容</param>
        /// <remarks>
        /// 處理流程：
        /// 1. 遍歷所有活動的連接
        /// 2. 檢查連接狀態
        /// 3. 將消息轉換為字節數組
        /// 4. 異步發送消息
        /// </remarks>
        public async Task NotifyAll(string message)
        {
            // 遍歷所有活動的 WebSocket 連接
            foreach (var socket in _sockets.Values)
            {
                // 檢查連接是否處於開啟狀態
                if (socket.State == WebSocketState.Open)
                {
                    // 將消息轉換為 UTF-8 編碼的字節數組
                    var bytes = System.Text.Encoding.UTF8.GetBytes(message);
                    
                    // 異步發送消息到客戶端
                    await socket.SendAsync(
                        new ArraySegment<byte>(bytes), // 消息內容
                        WebSocketMessageType.Text, // 消息類型為文本
                        true, // 表示這是消息的最後一部分
                        CancellationToken.None // 不使用取消令牌
                    );
                }
            }
        }
    }
}
