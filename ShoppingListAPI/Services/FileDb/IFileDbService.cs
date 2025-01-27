using ShoppingListAPI.Models;

namespace ShoppingListAPI.Services.FileDb;

public interface IFileDbService
{
    Task<List<ShoppingList>> GetShoppingLists();
    Task<ShoppingList?> GetShoppingListById(string id);
    Task<bool> SaveShoppingList(ShoppingList list);
    Task<bool> DeleteShoppingList(string id);
    Task<ShoppingList> AddItemToListAsync(string listId, ShoppingItem item);
    Task<ShoppingList> ToggleItemAsync(string listId, string itemId);
    Task<IEnumerable<ShoppingItem>> GetItemsAsync();
    Task SaveChangesAsync(List<ShoppingItem> items, List<string> deletedIds);
}