using Microsoft.EntityFrameworkCore;
using TestWebApl.Application.Data;
using TestWebApl.Entitie;
using TestWebApl.Interfaces;

namespace TestWebApl.Application.Repository
{
   public class ProductRepository : IProductRepository
{
    /// <summary>
    ///  產品資料庫存取
    /// </summary>
    private readonly ApplicationDbContext _context;
    /// <summary>
    ///  產品資料集
    /// </summary>
    private readonly DbSet<Product> _products;

    /// <summary>
    ///  建構函數
    /// </summary>
    /// <param name="context">資料庫內容</param>
    public ProductRepository(ApplicationDbContext context)
    {
        _context = context;
        _products = _context.Set<Product>();
    }

    /// <summary>
    ///  取得所有產品
    /// </summary>
    /// <returns>所有產品</returns>
    public async Task<IEnumerable<Product>> GetAllAsync()
    {
        return await _products.ToListAsync();
    }

    /// <summary>
    ///  取得單一產品
    /// </summary>
    /// <param name="id">產品編號</param>
    /// <returns>單一產品</returns>
    public async Task<Product> GetByIdAsync(int id)
    {
        return await _products.FindAsync(id);
    }

    /// <summary>
    ///  新增產品
    /// </summary>
    /// <param name="product">要新增的產品</param>
    /// <returns>已經新增的產品</returns>
    public async Task AddAsync(Product product)
    {
        await _products.AddAsync(product);
        await _context.SaveChangesAsync();
    }

    /// <summary>
    ///  更新產品
    /// </summary>
    /// <param name="product">要更新的產品</param>
    /// <returns>已經更新的產品</returns>
    public async Task UpdateAsync(Product product)
    {
        _context.Entry(product).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }

    /// <summary>
    ///  刪除產品
    /// </summary>
    /// <param name="id">要刪除的產品編號</param>
    /// <returns>已經刪除的產品</returns>
    public async Task DeleteAsync(int id)
    {
        var product = await _products.FindAsync(id);
        if (product != null)
        {
            _products.Remove(product);
            await _context.SaveChangesAsync();
        }
    }
}
}
