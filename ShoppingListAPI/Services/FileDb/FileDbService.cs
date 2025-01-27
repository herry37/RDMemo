using ShoppingListAPI.Models;
using System.Text.Json;

namespace ShoppingListAPI.Services.FileDb;

public class FileDbService : IFileDbService
{
    private readonly string _dataDirectory;
    private readonly ILogger<FileDbService> _logger;

    public FileDbService(IConfiguration configuration, ILogger<FileDbService> logger)
    {
        _dataDirectory = configuration["DataDirectory"] ?? "Data";
        _logger = logger;

        if (!Directory.Exists(_dataDirectory))
        {
            Directory.CreateDirectory(_dataDirectory);
        }
    }

    public async Task<List<ShoppingList>> GetShoppingLists()
    {
        var lists = new List<ShoppingList>();
        var files = Directory.GetFiles(_dataDirectory, "*.json");

        foreach (var file in files)
        {
            try
            {
                var json = await File.ReadAllTextAsync(file);
                var list = JsonSerializer.Deserialize<ShoppingList>(json);
                if (list != null)
                {
                    lists.Add(list);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "讀取購物清單時發生錯誤: {File}", file);
            }
        }

        return lists.OrderByDescending(x => x.CreatedAt).ToList();
    }

    public async Task<ShoppingList?> GetShoppingListById(string id)
    {
        var filePath = Path.Combine(_dataDirectory, $"{id}.json");
        if (!File.Exists(filePath))
        {
            return null;
        }

        try
        {
            var json = await File.ReadAllTextAsync(filePath);
            return JsonSerializer.Deserialize<ShoppingList>(json);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "讀取購物清單時發生錯誤: {Id}", id);
            return null;
        }
    }

    public async Task<bool> SaveShoppingList(ShoppingList list)
    {
        try
        {
            var filePath = Path.Combine(_dataDirectory, $"{list.Id}.json");
            var json = JsonSerializer.Serialize(list);
            await File.WriteAllTextAsync(filePath, json);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "儲存購物清單時發生錯誤: {Id}", list.Id);
            return false;
        }
    }

    public async Task<bool> DeleteShoppingList(string id)
    {
        try
        {
            var filePath = Path.Combine(_dataDirectory, $"{id}.json");
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                return true;
            }
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "刪除購物清單時發生錯誤: {Id}", id);
            return false;
        }
    }

    public async Task<ShoppingList> AddItemToListAsync(string listId, ShoppingItem item)
    {
        var list = await GetShoppingListById(listId);
        if (list == null)
        {
            throw new KeyNotFoundException($"找不到購物清單: {listId}");
        }

        item.Id = Guid.NewGuid().ToString();
        list.Items.Add(item);
        await SaveShoppingList(list);
        return list;
    }

    public async Task<ShoppingList> ToggleItemAsync(string listId, string itemId)
    {
        var list = await GetShoppingListById(listId);
        if (list == null)
        {
            throw new KeyNotFoundException($"找不到購物清單: {listId}");
        }

        var item = list.Items.FirstOrDefault(x => x.Id == itemId);
        if (item == null)
        {
            throw new KeyNotFoundException($"找不到購物項目: {itemId}");
        }

        item.IsCompleted = !item.IsCompleted;
        await SaveShoppingList(list);
        return list;
    }

    public async Task<IEnumerable<ShoppingItem>> GetItemsAsync()
    {
        var lists = await GetShoppingLists();
        return lists.SelectMany(list => list.Items).OrderBy(item => item.Name);
    }

    public async Task SaveChangesAsync(List<ShoppingItem> items, List<string> deletedIds)
    {
        // Validate all items first
        foreach (var item in items)
        {
            ValidateShoppingItem(item);
        }

        var lists = await GetShoppingLists();
        var allItems = lists.SelectMany(list => list.Items).ToList();

        // Remove deleted items
        foreach (var list in lists)
        {
            list.Items.RemoveAll(item => deletedIds.Contains(item.Id));
        }

        // Update or add items
        foreach (var item in items)
        {
            var existingItem = allItems.FirstOrDefault(x => x.Id == item.Id);
            if (existingItem != null)
            {
                // Update existing item
                var list = lists.First(l => l.Items.Any(i => i.Id == item.Id));
                var index = list.Items.FindIndex(i => i.Id == item.Id);
                list.Items[index] = item;
            }
            else
            {
                // Add new item to the first list (or create a new list if none exists)
                if (!lists.Any())
                {
                    var newList = new ShoppingList
                    {
                        Id = Guid.NewGuid().ToString(),
                        Title = "Default List",
                        CreatedAt = DateTime.UtcNow
                    };
                    lists.Add(newList);
                }

                if (string.IsNullOrEmpty(item.Id))
                {
                    item.Id = Guid.NewGuid().ToString();
                }
                lists.First().Items.Add(item);
            }
        }

        // Save all modified lists
        foreach (var list in lists)
        {
            await SaveShoppingList(list);
        }
    }
    /// <summary>
    /// 檢查商品項目是否有效
    /// </summary>
    /// <param name="item"></param>
    /// <exception cref="ArgumentException"></exception>
    private void ValidateShoppingItem(ShoppingItem item)
    {
        if (string.IsNullOrWhiteSpace(item.Name))
        {
            throw new ArgumentException("商品名稱不能為空");
        }

        if (item.Name.Length > 100)
        {
            throw new ArgumentException("商品名稱不能超過100個字元");
        }

        if (item.Quantity <= 0)
        {
            throw new ArgumentException("商品數量必須大於0");
        }

        if (item.Quantity > 9999)
        {
            throw new ArgumentException("商品數量不能超過9999");
        }

        if (item.Price <= 0)
        {
            throw new ArgumentException("商品金額不能為負數或0");
        }

        if (item.Price > 999999.99m)
        {
            throw new ArgumentException("商品金額不能超過999,999.99");
        }

        // 檢查價格小數位數不能超過2位
        if (Math.Round(item.Price, 2) != item.Price)
        {
            throw new ArgumentException("商品金額最多只能有兩位小數");
        }
    }
}