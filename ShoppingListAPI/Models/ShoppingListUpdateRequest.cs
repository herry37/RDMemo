using System.ComponentModel.DataAnnotations;

namespace ShoppingListAPI.Models;

public class ShoppingListUpdateRequest
{
    [Required]
    public List<ShoppingItem> Items { get; set; } = new();
}
