using System.ComponentModel.DataAnnotations;

namespace ShoppingListAPI.Models;

public class BatchDeleteModel
{
    [Required]
    [Range(1, 12)]
    public int Month { get; set; }

    [Required]
    [Range(2000, 9999)]
    public int Year { get; set; }
}
