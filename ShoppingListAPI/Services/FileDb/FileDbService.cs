using ShoppingListAPI.Models;
using System.Text.Json;

namespace ShoppingListAPI.Services.FileDb
{
    /// <summary>
    /// 檔案資料庫服務實作
    /// 使用 JSON 檔案作為資料儲存媒介
    /// </summary>
    public class FileDbService : IFileDbService
    {
        /// <summary>
        /// 日誌服務
        /// </summary>
        private readonly ILogger<FileDbService> _logger;

        /// <summary>
        /// 資料目錄路徑
        /// </summary>
        private readonly string _dataDirectory;

        /// <summary>
        /// 檔案鎖定物件，用於同步存取
        /// </summary>
        private static readonly object _fileLock = new();

        /// <summary>
        /// 建構函式
        /// </summary>
        /// <param name="configuration">應用程式設定</param>
        /// <param name="logger">日誌服務</param>
        public FileDbService(IConfiguration configuration, ILogger<FileDbService> logger)
        {
            _logger = logger;

            // 使用 Data/FileStore/shoppinglists 作為資料目錄
            _dataDirectory = Path.Combine(
                Directory.GetCurrentDirectory(),
                "Data",
                "FileStore",
                "shoppinglists"
            );

            // 確保目錄存在
            if (!Directory.Exists(_dataDirectory))
            {
                Directory.CreateDirectory(_dataDirectory);
                _logger.LogInformation("已建立資料目錄: {Directory}", _dataDirectory);
            }
        }

        /// <summary>
        /// 取得所有購物清單
        /// </summary>
        /// <returns>購物清單集合</returns>
        public async Task<IEnumerable<ShoppingList>> GetAllShoppingListsAsync()
        {
            try
            {
                _logger.LogInformation("準備讀取購物清單資料");
                var lists = new List<ShoppingList>();

                // 確保目錄存在
                if (!Directory.Exists(_dataDirectory))
                {
                    _logger.LogWarning("資料目錄不存在，建立新目錄: {Directory}", _dataDirectory);
                    Directory.CreateDirectory(_dataDirectory);
                    return lists;
                }

                // 取得目錄中所有的 JSON 檔案
                var files = Directory.GetFiles(_dataDirectory, "*.json");
                _logger.LogInformation("找到 {Count} 個 JSON 檔案", files.Length);

                foreach (var file in files)
                {
                    try
                    {
                        // 讀取檔案內容
                        var json = await File.ReadAllTextAsync(file);
                        if (string.IsNullOrWhiteSpace(json))
                        {
                            _logger.LogWarning("檔案內容為空: {File}", file);
                            continue;
                        }

                        // 反序列化
                        var options = new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        };
                        // 反序列化資料
                        var list = JsonSerializer.Deserialize<ShoppingList>(json, options);
                        if (list != null)
                        {
                            // 確保所有必要欄位都有值
                            list.Items ??= new List<ShoppingItem>();
                            foreach (var item in list.Items)
                            {
                                item.Quantity = Math.Max(1, item.Quantity);
                                item.Price ??= 0;
                            }

                            // 重新計算總金額
                            list.totalAmount = list.Items.Sum(item =>
                                (item.Price ?? 0) * Math.Max(1, item.Quantity));

                            lists.Add(list);

                            _logger.LogInformation(
                                "載入購物清單 {ListId}: {Title}, 項目數量: {ItemCount}, 總金額: {Total}",
                                list.Id,
                                list.Title,
                                list.Items.Count,
                                list.totalAmount);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "讀取檔案失敗: {File}", file);
                    }
                }

                _logger.LogInformation("成功載入 {Count} 個購物清單", lists.Count);
                return lists.OrderByDescending(x => x.BuyDate);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "取得購物清單時發生錯誤");
                throw;
            }
        }

        /// <summary>
        /// 根據 ID 取得特定購物清單
        /// </summary>
        /// <param name="id">購物清單 ID</param>
        /// <returns>購物清單，如果找不到則回傳 null</returns>
        public async Task<ShoppingList?> GetShoppingListAsync(string id)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                {
                    throw new ArgumentException("購物清單 ID 不能為空", nameof(id));
                }

                // 檢查檔案是否存在
                var filePath = Path.Combine(_dataDirectory, $"{id}.json");
                if (!File.Exists(filePath))
                {
                    _logger.LogWarning("找不到購物清單檔案: {File}", filePath);
                    return null;
                }

                // 讀取檔案內容
                var json = await File.ReadAllTextAsync(filePath);
                if (string.IsNullOrWhiteSpace(json))
                {
                    _logger.LogWarning("購物清單檔案內容為空: {File}", filePath);
                    return null;
                }

                // 反序列化
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                // 反序列化資料
                var list = JsonSerializer.Deserialize<ShoppingList>(json, options);
                if (list == null)
                {
                    _logger.LogWarning("無法反序列化購物清單: {File}", filePath);
                    return null;
                }

                // 確保所有必要欄位都有值
                list.Items ??= new List<ShoppingItem>();

                return list;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "取得購物清單 {ListId} 失敗", id);
                throw;
            }
        }

        /// <summary>
        /// 建立新的購物清單
        /// </summary>
        /// <param name="list">要建立的購物清單資料</param>
        /// <returns>已建立的購物清單，包含新生成的 ID</returns>
        public async Task<ShoppingList> CreateShoppingListAsync(ShoppingList list)
        {
            try
            {
                // 設定新清單的屬性
                list.Id = Guid.NewGuid().ToString();
                list.CreatedAt = DateTime.UtcNow;
                list.UpdatedAt = list.CreatedAt;

                // 確保購物項目列表已初始化
                list.Items ??= new List<ShoppingItem>();

                // 為每個購物項目設定 ID 和時間戳記
                foreach (var item in list.Items)
                {
                    item.Id = Guid.NewGuid().ToString();
                    item.CreatedAt = DateTime.UtcNow;
                    item.UpdatedAt = item.CreatedAt;
                }

                // 儲存購物清單
                await SaveShoppingListToFile(list);

                return list;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "建立購物清單失敗");
                throw;
            }
        }

        /// <summary>
        /// 更新現有的購物清單
        /// </summary>
        /// <param name="list">要更新的購物清單資料</param>
        /// <returns>更新後的購物清單</returns>
        public async Task<ShoppingList> UpdateShoppingListAsync(ShoppingList list)
        {
            try
            {
                _logger.LogInformation("開始更新購物清單 {ListId}", list.Id);

                // 找到要更新的清單
                var existingList = await GetShoppingListAsync(list.Id);
                if (existingList == null)
                {
                    throw new KeyNotFoundException($"找不到 ID 為 {list.Id} 的購物清單");
                }

                // 更新清單資料
                existingList.Items = list.Items;
                existingList.UpdatedAt = list.UpdatedAt;

                // 重新計算總金額
                existingList.totalAmount = existingList.Items.Sum(item =>
                    (item.Price ?? 0) * Math.Max(1, item.Quantity));

                // 儲存更新後的購物清單
                await SaveShoppingListToFile(existingList);

                _logger.LogInformation(
                    "成功更新購物清單 {ListId}, 項目數量: {ItemCount}, 總金額: {Total}",
                    existingList.Id,
                    existingList.Items.Count,
                    existingList.totalAmount);

                return existingList;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "更新購物清單 {ListId} 失敗", list.Id);
                throw;
            }
        }

        /// <summary>
        /// 刪除指定的購物清單
        /// </summary>
        /// <param name="id">要刪除的購物清單 ID</param>
        public async Task DeleteShoppingList(string id)
        {
            try
            {
                // 檢查 ID 是否為空
                if (string.IsNullOrWhiteSpace(id))
                {
                    throw new ArgumentException("購物清單 ID 不能為空", nameof(id));
                }
                // 檢查檔案是否存在
                var filePath = Path.Combine(_dataDirectory, $"{id}.json");
                if (!File.Exists(filePath))
                {
                    _logger.LogWarning("找不到要刪除的購物清單檔案: {File}", filePath);
                    return;
                }

                // 刪除檔案
                File.Delete(filePath);
                _logger.LogInformation("已刪除購物清單檔案: {File}", filePath);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "刪除購物清單時發生錯誤: {ListId}", id);
                throw;
            }
        }

        /// <summary>
        /// 檢查是否有任何購物清單
        /// </summary>
        /// <returns>是否有購物清單</returns>
        public bool HasAnyShoppingLists()
        {
            try
            {
                // 取得目錄中所有的 JSON 檔案
                var files = Directory.GetFiles(_dataDirectory, "*.json");
                return files.Length > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "檢查購物清單時發生錯誤");
                return false;
            }
        }

        /// <summary>
        /// 將購物清單儲存到檔案
        /// </summary>
        /// <param name="list">要儲存的購物清單</param>
        private async Task SaveShoppingListToFile(ShoppingList list)
        {
            try
            {
                // 確保資料夾存在
                var filePath = Path.Combine(_dataDirectory, $"{list.Id}.json");

                // 確保所有必要欄位都有值
                list.Items ??= new List<ShoppingItem>();

                // 重新計算總金額
                list.totalAmount = list.Items.Sum(item =>
                    (item.Price ?? 0) * Math.Max(1, item.Quantity));

                var options = new JsonSerializerOptions
                {
                    // 保留原有的屬性名稱大小寫
                    WriteIndented = true,
                    PropertyNamingPolicy = null // 保持原有的屬性名稱大小寫
                };
                // 序列化資料
                var json = JsonSerializer.Serialize(list, options);
                // 寫入檔案
                await File.WriteAllTextAsync(filePath, json);

                _logger.LogInformation(
                    "已儲存購物清單 {ListId}: {Title}, 項目數量: {ItemCount}, 總金額: {Total}",
                    list.Id,
                    list.Title,
                    list.Items.Count,
                    list.totalAmount);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "儲存購物清單到檔案失敗");
                throw;
            }
        }
    }
}