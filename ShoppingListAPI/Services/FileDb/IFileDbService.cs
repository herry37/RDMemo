using ShoppingListAPI.Models;

namespace ShoppingListAPI.Services.FileDb;

public interface IFileDbService
{
    Task<ShoppingList> GetShoppingListAsync(string id);
    Task<List<ShoppingList>> GetAllShoppingListsAsync(int page = 1, int pageSize = 20);
    Task<ShoppingList> CreateShoppingListAsync(ShoppingList list);
    Task<ShoppingList> SaveShoppingListAsync(ShoppingList list);
    Task<ShoppingList> AddItemToListAsync(string listId, ShoppingItem item);
    Task<ShoppingList> ToggleItemAsync(string listId, string itemId);
    Task DeleteShoppingListAsync(string id);
    Task<List<ShoppingList>> GetAllAsync();
    Task<bool> DeleteAsync(string id);
}