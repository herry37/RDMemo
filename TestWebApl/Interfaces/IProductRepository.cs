using TestWebApl.Entitie;

namespace TestWebApl.Interfaces
{
    /// <summary>
    /// 產品資料存儲庫介面
    /// </summary>
    public interface IProductRepository
    {
        /// <summary>
        /// 取得所有產品
        /// </summary>
        /// <returns>所有產品</returns>
        Task<IEnumerable<Product>> GetAllAsync();
        /// <summary>
        ///  取得單一產品
        /// </summary>
        /// <param name="id">產品編號</param>
        /// <returns>單一產品</returns>
        Task<Product> GetByIdAsync(int id);
        /// <summary>
        ///  新增產品
        /// </summary>
        /// <param name="product">要新增的產品</param>
        /// <returns>已經新增的產品</returns>
        Task AddAsync(Product product);
        /// <summary>
        ///  更新產品
        /// </summary>
        /// <param name="product">要更新的產品</param>
        /// <returns>已經更新的產品</returns>
        Task UpdateAsync(Product product);
        /// <summary>
        ///  刪除產品
        /// </summary>
        /// <param name="id">要刪除的產品編號</param>
        /// <returns>已經刪除的產品</returns>
        Task DeleteAsync(int id);
    }
}
