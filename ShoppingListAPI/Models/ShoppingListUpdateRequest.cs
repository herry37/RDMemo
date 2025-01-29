using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ShoppingListAPI.Models;

/// <summary>
/// 購物清單更新請求模型
/// </summary>
public class ShoppingListUpdateRequest
{
    /// <summary>
    /// 購物清單標題
    /// </summary>
    [Required(ErrorMessage = "標題不能為空")]
    [StringLength(100, ErrorMessage = "標題不能超過 100 個字元")]
    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// 購買日期
    /// </summary>
    [Required(ErrorMessage = "購買日期不能為空")]
    [JsonPropertyName("buyDate")]
    public DateTime BuyDate { get; set; }

    /// <summary>
    /// 要更新的購物項目列表
    /// </summary>
    [Required(ErrorMessage = "購物項目列表不能為空")]
    [JsonPropertyName("items")]
    public List<ShoppingItem> Items { get; set; } = new();
}
