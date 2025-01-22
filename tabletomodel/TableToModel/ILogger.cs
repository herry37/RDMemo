namespace TableToModel
{
    /// <summary>
    /// 日誌記錄介面
    /// </summary>
    public interface ILogger
    {
        /// <summary>
        /// 記錄資訊
        /// </summary>
        void LogInformation(string message);

        /// <summary>
        /// 記錄錯誤
        /// </summary>
        void LogError(string message);

        /// <summary>
        /// 記錄警告
        /// </summary>
        void LogWarning(string message);
    }
}
