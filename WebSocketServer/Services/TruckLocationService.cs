using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using WebSocketServer.Dtos;

namespace WebSocketServer.Services;

/// <summary>
/// 垃圾車位置服務介面
/// 定義了與垃圾車位置相關的核心功能
/// </summary>
public interface ITruckLocationService
{
    /// <summary>
    /// 非同步傳送垃圾車位置資訊
    /// </summary>
    /// <param name="webSocket">用於傳送資料的 WebSocket 連線</param>
    /// <param name="cancellationToken">取消權杖，用於控制非同步操作的取消</param>
    /// <returns>非同步工作</returns>
    Task SendTruckLocationsAsync(WebSocket webSocket, CancellationToken cancellationToken = default);
}

/// <summary>
/// 垃圾車位置服務實作
/// 負責從高雄市政府 API 獲取垃圾車位置資訊並透過 WebSocket 傳送給客戶端
/// </summary>
public class TruckLocationService : ITruckLocationService
{
    /// <summary>
    /// HttpClient 用於發送 HTTP 請求
    /// </summary>
    private readonly HttpClient _httpClient;
    /// <summary>
    /// 日誌記錄器
    /// </summary>
    private readonly ILogger<TruckLocationService> _logger;
    /// <summary>
    /// 高雄市政府清潔車 API 的 URL
    /// </summary>
    private const string ApiUrl = "https://api.kcg.gov.tw/api/service/Get/aaf4ce4b-4ca8-43de-bfaf-6dc97e89cac0";

    /// <summary>
    /// 初始化垃圾車位置服務
    /// </summary>
    /// <param name="httpClient"></param>
    /// <param name="logger"></param>
    public TruckLocationService(HttpClient httpClient, ILogger<TruckLocationService> logger)
    {
        _httpClient = httpClient;
        // 設定 HttpClient 的預設標頭
        _httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/121.0.0.0 Safari/537.36");
        // 設定 Accept 標頭，指定接受的內容類型
        _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
        // 設定 Referer 標頭，指定請求的來源網址
        _httpClient.DefaultRequestHeaders.Add("Referer", "https://api.kcg.gov.tw/");
        _logger = logger;
    }

    /// <summary>
    /// 非同步傳送垃圾車位置資訊
    /// </summary>
    /// <param name="webSocket">用於傳送資料的 WebSocket 連線</param>
    /// <param name="cancellationToken">取消權杖，用於控制非同步操作的取消</param> 
    /// <returns></returns>
    public async Task SendTruckLocationsAsync(WebSocket webSocket, CancellationToken cancellationToken = default)
    {
        // 持續傳送垃圾車位置資訊，直到 WebSocket 連線關閉或取消操作
        while (webSocket.State == WebSocketState.Open && !cancellationToken.IsCancellationRequested)
        {
            try
            {
                // 構建帶有時間戳的 URL，避免快取
                var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                // 構建 API 請求 URL
                var requestUrl = $"{ApiUrl}?format=json&t={timestamp}";
                //呼叫 API 取得垃圾車位置資訊
                var response = await _httpClient.GetAsync(requestUrl, cancellationToken);
                if (response.IsSuccessStatusCode)
                {
                    // 將 API 回應轉換為字串
                    var json = await response.Content.ReadAsStringAsync(cancellationToken);

                    try
                    {
                        // 解析 API 回應
                        var document = JsonDocument.Parse(json);
                        // 取得 data 屬性的值
                        var data = document.RootElement.GetProperty("data");
                        // 建立垃圾車位置資料列表
                        var locations = new List<TruckLocationDto>();

                        // 遍歷 data 屬性的陣列
                        foreach (var item in data.EnumerateArray())
                        {
                            try
                            {
                                var truckLocation = new TruckLocationDto
                                {
                                    car = item.GetProperty("car").GetString(),//取得車牌號碼
                                    time = item.GetProperty("time").GetString(),//取得時間
                                    location = item.GetProperty("location").GetString(),//取得位置
                                    longitude = double.Parse(item.GetProperty("x").GetString() ?? "0"),//取得經度
                                    latitude = double.Parse(item.GetProperty("y").GetString() ?? "0")//取得緯度
                                };
                                locations.Add(truckLocation);
                            }
                            catch (Exception ex)
                            {
                                _logger.LogWarning(ex, "解析垃圾車資料時發生錯誤");
                                continue; // 跳過這筆資料，繼續處理下一筆
                            }
                        }

                        if (locations.Count > 0)
                        {
                            // 將垃圾車位置資料序列化為 JSON 字串
                            var message = JsonSerializer.Serialize(locations);
                            // 將 JSON 字串轉換為位元組陣列
                            var messageBytes = Encoding.UTF8.GetBytes(message);
                            // 傳送垃圾車位置資料
                            await webSocket.SendAsync(
                                new ArraySegment<byte>(messageBytes),
                                WebSocketMessageType.Text,
                                true,
                                cancellationToken
                            );
                        }
                        else
                        {
                            _logger.LogWarning("未找到任何有效的垃圾車位置資料");
                            var errorMessage = JsonSerializer.Serialize(new { error = "目前沒有垃圾車位置資料" });
                            var errorBytes = Encoding.UTF8.GetBytes(errorMessage);
                            await webSocket.SendAsync(
                                new ArraySegment<byte>(errorBytes),
                                WebSocketMessageType.Text,
                                true,
                                CancellationToken.None
                            );
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "解析 API 回應時發生錯誤");
                        var errorMessage = JsonSerializer.Serialize(new { error = "資料格式錯誤" });
                        var errorBytes = Encoding.UTF8.GetBytes(errorMessage);
                        await webSocket.SendAsync(
                            new ArraySegment<byte>(errorBytes),
                            WebSocketMessageType.Text,
                            true,
                            CancellationToken.None
                        );
                    }
                }
                else
                {
                    _logger.LogWarning($"API 請求失敗，狀態碼：{response.StatusCode}，URL：{requestUrl}");
                    var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
                    _logger.LogWarning($"錯誤回應：{errorContent}");

                    var errorMessage = JsonSerializer.Serialize(new { error = $"無法取得垃圾車位置資料，請稍後再試" });

                    // 將錯誤訊息轉換為位元組陣列
                    var errorBytes = Encoding.UTF8.GetBytes(errorMessage);

                    // 傳送錯誤訊息
                    await webSocket.SendAsync(
                        new ArraySegment<byte>(errorBytes),
                        WebSocketMessageType.Text,
                        true,
                        CancellationToken.None
                    );
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "傳送垃圾車位置時發生錯誤");
                if (webSocket.State == WebSocketState.Open)
                {
                    var errorMessage = JsonSerializer.Serialize(new { error = "系統發生錯誤，請稍後再試" });
                    var errorBytes = Encoding.UTF8.GetBytes(errorMessage);
                    await webSocket.SendAsync(
                        new ArraySegment<byte>(errorBytes),
                        WebSocketMessageType.Text,
                        true,
                        CancellationToken.None
                    );
                }
            }
            // 等待 5 秒再繼續下一次傳送
            await Task.Delay(5000, cancellationToken);
        }
    }
}
