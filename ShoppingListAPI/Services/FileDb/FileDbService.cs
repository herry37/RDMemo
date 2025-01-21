using System.Text.Json;
using Microsoft.Extensions.Caching.Memory;
using ShoppingListAPI.Models;

namespace ShoppingListAPI.Services.FileDb;

public class FileDbService : IFileDbService
{
    private readonly string _baseDirectory;
    private readonly ILogger<FileDbService> _logger;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly IMemoryCache _cache;
    private const string CacheKeyPrefix = "ShoppingList_";
    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(5);

    public FileDbService(
        IWebHostEnvironment env,
        ILogger<FileDbService> logger,
        IMemoryCache cache)
    {
        _baseDirectory = Path.Combine(env.ContentRootPath, "Data", "FileStore", "shoppinglists");
        _logger = logger;
        _cache = cache;
        _jsonOptions = new JsonSerializerOptions { WriteIndented = true };
        
        if (!Directory.Exists(_baseDirectory))
        {
            Directory.CreateDirectory(_baseDirectory);
        }
    }

    public async Task<ShoppingList> GetShoppingListAsync(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            throw new ArgumentException("ID不能為空", nameof(id));
        }

        var cacheKey = $"{CacheKeyPrefix}{id}";
        if (_cache.TryGetValue<ShoppingList>(cacheKey, out var cachedList))
        {
            return cachedList!;
        }

        var path = GetFilePath(id);
        if (!File.Exists(path))
        {
            throw new KeyNotFoundException($"找不到ID為 {id} 的購物清單");
        }

        try
        {
            var json = await File.ReadAllTextAsync(path);
            var list = JsonSerializer.Deserialize<ShoppingList>(json, _jsonOptions);

            if (list == null)
            {
                throw new JsonException($"無法解析ID為 {id} 的購物清單");
            }

            _cache.Set(cacheKey, list, CacheDuration);
            return list;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "讀取購物清單時發生錯誤");
            throw;
        }
    }

    public async Task<List<ShoppingList>> GetAllShoppingListsAsync(int page = 1, int pageSize = 20)
    {
        try
        {
            var allLists = await GetAllAsync();
            return allLists
                .OrderByDescending(l => l.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "取得所有購物清單時發生錯誤");
            throw;
        }
    }

    public async Task<ShoppingList> CreateShoppingListAsync(ShoppingList list)
    {
        if (list == null)
        {
            throw new ArgumentNullException(nameof(list));
        }

        if (string.IsNullOrEmpty(list.Id))
        {
            list.Id = Guid.NewGuid().ToString();
        }

        await SaveShoppingListAsync(list);
        return list;
    }

    public async Task<ShoppingList> SaveShoppingListAsync(ShoppingList list)
    {
        if (list == null)
        {
            throw new ArgumentNullException(nameof(list));
        }

        var path = GetFilePath(list.Id);
        try
        {
            var json = JsonSerializer.Serialize(list, _jsonOptions);
            await File.WriteAllTextAsync(path, json);

            var cacheKey = $"{CacheKeyPrefix}{list.Id}";
            _cache.Set(cacheKey, list, CacheDuration);

            return list;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "保存購物清單時發生錯誤");
            throw;
        }
    }

    public async Task<ShoppingList> AddItemToListAsync(string listId, ShoppingItem item)
    {
        var list = await GetShoppingListAsync(listId);
        
        if (item == null)
        {
            throw new ArgumentNullException(nameof(item));
        }

        if (string.IsNullOrEmpty(item.Id))
        {
            item.Id = Guid.NewGuid().ToString();
        }

        list.Items.Add(item);
        await SaveShoppingListAsync(list);
        return list;
    }

    public async Task<ShoppingList> ToggleItemAsync(string listId, string itemId)
    {
        var list = await GetShoppingListAsync(listId);
        var item = list.Items.FirstOrDefault(i => i.Id == itemId);
        
        if (item == null)
        {
            throw new KeyNotFoundException($"找不到ID為 {itemId} 的項目");
        }

        item.IsCompleted = !item.IsCompleted;
        await SaveShoppingListAsync(list);
        return list;
    }

    public async Task DeleteShoppingListAsync(string id)
    {
        var path = GetFilePath(id);
        if (!File.Exists(path))
        {
            throw new KeyNotFoundException($"找不到ID為 {id} 的購物清單");
        }

        try
        {
            File.Delete(path);
            var cacheKey = $"{CacheKeyPrefix}{id}";
            _cache.Remove(cacheKey);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "刪除購物清單時發生錯誤");
            throw;
        }
    }

    public async Task<List<ShoppingList>> GetAllAsync()
    {
        var lists = new List<ShoppingList>();
        try
        {
            var files = Directory.GetFiles(_baseDirectory, "*.json");
            foreach (var file in files)
            {
                var json = await File.ReadAllTextAsync(file);
                var list = JsonSerializer.Deserialize<ShoppingList>(json, _jsonOptions);
                if (list != null)
                {
                    lists.Add(list);
                }
            }
            return lists;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "讀取所有購物清單時發生錯誤");
            throw;
        }
    }

    public async Task<bool> DeleteAsync(string id)
    {
        try
        {
            await DeleteShoppingListAsync(id);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"刪除ID為 {id} 的購物清單時發生錯誤");
            return false;
        }
    }

    private string GetFilePath(string id)
    {
        return Path.Combine(_baseDirectory, $"{id}.json");
    }
}