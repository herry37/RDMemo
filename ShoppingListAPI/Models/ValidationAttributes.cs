using System;
using System.ComponentModel.DataAnnotations;

namespace ShoppingListAPI.Models;

/// <summary>
/// 購物清單標題驗證屬性
/// 驗證購物清單標題的格式和長度
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class ShoppingListTitleAttribute : ValidationAttribute
{
    /// <summary>
    /// 驗證標題是否符合規則
    /// </summary>
    /// <param name="value">要驗證的標題值</param>
    /// <param name="validationContext">驗證上下文</param>
    /// <returns>驗證結果</returns>
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        // 檢查值是否為空
        if (value == null)
        {
            return new ValidationResult("標題不能為空");
        }

        var title = value.ToString();

        // 檢查標題長度
        if (string.IsNullOrWhiteSpace(title))
        {
            return new ValidationResult("標題不能為空白");
        }

        if (title.Length > 100)
        {
            return new ValidationResult("標題長度不能超過 100 個字元");
        }

        // 檢查標題是否包含無效字元
        if (title.Contains("@") || title.Contains("#") || title.Contains("$"))
        {
            return new ValidationResult("標題不能包含特殊字元 (@, #, $)");
        }

        return ValidationResult.Success;
    }
}

/// <summary>
/// 購物項目名稱驗證屬性
/// 驗證購物項目名稱的格式和長度
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class ShoppingItemNameAttribute : ValidationAttribute
{
    /// <summary>
    /// 驗證項目名稱是否符合規則
    /// </summary>
    /// <param name="value">要驗證的名稱值</param>
    /// <param name="validationContext">驗證上下文</param>
    /// <returns>驗證結果</returns>
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        // 檢查值是否為空
        if (value == null)
        {
            return new ValidationResult("項目名稱不能為空");
        }

        var name = value.ToString();

        // 檢查名稱長度
        if (string.IsNullOrWhiteSpace(name))
        {
            return new ValidationResult("項目名稱不能為空白");
        }

        if (name.Length > 100)
        {
            return new ValidationResult("項目名稱長度不能超過 100 個字元");
        }

        // 檢查名稱是否包含無效字元
        if (name.Any(c => !char.IsLetterOrDigit(c) && !char.IsWhiteSpace(c) && c != '-' && c != '_'))
        {
            return new ValidationResult("項目名稱只能包含字母、數字、空格、連字符和底線");
        }

        return ValidationResult.Success;
    }
}

/// <summary>
/// 購物項目數量驗證屬性
/// 驗證購物項目數量的範圍
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class ShoppingItemQuantityAttribute : ValidationAttribute
{
    /// <summary>
    /// 驗證數量是否符合規則
    /// </summary>
    /// <param name="value">要驗證的數量值</param>
    /// <param name="validationContext">驗證上下文</param>
    /// <returns>驗證結果</returns>
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        // 檢查值是否為空
        if (value == null)
        {
            return new ValidationResult("數量不能為空");
        }

        // 檢查數量範圍
        if (value is int quantity)
        {
            if (quantity < 1)
            {
                return new ValidationResult("數量必須大於 0");
            }

            if (quantity > 9999)
            {
                return new ValidationResult("數量不能超過 9999");
            }

            return ValidationResult.Success;
        }

        return new ValidationResult("數量必須是整數");
    }
}

/// <summary>
/// 購物項目價格驗證屬性
/// 驗證購物項目價格的範圍和格式
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class ShoppingItemPriceAttribute : ValidationAttribute
{
    /// <summary>
    /// 驗證價格是否符合規則
    /// </summary>
    /// <param name="value">要驗證的價格值</param>
    /// <param name="validationContext">驗證上下文</param>
    /// <returns>驗證結果</returns>
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        // 允許空值（表示價格未設定）
        if (value == null)
        {
            return ValidationResult.Success;
        }

        // 檢查價格範圍
        if (value is decimal price)
        {
            if (price < 0)
            {
                return new ValidationResult("價格不能為負數");
            }

            if (price > 999999.99m)
            {
                return new ValidationResult("價格不能超過 999,999.99");
            }

            // 檢查小數位數
            var decimalPlaces = BitConverter.GetBytes(decimal.GetBits(price)[3])[2];
            if (decimalPlaces > 2)
            {
                return new ValidationResult("價格最多只能有兩位小數");
            }

            return ValidationResult.Success;
        }

        return new ValidationResult("價格必須是有效的金額");
    }
}
