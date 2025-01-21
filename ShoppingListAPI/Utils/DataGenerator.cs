using Bogus;
using ShoppingListAPI.Models;
using ShoppingListAPI.Services.FileDb;

namespace ShoppingListAPI.Utils;

public class DataGenerator
{
    private readonly ILogger<DataGenerator> _logger;
    private readonly IFileDbService _fileDbService;
    private readonly Faker _faker;
    
    private readonly string[] _commonItems = new[]
    {
        "蘋果", "香蕉", "牛奶", "麵包", "蛋", "肉", "魚", "米", "麵條", "醬油",
        "鹽", "糖", "蔬菜", "水果", "飲料", "零食", "調味料", "罐頭", "餅乾", "咖啡",
        "茶", "果汁", "礦泉水", "衛生紙", "洗衣粉", "沐浴乳", "洗髮精", "牙膏", "肥皂", "垃圾袋"
    };
    
    private readonly string[] _categories = new[]
    {
        "日常用品", "食品", "飲料", "零食", "生鮮", "清潔用品", "個人護理"
    };

    public DataGenerator(ILogger<DataGenerator> logger, IFileDbService fileDbService)
    {
        _logger = logger;
        _fileDbService = fileDbService;
        _faker = new Faker("zh_TW");
    }

    public async Task GenerateTestDataAsync(int count)
    {
        try
        {
            _logger.LogInformation($"開始生成 {count} 筆測試資料");

            for (int i = 0; i < count; i++)
            {
                var list = new ShoppingList
                {
                    Id = Guid.NewGuid().ToString(),
                    Title = GenerateListTitle(),
                    CreatedAt = _faker.Date.Recent(7),
                    Items = GenerateItems()
                };

                await _fileDbService.SaveShoppingListAsync(list);
            }

            _logger.LogInformation($"成功生成 {count} 筆測試資料");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "生成測試資料時發生錯誤");
            throw;
        }
    }

    private string GenerateListTitle()
    {
        var category = _faker.PickRandom(_categories);
        var date = _faker.Date.Recent(7).ToString("MM/dd");
        var suffix = _faker.Random.Bool() ? "購物清單" : "採買清單";
        return $"{date} {category}{suffix}";
    }

    private List<ShoppingItem> GenerateItems()
    {
        var itemCount = _faker.Random.Int(3, 10);
        var items = new List<ShoppingItem>();

        for (int i = 0; i < itemCount; i++)
        {
            var useCommonItem = _faker.Random.Bool(0.8f); // 80% 機率使用常見項目
            var itemName = useCommonItem ? 
                _faker.PickRandom(_commonItems) : 
                $"{_faker.Commerce.ProductName()}";

            items.Add(new ShoppingItem
            {
                Id = Guid.NewGuid().ToString(),
                Name = itemName,
                Quantity = _faker.Random.Int(1, 5),
                Price = _faker.Random.Int(10, 500),
                IsCompleted = _faker.Random.Bool(0.3f), // 30% 機率已完成
                CreatedAt = DateTime.UtcNow,
                CompletedAt = null
            });
        }

        return items;
    }
}