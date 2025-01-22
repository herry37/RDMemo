namespace TableToModel
{
    /// <summary>
    /// 找不到資料表例外
    /// </summary>
    public class TableNotFoundException : Exception
    {
        public TableNotFoundException(string message) : base(message) { }
    }
}
