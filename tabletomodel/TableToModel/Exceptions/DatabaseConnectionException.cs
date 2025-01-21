namespace TableToModel
{
    /// <summary>
    /// 資料庫連線例外
    /// </summary>
    public class DatabaseConnectionException : Exception
    {
        public DatabaseConnectionException() { }

        public DatabaseConnectionException(string message)
            : base(message) { }

        public DatabaseConnectionException(string message, Exception inner)
            : base(message, inner) { }
    }
}
