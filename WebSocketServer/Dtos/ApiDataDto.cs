namespace WebSocketServer.Dtos
{
    /// <summary>
    /// API 資料傳輸物件
    /// </summary>
    public class ApiDataDto
    {
        /// <summary>
        /// 垃圾車車牌號碼
        /// </summary>
        public string? car { get; set; }
        /// <summary>
        /// 位置更新時間
        /// </summary>
        public string? time { get; set; }
        /// <summary>
        /// 位置描述
        /// </summary>
        public string? location { get; set; }
        /// <summary>
        /// 經度座標
        /// </summary>
        public string? x { get; set; }
        /// <summary>
        /// 緯度座標
        /// </summary>
        public string? y { get; set; }
    }
}
