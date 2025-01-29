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
    /// 項目描述
    /// </summary>
    [StringLength(500, ErrorMessage = "項目描述長度不能超過 500 個字元")]
    public string Description { get; set; }

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
    /// 項目完成時間
    /// </summary>
    public DateTime? CompletedAt { get; set; }

    /// <summary>
    /// 項目建立時間
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// 項目最後更新時間
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// 計算項目總金額
    /// </summary>
    /// <returns>數量乘以單價的總金額</returns>
    public decimal CalculateTotal()
    {
        return Quantity * (Price ?? 0);
    }

    /// <summary>
    /// 建立購物項目的副本
    /// </summary>
    /// <returns>新的購物項目實例，包含相同的資料</returns>
    public ShoppingItem Clone()
    {
        return new ShoppingItem
        {
            Id = Id,
            Name = Name,
            Description = Description,
            Quantity = Quantity,
            Price = Price,
            IsCompleted = IsCompleted,
            CreatedAt = CreatedAt,
            UpdatedAt = UpdatedAt
        };
    }

    /// <summary>
    /// 更新項目資訊
    /// </summary>
    /// <param name="item">包含新資訊的購物項目</param>
    public void Update(ShoppingItem item)
    {
        Name = item.Name;
        Description = item.Description;
        Quantity = item.Quantity;
        Price = item.Price;
        IsCompleted = item.IsCompleted;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// 標記項目為已完成
    /// </summary>
    public void MarkAsCompleted()
    {
        IsCompleted = true;
        CompletedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// 標記項目為未完成
    /// </summary>
    public void MarkAsIncomplete()
    {
        IsCompleted = false;
        CompletedAt = null;
        UpdatedAt = DateTime.UtcNow;
    }
}
