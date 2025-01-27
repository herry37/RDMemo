using System.ComponentModel.DataAnnotations;

namespace ShoppingListAPI.Models;

/// <summary>
/// 購物清單
/// </summary>
public class ShoppingList
{
    public string Id { get; set; } = Guid.NewGuid().ToString();

    /// <summary>
    /// 清單標題
    /// </summary>
    [Required]
    [StringLength(100, MinimumLength = 1)]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// 購買日期
    /// </summary>
    [DataType(DataType.Date)]
    public DateTime? BuyDate { get; set; } // 購買日期

    /// <summary>
    /// 建立時間
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// 購物清單項目
    /// </summary>
    public List<ShoppingItem> Items { get; set; } = new();
}

/// <summary>
/// 購物清單項目
/// </summary>
public class ShoppingItem
{
    public string Id { get; set; } = Guid.NewGuid().ToString();

    /// <summary>
    /// 項目名稱
    /// </summary>
    [Required]
    [StringLength(200, MinimumLength = 1)]
    public string Name { get; set; } = string.Empty;
    /// <summary>
    /// 數量
    /// </summary>
    [Range(1, 1000)]
    public int Quantity { get; set; }
    /// <summary>
    /// 價格
    /// </summary>
    [Range(1, 1000000)]
    public decimal Price { get; set; }
    /// <summary>
    /// 是否已購買
    /// </summary>
    public bool IsCompleted { get; set; }
    /// <summary>
    /// 建立時間
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    /// <summary>
    /// 完成時間
    /// </summary>
    public DateTime? CompletedAt { get; set; }
}