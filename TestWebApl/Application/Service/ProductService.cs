using TestWebApl.Entitie;
using TestWebApl.Interfaces;

namespace TestWebApl.Application.Service
{
    /// <summary>
    /// 產品服務
    /// </summary>
    public class ProductService
    {
        private readonly IProductRepository _productRepository;
        /// <summary>
        ///  建構函數
        /// </summary>
        /// <param name="productRepository">產品存儲庫</param>
        public ProductService(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        /// <summary>
        ///  取得所有產品
        /// </summary>
        /// <returns>所有產品</returns>
        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            return await _productRepository.GetAllAsync();
        }

        /// <summary>
        ///  取得單一產品
        /// </summary>
        /// <param name="id">產品編號</param>
        /// <returns>單一產品</returns>
        public async Task<Product> GetByIdAsync(int id)
        {
            return await _productRepository.GetByIdAsync(id);
        }

        /// <summary>
        ///  新增產品
        /// </summary>
        /// <param name="product">要新增的產品</param>
        /// <returns>已經新增的產品</returns>
        public async Task AddAsync(Product product)
        {
            await _productRepository.AddAsync(product);
        }

        /// <summary>
        ///  更新產品
        /// </summary>
        /// <param name="product">要更新的產品</param>
        /// <returns>已經更新的產品</returns>
        public async Task UpdateAsync(Product product)
        {
            await _productRepository.UpdateAsync(product);
        }

        /// <summary>
        ///  刪除產品
        /// </summary>
        /// <param name="id">要刪除的產品編號</param>
        /// <returns>已經刪除的產品</returns>
        public async Task DeleteAsync(int id)
        {
            await _productRepository.DeleteAsync(id);
        }
    }
}
