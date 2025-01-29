using System.Text.RegularExpressions;

namespace ShoppingListAPI.Utils;

/// <summary>
/// 驗證工具類別
/// 提供各種資料驗證方法，包含購物清單和項目的所有欄位驗證
/// 同時提供前端可用的驗證規則
/// </summary>
public static class ValidationUtils
{
    // 定義常數以提高可維護性
    private const int MaxTitleLength = 100;
    private const int MaxItemNameLength = 100;
    private const int MaxIdLength = 50;
    private const int MinQuantity = 1;
    private const int MaxQuantity = 9999;
    private const decimal MaxPrice = 999999.99m;
    private const int MaxPriceDecimalPlaces = 2;

    // 定義正則表達式模式
    private const string ItemNamePattern = @"^[\w\s\-]+$";
    private const string IdPattern = @"^[a-zA-Z0-9\-_]+$";
    private static readonly char[] InvalidTitleChars = { '@', '#', '$' };

    /// <summary>
    /// 驗證購物清單標題
    /// </summary>
    /// <param name="title">要驗證的標題字串</param>
    /// <returns>驗證失敗時返回錯誤訊息，驗證成功返回 null</returns>
    /// <remarks>
    /// 驗證規則：
    /// 1. 不能為空或只包含空白字元
    /// 2. 長度不能超過 100 個字元
    /// 3. 不能包含特殊字元 (@, #, $)
    /// </remarks>
    public static string ValidateTitle(string title)
    {
        // 檢查標題是否為空或只包含空白字元
        if (string.IsNullOrWhiteSpace(title))
        {
            return "標題不能為空";
        }

        // 檢查標題長度是否超過限制
        if (title.Length > MaxTitleLength)
        {
            return $"標題長度不能超過 {MaxTitleLength} 個字元";
        }

        // 檢查是否包含無效的特殊字元
        if (title.IndexOfAny(InvalidTitleChars) >= 0)
        {
            return "標題不能包含特殊字元 (@, #, $)";
        }

        return null;
    }

    /// <summary>
    /// 驗證購物項目名稱
    /// </summary>
    /// <param name="name">要驗證的項目名稱</param>
    /// <returns>驗證失敗時返回錯誤訊息，驗證成功返回 null</returns>
    /// <remarks>
    /// 驗證規則：
    /// 1. 不能為空或只包含空白字元
    /// 2. 長度不能超過 100 個字元
    /// 3. 只能包含字母、數字、空格、連字符和底線
    /// </remarks>
    public static string ValidateItemName(string name)
    {
        // 檢查名稱是否為空或只包含空白字元
        if (string.IsNullOrWhiteSpace(name))
        {
            return "項目名稱不能為空";
        }

        // 檢查名稱長度是否超過限制
        if (name.Length > MaxItemNameLength)
        {
            return $"項目名稱長度不能超過 {MaxItemNameLength} 個字元";
        }

        // 使用正則表達式檢查名稱格式
        if (!Regex.IsMatch(name, ItemNamePattern))
        {
            return "項目名稱只能包含字母、數字、空格、連字符和底線";
        }

        return null;
    }

    /// <summary>
    /// 驗證購物項目數量
    /// </summary>
    /// <param name="quantity">要驗證的數量</param>
    /// <returns>驗證失敗時返回錯誤訊息，驗證成功返回 null</returns>
    /// <remarks>
    /// 驗證規則：
    /// 1. 必須大於 0
    /// 2. 不能超過 9999
    /// </remarks>
    public static string ValidateQuantity(int quantity)
    {
        // 檢查數量是否小於最小值
        if (quantity < MinQuantity)
        {
            return $"數量必須大於 {MinQuantity - 1}";
        }

        // 檢查數量是否超過最大值
        if (quantity > MaxQuantity)
        {
            return $"數量不能超過 {MaxQuantity}";
        }

        return null;
    }

    /// <summary>
    /// 驗證購物項目價格
    /// </summary>
    /// <param name="price">要驗證的價格（可為 null）</param>
    /// <returns>驗證失敗時返回錯誤訊息，驗證成功返回 null</returns>
    /// <remarks>
    /// 驗證規則：
    /// 1. 可以為 null（表示價格未設定）
    /// 2. 不能為負數
    /// 3. 不能超過 999,999.99
    /// 4. 最多只能有兩位小數
    /// </remarks>
    public static string ValidatePrice(decimal? price)
    {
        // 允許價格為 null
        if (price == null)
        {
            return null;
        }

        // 檢查價格是否為負數
        if (price < 0)
        {
            return "價格不能為負數";
        }

        // 檢查價格是否超過最大值
        if (price > MaxPrice)
        {
            return $"價格不能超過 {MaxPrice:N2}";
        }

        // 檢查小數位數是否超過限制
        var decimalPlaces = BitConverter.GetBytes(decimal.GetBits(price.Value)[3])[2];
        if (decimalPlaces > MaxPriceDecimalPlaces)
        {
            return $"價格最多只能有 {MaxPriceDecimalPlaces} 位小數";
        }

        return null;
    }

    /// <summary>
    /// 驗證識別碼（ID）
    /// </summary>
    /// <param name="id">要驗證的識別碼</param>
    /// <returns>驗證失敗時返回錯誤訊息，驗證成功返回 null</returns>
    /// <remarks>
    /// 驗證規則：
    /// 1. 不能為空或只包含空白字元
    /// 2. 只能包含字母、數字、連字符和底線
    /// 3. 長度不能超過 50 個字元
    /// </remarks>
    public static string ValidateId(string id)
    {
        // 檢查 ID 是否為空或只包含空白字元
        if (string.IsNullOrWhiteSpace(id))
        {
            return "ID 不能為空";
        }

        // 使用正則表達式檢查 ID 格式
        if (!Regex.IsMatch(id, IdPattern))
        {
            return "ID 只能包含字母、數字、連字符和底線";
        }

        // 檢查 ID 長度是否超過限制
        if (id.Length > MaxIdLength)
        {
            return $"ID 長度不能超過 {MaxIdLength} 個字元";
        }

        return null;
    }

    /// <summary>
    /// 取得前端使用的驗證規則
    /// </summary>
    /// <returns>包含所有驗證規則的物件，可序列化為 JSON 供前端使用</returns>
    /// <remarks>
    /// 返回的物件包含：
    /// 1. 每個欄位的驗證規則
    /// 2. 對應的錯誤訊息
    /// 3. 限制值（最大長度、最小值、最大值等）
    /// </remarks>
    public static object GetClientValidationRules()
    {
        return new
        {
            Title = new
            {
                Required = true,
                MaxLength = MaxTitleLength,
                Pattern = $"^[^{string.Join("", InvalidTitleChars)}]+$",
                Messages = new
                {
                    Required = "標題不能為空",
                    MaxLength = $"標題長度不能超過 {MaxTitleLength} 個字元",
                    Pattern = "標題不能包含特殊字元 (@, #, $)"
                }
            },
            ItemName = new
            {
                Required = true,
                MaxLength = MaxItemNameLength,
                Pattern = ItemNamePattern,
                Messages = new
                {
                    Required = "項目名稱不能為空",
                    MaxLength = $"項目名稱長度不能超過 {MaxItemNameLength} 個字元",
                    Pattern = "項目名稱只能包含字母、數字、空格、連字符和底線"
                }
            },
            Quantity = new
            {
                Required = true,
                Min = MinQuantity,
                Max = MaxQuantity,
                Messages = new
                {
                    Required = "數量不能為空",
                    Min = $"數量必須大於 {MinQuantity - 1}",
                    Max = $"數量不能超過 {MaxQuantity}"
                }
            },
            Price = new
            {
                Required = false,
                Min = 0,
                Max = MaxPrice,
                DecimalPlaces = MaxPriceDecimalPlaces,
                Messages = new
                {
                    Min = "價格不能為負數",
                    Max = $"價格不能超過 {MaxPrice:N2}",
                    DecimalPlaces = $"價格最多只能有 {MaxPriceDecimalPlaces} 位小數"
                }
            },
            Id = new
            {
                Required = true,
                MaxLength = MaxIdLength,
                Pattern = IdPattern,
                Messages = new
                {
                    Required = "ID 不能為空",
                    MaxLength = $"ID 長度不能超過 {MaxIdLength} 個字元",
                    Pattern = "ID 只能包含字母、數字、連字符和底線"
                }
            }
        };
    }
}
