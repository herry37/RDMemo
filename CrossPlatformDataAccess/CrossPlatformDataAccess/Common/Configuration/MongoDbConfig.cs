namespace CrossPlatformDataAccess.Common.Configuration
{
    /// <summary>
    /// MongoDB 配置
    /// </summary>
    public class MongoDbConfig
    {
        /// <summary>
        /// 伺服器位址
        /// </summary>
        public string Server { get; set; }

        /// <summary>
        /// 連接埠
        /// </summary>
        public int Port { get; set; } = 27017;

        /// <summary>
        /// 資料庫名稱
        /// </summary>
        public string Database { get; set; }

        /// <summary>
        /// 使用者名稱
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// 密碼
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// 是否使用 SSL
        /// </summary>
        public bool UseSsl { get; set; }

        /// <summary>
        /// 讀取偏好設定
        /// </summary>
        public string ReadPreference { get; set; }

        /// <summary>
        /// 取得連線字串
        /// </summary>
        public string GetConnectionString()
        {
            var credentials = string.IsNullOrEmpty(Username) ? "" : $"{Username}:{Password}@";
            var auth = !string.IsNullOrEmpty(credentials) ? $"authSource={Database}&" : "";
            var ssl = UseSsl ? "ssl=true&" : "";

            return $"mongodb://{credentials}{Server}:{Port}/{Database}?{auth}{ssl}" +
                   $"readPreference={ReadPreference}&";
        }

        /// <summary>
        /// 驗證配置是否有效
        /// </summary>
        public bool Validate()
        {
            return !string.IsNullOrEmpty(Server) &&
                   !string.IsNullOrEmpty(Database) &&
                   Port > 0 && Port < 65536;
        }
    }
}