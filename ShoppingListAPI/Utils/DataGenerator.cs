using Bogus;
using ShoppingListAPI.Models;
using ShoppingListAPI.Services.FileDb;

namespace ShoppingListAPI.Utils;

/// <summary>
/// 資料生成器
/// </summary>
public class DataGenerator
{
    /// <summary>
    /// 日誌記錄器
    /// </summary>
    private readonly ILogger<DataGenerator> _logger;
    /// <summary>
    /// 檔案資料庫服務
    /// </summary>
    private readonly IFileDbService _fileDbService;
    /// <summary>
    /// 假數據生成器
    /// </summary>
    private readonly Faker _faker;

    /// <summary>
    /// 常見項目清單
    /// </summary>
    private readonly string[] _commonItems = new[]
    {
        "蘋果", "香蕉", "牛奶", "麵包", "蛋", "肉", "魚", "米", "麵條", "醬油",
        "鹽", "糖", "蔬菜", "水果", "飲料", "零食", "調味料", "罐頭", "餅乾", "咖啡",
        "茶", "果汁", "礦泉水", "衛生紙", "洗衣粉", "沐浴乳", "洗髮精", "牙膏", "肥皂", "垃圾袋"
    };

    /// <summary>
    /// 商品類別清單
    /// </summary>
    private readonly string[] _categories = new[]
    {
        "日常用品", "食品", "飲料", "零食", "生鮮", "清潔用品", "個人護理"
    };

    /// <summary>
    /// 初始化資料生成器
    /// </summary>
    /// <param name="logger">日誌記錄器</param>
    /// <param name="fileDbService">檔案資料庫服務</param> 
    public DataGenerator(ILogger<DataGenerator> logger, IFileDbService fileDbService)
    {
        _logger = logger;
        _fileDbService = fileDbService;
        _faker = new Faker("zh_TW");
    }

    /// <summary>
    /// 生成測試資料
    /// </summary>
    /// <param name="count">要生成的資料數量</param>
    /// <returns></returns>
    public async Task GenerateTestDataAsync(int count)
    {
        try
        {
            // 記錄開始生成測試資料
            _logger.LogInformation($"開始生成 {count} 筆測試資料");

            // 設定日期範圍：從 2024 年 1 月 1 日到現在
            var startDate = new DateTime(2024, 1, 1);
            // 結束日期為現在
            var endDate = DateTime.Now;
            // 記錄日期範圍
            _logger.LogInformation($"日期範圍：{startDate:yyyy-MM-dd} 到 {endDate:yyyy-MM-dd}");
            // 逐筆生成測試資料
            for (int i = 0; i < count; i++)
            {
                // 記錄正在生成第 i 筆測試資料
                _logger.LogInformation($"正在生成第 {i + 1} 筆測試資料");

                // 生成購買日期
                var buyDate = _faker.Date.Between(startDate, endDate);
                // 記錄生成購買日期
                _logger.LogInformation($"生成購買日期：{buyDate:yyyy-MM-dd}");
                // 生成清單標題
                var title = GenerateListTitle();
                // 記錄生成標題
                _logger.LogInformation($"生成標題：{title}");
                // 生成購物清單
                var list = new ShoppingList
                {
                    // 生成唯一識別碼
                    Id = Guid.NewGuid().ToString(),
                    // 設定標題
                    Title = title,
                    // 設定建立時間
                    CreatedAt = _faker.Date.Recent(7),
                    // 設定購買日期
                    BuyDate = buyDate,
                    // 設定是否已結束
                    Items = GenerateItems()
                };
                // 記錄開始儲存購物清單
                _logger.LogInformation($"開始儲存購物清單：{list.Id}");
                // 儲存購物清單
                await _fileDbService.CreateShoppingListAsync(list);
                // 記錄成功儲存購物清單
                _logger.LogInformation($"成功儲存購物清單：{list.Title}，購買日期：{list.BuyDate:yyyy-MM-dd}，項目數量：{list.Items.Count}");
            }
            // 記錄成功生成測試資料
            _logger.LogInformation($"成功生成 {count} 筆測試資料");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "生成測試資料時發生錯誤");
            throw;
        }
    }
    /// <summary>
    /// 生成清單標題
    /// </summary>
    /// <returns></returns>
    private string GenerateListTitle()
    {
        // 隨機選擇商品類別
        var category = _faker.PickRandom(_categories);
        // 隨機生成日期
        var date = _faker.Date.Recent(7).ToString("MM/dd");
        // 隨機選擇清單名稱後綴
        var suffix = _faker.Random.Bool() ? "購物清單" : "採買清單";
        // 生成清單標題
        return $"{date} {category}{suffix}";
    }
    /// <summary>
    /// 生成購物清單項目
    /// </summary>
    /// <returns></returns>
    private List<ShoppingItem> GenerateItems()
    {
        // 隨機生成 3 到 10 項商品
        var itemCount = _faker.Random.Int(3, 10);
        // 建立空的購物項目清單
        var items = new List<ShoppingItem>();
        // 逐項生成購物項目
        for (int i = 0; i < itemCount; i++)
        {
            var useCommonItem = _faker.Random.Bool(0.8f); // 80% 機率使用常見項目
            // 隨機選擇商品名稱
            var itemName = useCommonItem ?
                _faker.PickRandom(_commonItems) :
                $"{_faker.Commerce.ProductName()}";
            // 新增購物項目
            items.Add(new ShoppingItem
            {
                // 生成唯一識別碼
                Id = Guid.NewGuid().ToString(),
                // 設定商品名稱
                Name = itemName,
                // 隨機生成購買數量
                Quantity = _faker.Random.Int(1, 5),
                // 隨機生成單價
                Price = _faker.Random.Int(10, 500),
                // 隨機生成是否已完成
                IsCompleted = _faker.Random.Bool(0.3f), // 30% 機率已完成
                // 設定建立時間
                CreatedAt = DateTime.UtcNow,
                // 更新時間為空
                UpdatedAt = null
            });
        }

        return items;
    }
}