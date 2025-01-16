namespace CrossPlatformDataAccess.Common.Configuration
{
    /// <summary>
    /// MongoDB 資料庫設定類別
    /// </summary>
    public class MongoDbConfig
    {
        /// <summary>
        /// 取得或設定 MongoDB 的連線字串
        /// </summary>
        public string ConnectionString { get; set; }

        /// <summary>
        /// 取得或設定 MongoDB 的資料庫名稱
        /// </summary>
        public string DatabaseName { get; set; }
    }
}