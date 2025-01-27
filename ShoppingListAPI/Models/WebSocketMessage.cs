using System.Text.Json.Serialization;

namespace ShoppingListAPI.Models;

public class WebSocketMessage
{
    [JsonPropertyName("type")]
    public string Type { get; set; }

    [JsonPropertyName("message")]
    public string Message { get; set; }

    [JsonPropertyName("data")]
    public object Data { get; set; }
}
