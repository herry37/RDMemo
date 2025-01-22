using System.ComponentModel.DataAnnotations;

namespace ShoppingListAPI.Models;

public class ShoppingList
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    
    [Required]
    [StringLength(100, MinimumLength = 1)]
    public string Title { get; set; } = string.Empty;
    
    [DataType(DataType.Date)]
    public DateTime? BuyDate { get; set; } // 購買日期
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public List<ShoppingItem> Items { get; set; } = new();
}

public class ShoppingItem
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    
    [Required]
    [StringLength(200, MinimumLength = 1)]
    public string Name { get; set; } = string.Empty;
    
    [Range(1, 1000)]
    public int Quantity { get; set; }
    
    [Range(0, 1000000)]
    public decimal Price { get; set; }
    
    public bool IsCompleted { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime? CompletedAt { get; set; }
}