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
    private static readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

    public FileDbService(
        IWebHostEnvironment env,
        ILogger<FileDbService> logger,
        IMemoryCache cache)
    {
        _baseDirectory = Path.Combine(env.ContentRootPath, "Data", "FileStore", "shoppinglists");
        _logger = logger;
        _cache = cache;
        _jsonOptions = new JsonSerializerOptions { WriteIndented = true };
        
        // 初始化資料目錄
        InitializeDataDirectory().GetAwaiter().GetResult();
    }

    private async Task InitializeDataDirectory()
    {
        await _semaphore.WaitAsync();
        try
        {
            if (!Directory.Exists(_baseDirectory))
            {
                _logger.LogInformation($"建立資料目錄：{_baseDirectory}");
                Directory.CreateDirectory(_baseDirectory);
            }
        }
        finally
        {
            _semaphore.Release();
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

        await _semaphore.WaitAsync();
        try
        {
            var path = GetFilePath(id);
            if (!File.Exists(path))
            {
                throw new KeyNotFoundException($"找不到ID為 {id} 的購物清單");
            }

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
        finally
        {
            _semaphore.Release();
        }
    }

    public async Task<List<ShoppingList>> GetAllAsync()
    {
        var lists = new List<ShoppingList>();
        await _semaphore.WaitAsync();
        try
        {
            _logger.LogInformation($"開始讀取購物清單，目錄：{_baseDirectory}");
            
            if (!Directory.Exists(_baseDirectory))
            {
                _logger.LogWarning($"目錄不存在，建立新目錄：{_baseDirectory}");
                Directory.CreateDirectory(_baseDirectory);
                return lists;
            }

            var files = Directory.GetFiles(_baseDirectory, "*.json");
            _logger.LogInformation($"找到 {files.Length} 個購物清單檔案");

            foreach (var file in files)
            {
                try
                {
                    _logger.LogInformation($"讀取檔案：{Path.GetFileName(file)}");
                    var json = await File.ReadAllTextAsync(file);
                    var list = JsonSerializer.Deserialize<ShoppingList>(json, _jsonOptions);
                    
                    if (list != null)
                    {
                        lists.Add(list);
                        var cacheKey = $"{CacheKeyPrefix}{list.Id}";
                        _cache.Set(cacheKey, list, CacheDuration);
                        _logger.LogInformation($"成功讀取購物清單：{list.Title}");
                    }
                    else
                    {
                        _logger.LogWarning($"檔案 {Path.GetFileName(file)} 解析後為空");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"讀取檔案 {Path.GetFileName(file)} 時發生錯誤");
                    continue;
                }
            }
            
            _logger.LogInformation($"成功讀取 {lists.Count} 個購物清單");
            return lists;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "讀取所有購物清單時發生錯誤");
            throw;
        }
        finally
        {
            _semaphore.Release();
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

        await _semaphore.WaitAsync();
        try
        {
            var path = GetFilePath(list.Id);
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
        finally
        {
            _semaphore.Release();
        }
    }

    public async Task<ShoppingList> UpdateShoppingListAsync(ShoppingList list)
    {
        var lists = await GetAllAsync();
        var index = lists.FindIndex(x => x.Id == list.Id);
        if (index == -1)
        {
            throw new KeyNotFoundException($"找不到ID為 {list.Id} 的購物清單");
        }

        lists[index] = list;
        await SaveListsToFileAsync(lists);
        return list;
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
        await _semaphore.WaitAsync();
        try
        {
            if (!File.Exists(path))
            {
                throw new KeyNotFoundException($"找不到ID為 {id} 的購物清單");
            }

            File.Delete(path);
            var cacheKey = $"{CacheKeyPrefix}{id}";
            _cache.Remove(cacheKey);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "刪除購物清單時發生錯誤");
            throw;
        }
        finally
        {
            _semaphore.Release();
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

    public async Task<int> BatchDeleteAsync(int year, int month)
    {
        await _semaphore.WaitAsync();
        try
        {
            _logger.LogInformation($"開始批量刪除 {year}年{month}月 的購物清單");
            
            var lists = await GetAllAsync();
            var listsToDelete = lists.Where(l => 
                l.BuyDate?.Year == year && 
                l.BuyDate?.Month == month).ToList();

            _logger.LogInformation($"找到 {listsToDelete.Count} 筆符合條件的購物清單");

            foreach (var list in listsToDelete)
            {
                try
                {
                    var path = GetFilePath(list.Id);
                    if (File.Exists(path))
                    {
                        File.Delete(path);
                        var cacheKey = $"{CacheKeyPrefix}{list.Id}";
                        _cache.Remove(cacheKey);
                        _logger.LogInformation($"成功刪除購物清單：{list.Title}");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"刪除購物清單 {list.Id} 時發生錯誤");
                }
            }

            return listsToDelete.Count;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"批量刪除 {year}年{month}月 的購物清單時發生錯誤");
            throw;
        }
        finally
        {
            _semaphore.Release();
        }
    }

    private async Task SaveListsToFileAsync(List<ShoppingList> lists)
    {
        await _semaphore.WaitAsync();
        try
        {
            foreach (var list in lists)
            {
                var path = GetFilePath(list.Id);
                var json = JsonSerializer.Serialize(list, _jsonOptions);
                await File.WriteAllTextAsync(path, json);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "保存購物清單列表時發生錯誤");
            throw;
        }
        finally
        {
            _semaphore.Release();
        }
    }

    private string GetFilePath(string id)
    {
        return Path.Combine(_baseDirectory, $"{id}.json");
    }
}