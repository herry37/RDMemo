using System.Linq.Expressions;

namespace CrossPlatformDataAccess.Core.Interfaces
{
    /// <summary>
    /// 定義 MongoDB 儲存庫的通用介面，提供基本的 CRUD 操作和查詢功能
    /// </summary>
    /// <typeparam name="TDocument">文件類型，必須是參考類型</typeparam>
    public interface IMongoRepository<TDocument> where TDocument : class
    {
        /// <summary>
        /// 非同步取得集合中的所有文件
        /// </summary>
        /// <returns>包含所有文件的集合</returns>
        Task<IEnumerable<TDocument>> GetAllAsync();

        /// <summary>
        /// 非同步依據指定的 ID 取得單一文件
        /// </summary>
        /// <param name="id">文件的唯一識別碼</param>
        /// <returns>符合指定 ID 的文件，如果找不到則返回 null</returns>
        Task<TDocument> GetByIdAsync(string id);

        /// <summary>
        /// 非同步新增文件到集合中
        /// </summary>
        /// <param name="document">要新增的文件</param>
        /// <returns>表示非同步操作的工作</returns>
        Task CreateAsync(TDocument document);

        /// <summary>
        /// 非同步更新指定 ID 的文件
        /// </summary>
        /// <param name="id">要更新文件的唯一識別碼</param>
        /// <param name="document">包含更新內容的文件</param>
        /// <returns>表示非同步操作的工作</returns>
        Task UpdateAsync(string id, TDocument document);

        /// <summary>
        /// 非同步刪除指定 ID 的文件
        /// </summary>
        /// <param name="id">要刪除文件的唯一識別碼</param>
        /// <returns>表示非同步操作的工作</returns>
        Task DeleteAsync(string id);

        /// <summary>
        /// 非同步依據指定的條件運算式查詢文件
        /// </summary>
        /// <param name="filterExpression">用於過濾文件的 Lambda 運算式</param>
        /// <returns>符合條件的文件集合</returns>
        Task<IEnumerable<TDocument>> FindAsync(Expression<Func<TDocument, bool>> filterExpression);
    }
}
