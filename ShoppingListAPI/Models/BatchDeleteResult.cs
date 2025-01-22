namespace ShoppingListAPI.Models;

public class BatchDeleteResult
{
    public int DeletedCount { get; set; }
    public string Message { get; set; } = string.Empty;
}
