using WebSocketServer.Dtos;

namespace WebSocketServer.Response
{
    /// <summary>
    /// API 回應物件
    /// </summary>
    public class ApiResponse
    {
        /// <summary>
        /// API 回應List物件
        /// </summary>
        /// <value>API 回應List物件</value>
        public List<ApiDataDto> data { get; set; } = new List<ApiDataDto>();
    }
}
