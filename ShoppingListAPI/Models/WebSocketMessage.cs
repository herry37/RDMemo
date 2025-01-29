using System.Text.Json.Serialization;

namespace ShoppingListAPI.Models;

/// <summary>
/// WebSocket 訊息模型
/// 用於在客戶端和伺服器之間傳遞訊息
/// </summary>
public class WebSocketMessage
{
    /// <summary>
    /// 訊息類型
    /// 可能的值：
    /// - welcome：歡迎訊息
    /// - ping：心跳檢測請求
    /// - pong：心跳檢測回應
    /// - subscribe：訂閱購物清單
    /// - unsubscribe：取消訂閱購物清單
    /// - list_update：購物清單更新通知
    /// - error：錯誤訊息
    /// </summary>
    [JsonPropertyName("type")]
    public string Type { get; set; }

    /// <summary>
    /// 訊息內容
    /// 根據訊息類型可能包含不同的資料結構
    /// </summary>
    [JsonPropertyName("data")]
    public object Data { get; set; }

    /// <summary>
    /// 訊息時間戳記
    /// </summary>
    [JsonPropertyName("timestamp")]
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// 建立錯誤訊息
    /// </summary>
    /// <param name="errorMessage">錯誤訊息內容</param>
    /// <returns>包含錯誤資訊的 WebSocket 訊息</returns>
    public static WebSocketMessage CreateError(string errorMessage)
    {
        return new WebSocketMessage
        {
            Type = "error",
            Data = new { message = errorMessage }
        };
    }

    /// <summary>
    /// 建立更新通知訊息
    /// </summary>
    /// <param name="listId">購物清單 ID</param>
    /// <param name="data">更新的資料</param>
    /// <returns>包含更新資訊的 WebSocket 訊息</returns>
    public static WebSocketMessage CreateUpdateNotification(string listId, object data)
    {
        return new WebSocketMessage
        {
            Type = "list_update",
            Data = new { listId, data }
        };
    }

    /// <summary>
    /// 建立心跳回應訊息
    /// </summary>
    /// <returns>包含時間戳記的心跳回應訊息</returns>
    public static WebSocketMessage CreatePongResponse()
    {
        return new WebSocketMessage
        {
            Type = "pong",
            Data = new { timestamp = DateTime.UtcNow }
        };
    }
}
