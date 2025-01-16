namespace CrossPlatformDataAccess.Core.Interfaces
{
    /// <summary>
    /// 定義通用儲存庫的介面，提供對資料實體的基本 CRUD 操作和查詢功能
    /// </summary>
    /// <typeparam name="TEntity">實體類型，必須是參考類型</typeparam>
    public interface IRepository<TEntity> where TEntity : class
    {
        /// <summary>
        /// 定義通用儲存庫的介面，支援同步和非同步操作
        /// </summary>
        public interface IRepository<T> where T : class
        {

            /// <summary>
            /// 獲取所有記錄
            /// </summary>
            /// <returns>所有記錄的集合</returns>
            Task<IEnumerable<T>> GetAllAsync();

            /// <summary>
            /// 根據 ID 獲取單一記錄
            /// </summary>
            /// <param name="id">記錄的唯一標識</param>
            /// <returns>單一記錄</returns>
            Task<T> GetByIdAsync(int id);

            /// <summary>
            /// 新增記錄
            /// </summary>
            /// <param name="entity">要新增的記錄</param>
            /// <returns>新增的記錄</returns>
            Task<T> AddAsync(T entity);

            /// <summary>
            /// 更新記錄
            /// </summary>
            /// <param name="entity">要更新的記錄</param>
            /// <returns>更新後的記錄</returns>
            Task<T> UpdateAsync(T entity);

            /// <summary>
            /// 刪除記錄
            /// </summary>
            /// <param name="id">要刪除的記錄的唯一標識</param>
            /// <returns>刪除的結果</returns>
            Task<bool> DeleteAsync(int id);
        }

        #region
        ///// <summary>
        ///// 非同步依據指定的 ID 取得單一實體
        ///// </summary>
        ///// <param name="id">實體的唯一識別碼</param>
        ///// <returns>符合指定 ID 的實體，如果找不到則返回 null</returns>
        //Task<TEntity> GetByIdAsync(object id);

        ///// <summary>
        ///// 非同步取得資料表中的所有實體
        ///// </summary>
        ///// <returns>包含所有實體的集合</returns>
        //Task<IEnumerable<TEntity>> GetAllAsync();

        ///// <summary>
        ///// 非同步新增實體到資料表中
        ///// </summary>
        ///// <param name="entity">要新增的實體物件</param>
        ///// <returns>表示非同步操作的工作</returns>
        //Task AddAsync(TEntity entity);

        ///// <summary>
        ///// 更新資料表中的實體
        ///// </summary>
        ///// <param name="entity">包含更新內容的實體物件</param>
        //void Update(TEntity entity);

        ///// <summary>
        ///// 從資料表中刪除指定的實體
        ///// </summary>
        ///// <param name="entity">要刪除的實體物件</param>
        //void Delete(TEntity entity);

        ///// <summary>
        ///// 使用 LINQ 運算式建立自訂查詢
        ///// </summary>
        ///// <param name="filter">用於過濾實體的 Lambda 運算式，若為 null 則返回所有實體</param>
        ///// <returns>可進一步查詢的 IQueryable 物件</returns>
        //IQueryable<TEntity> Query(Expression<Func<TEntity, bool>> filter = null);

        ///// <summary>
        ///// 執行原生 SQL 查詢語句
        ///// </summary>
        ///// <param name="sql">要執行的 SQL 查詢字串</param>
        ///// <param name="parameters">SQL 查詢的參數陣列</param>
        ///// <returns>查詢結果的實體清單</returns>
        //Task<List<TEntity>> ExecuteQueryAsync(string sql, params object[] parameters);
        #endregion

    }
}
