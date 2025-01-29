using ShoppingListAPI.Models;

namespace ShoppingListAPI.Services.FileDb;

/// <summary>
/// 檔案資料庫服務介面
/// 定義購物清單的資料存取操作
/// </summary>
public interface IFileDbService
{
    /// <summary>
    /// 取得所有購物清單
    /// </summary>
    /// <returns>購物清單集合</returns>
    Task<IEnumerable<ShoppingList>> GetAllShoppingListsAsync();

    /// <summary>
    /// 根據 ID 取得特定購物清單
    /// </summary>
    /// <param name="id">購物清單 ID</param>
    /// <returns>購物清單，如果找不到則回傳 null</returns>
    Task<ShoppingList?> GetShoppingListAsync(string id);

    /// <summary>
    /// 建立新的購物清單
    /// </summary>
    /// <param name="list">要建立的購物清單資料</param>
    /// <returns>已建立的購物清單，包含新生成的 ID</returns>
    Task<ShoppingList> CreateShoppingListAsync(ShoppingList list);

    /// <summary>
    /// 更新現有的購物清單
    /// </summary>
    /// <param name="list">要更新的購物清單資料</param>
    /// <returns>更新後的購物清單</returns>
    Task<ShoppingList> UpdateShoppingListAsync(ShoppingList list);

    /// <summary>
    /// 刪除指定的購物清單
    /// </summary>
    /// <param name="id">要刪除的購物清單 ID</param>
    /// <returns>非同步操作</returns>
    Task DeleteShoppingList(string id);

    /// <summary>
    /// 檢查是否有購物清單
    /// </summary>
    /// <returns>是否有購物清單</returns>
    bool HasAnyShoppingLists();
}