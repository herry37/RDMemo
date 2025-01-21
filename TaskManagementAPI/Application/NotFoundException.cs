namespace TodoTaskManagementAPI.Application
{
    /// <summary>
    /// 當請求的資源不存在時拋出的異常
    /// </summary>
    public class NotFoundException : Exception
    {
        public NotFoundException(string message) : base(message)
        {
        }
    }
}
