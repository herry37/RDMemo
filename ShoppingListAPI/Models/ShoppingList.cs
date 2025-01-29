using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ShoppingListAPI.Models;

/// <summary>
/// 購物清單模型
/// 用於儲存購物清單的相關資訊
/// </summary>
public class ShoppingList
{
    /// <summary>
    /// 購物清單 ID
    /// </summary>
    [Required(ErrorMessage = "購物清單 ID 不能為空")]
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// 標題
    /// </summary>
    [Required(ErrorMessage = "購物清單標題不能為空")]
    [StringLength(100, ErrorMessage = "標題長度不能超過 100 個字元")]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// 購買日期
    /// </summary>
    public DateTime BuyDate { get; set; } = DateTime.Now;

    /// <summary>
    /// 建立時間
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    /// <summary>
    /// 更新時間
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// 總金額
    /// </summary>
    [JsonPropertyName("total")]
    public decimal totalAmount { get; set; }

    /// <summary>
    /// 購物項目列表
    /// </summary>
    public List<ShoppingItem> Items { get; set; } = new List<ShoppingItem>();
}
