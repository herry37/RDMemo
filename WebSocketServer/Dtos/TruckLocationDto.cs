namespace WebSocketServer.Dtos
{
    /// <summary>
    /// 垃圾車位置資料傳輸物件
    /// 用於在伺服器和客戶端之間傳輸垃圾車位置資訊
    /// </summary>
    public class TruckLocationDto
    {
        /// <summary>
        /// 垃圾車車牌號碼
        /// 例如：KCG-001
        /// </summary>
        public string car { get; set; }

        /// <summary>
        /// 位置更新時間
        /// 格式：yyyy-MM-dd HH:mm:ss
        /// </summary>
        public string time { get; set; }

        /// <summary>
        /// 位置描述
        /// 例如：高雄市前金區市中一路
        /// </summary>
        public string location { get; set; }

        /// <summary>
        /// 緯度座標
        /// WGS84 座標系統
        /// </summary>
        public double latitude { get; set; }

        /// <summary>
        /// 經度座標
        /// WGS84 座標系統
        /// </summary>
        public double longitude { get; set; }
    }
}
