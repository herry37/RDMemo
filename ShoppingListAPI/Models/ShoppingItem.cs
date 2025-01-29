using System.ComponentModel.DataAnnotations;

namespace ShoppingListAPI.Models;

/// <summary>
/// 購物項目模型
/// 用於儲存購物清單中的單個項目資訊
/// </summary>
public class ShoppingItem
{
    /// <summary>
    /// 購物項目唯一識別碼
    /// </summary>
    [Required(ErrorMessage = "購物項目 ID 不能為空")]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    /// <summary>
    /// 項目名稱
    /// </summary>
    [Required(ErrorMessage = "項目名稱不能為空")]
    [StringLength(100, ErrorMessage = "項目名稱長度不能超過 100 個字元")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 購買數量
    /// </summary>
    [Range(1, 9999, ErrorMessage = "數量必須在 1 到 9999 之間")]
    public int Quantity { get; set; } = 1;

    /// <summary>
    /// 單價
    /// </summary>
    [Range(0, 999999.99, ErrorMessage = "價格必須在 0 到 999999.99 之間")]
    public decimal? Price { get; set; }

    /// <summary>
    /// 是否已完成購買
    /// </summary>
    public bool IsCompleted { get; set; }

    /// <summary>
    /// 項目建立時間
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    /// <summary>
    /// 項目最後更新時間
    /// </summary>
    public DateTime? UpdatedAt { get; set; }
}
