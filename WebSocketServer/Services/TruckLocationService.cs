using Microsoft.Extensions.Caching.Memory;
using System.Text.Json;
using WebSocketServer.Dtos;
using WebSocketServer.Response;

namespace WebSocketServer.Services;

/// <summary>
///   垃圾車位置服務
/// </summary>
public interface ITruckLocationService
{
    /// <summary>
    ///   取得垃圾車位置
    /// </summary>
    /// <param name="cancellationToken">取消代碼</param>
    /// <returns>垃圾車位置</returns>
    Task<List<TruckLocationDto>> GetTruckLocationsAsync(CancellationToken cancellationToken);
}

/// <summary>
///   垃圾車位置服務
/// </summary>
public class TruckLocationService : ITruckLocationService
{
    /// <summary>       
    /// HttpClientFactory
    /// </summary>
    private readonly IHttpClientFactory _clientFactory;

    /// <summary>
    /// 日誌記錄器
    /// </summary>
    private readonly ILogger<TruckLocationService> _logger;
    /// <summary>
    /// 記憶體快取
    /// </summary>
    private readonly IMemoryCache _cache;

    private readonly IConfiguration _configuration;

    /// <summary>
    /// 基礎路徑
    /// </summary>
    private readonly string _baseUrl;

    /// <summary>
    /// 快取鍵
    /// </summary>
    private readonly string CacheKey = "TruckLocations";

    /// <summary>
    /// 快取選項
    /// </summary>
    private readonly MemoryCacheEntryOptions _cacheOptions = new MemoryCacheEntryOptions()
        .SetAbsoluteExpiration(TimeSpan.FromMinutes(5))
        .SetSlidingExpiration(TimeSpan.FromMinutes(2))
        .SetSize(1);

    /// <summary>
    ///   建構函數
    /// </summary>
    /// <param name="clientFactory">HttpClientFactory</param>
    /// <param name="logger">Logger</param>
    /// <param name="cache">MemoryCache</param>
    /// <param name="configuration">Configuration</param>
    public TruckLocationService(
        IHttpClientFactory clientFactory,
        IMemoryCache cache,
        ILogger<TruckLocationService> logger,
        IConfiguration configuration,
        IHostEnvironment env)
    {
        _clientFactory = clientFactory;
        _cache = cache;
        _logger = logger;
        _configuration = configuration;

        // 加強環境判斷邏輯
        if (env.IsProduction())
        {
            _baseUrl = _configuration["ApiSettings:ProductionBaseUrl"];
            _logger.LogInformation("正式環境使用 Production API: {ProductionUrl}", _baseUrl);
        }
        else
        {
            _baseUrl = _configuration["ApiSettings:LocalBaseUrl"];
            _logger.LogInformation("開發環境使用 Local API: {LocalUrl}", _baseUrl);
        }

        // 驗證 API URL 設定
        if (string.IsNullOrEmpty(_baseUrl))
        {
            throw new ArgumentNullException(nameof(_baseUrl), "API 基礎路徑未正確設定");
        }
    }

    /// <summary>
    ///   取得垃圾車位置
    /// </summary>
    /// <param name="cancellationToken">Token</param>
    /// <returns>垃圾車位置</returns>
    public async Task<List<TruckLocationDto>> GetTruckLocationsAsync(CancellationToken cancellationToken)
    {
        try
        {
            // 先嘗試從快取取得資料
            if (_cache.TryGetValue(CacheKey, out List<TruckLocationDto> cachedData))
            {
                _logger.LogInformation("使用快取資料");
                return cachedData;
            }

            var client = _clientFactory.CreateClient("TruckLocation");
            // 套用設定檔中的 timeout 設定
            var timeoutSeconds = _configuration.GetValue<int>("Performance:HttpClient:TimeoutSeconds");
            client.Timeout = TimeSpan.FromSeconds(timeoutSeconds);

            // 修改請求 URL，確保使用 HTTPS           
            var requestUrl = $"{_baseUrl}";
            _logger.LogInformation($"開始請求高雄市政府 API: {requestUrl}");
            using var request = new HttpRequestMessage(HttpMethod.Get, requestUrl);

            // 記錄請求標頭
            _logger.LogInformation($"請求標頭: {string.Join(", ", request.Headers.Select(h => $"{h.Key}={string.Join(";", h.Value)}"))}");

            using var response = await client.SendAsync(request, cancellationToken);

            _logger.LogInformation($"API 回應狀態: {response.StatusCode}");
            _logger.LogInformation($"API 回應狀態: {response.StatusCode}");

            // 讀取回應內容
            var content = await response.Content.ReadAsStringAsync(cancellationToken);

            // 記錄詳細的回應資訊
            _logger.LogInformation($"回應標頭: {string.Join(", ", response.Headers.Select(h => $"{h.Key}={string.Join(";", h.Value)}"))}");
            _logger.LogInformation($"回應內容: {content}");


            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError($"API 回應錯誤: {response.StatusCode}, URL: {requestUrl}, Content: {content}");
                return new List<TruckLocationDto>();
            }

            try
            {
                var apiResponse = JsonSerializer.Deserialize<ApiResponse>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });

                if (apiResponse?.data == null || !apiResponse.data.Any())
                {
                    _logger.LogWarning("API 回應資料為空，原始回應內容：{RawResponse}", content);
                    // 正式環境不記錄完整內容，只記錄摘要
                    if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Production")
                    {
                        _logger.LogError("正式環境 API 無效回應，狀態碼：{StatusCode}，內容長度：{ContentLength}",
                            response.StatusCode,
                            content?.Length ?? 0);
                    }
                    else
                    {
                        _logger.LogWarning("開發環境原始回應：{RawContent}", content);
                    }
                    return new List<TruckLocationDto>();
                }

                var locations = apiResponse.data
                    .Where(x => x != null)
                    .Select(x => new TruckLocationDto
                    {
                        car = x.car?.Trim() ?? "未知車號",
                        time = x.time?.Trim() ?? DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                        location = x.location?.Trim() ?? "位置更新中",
                        longitude = double.TryParse(x.x?.Trim(), out var lon) ? lon : 0,
                        latitude = double.TryParse(x.y?.Trim(), out var lat) ? lat : 0
                    })
                    .Where(x => x.latitude != 0 && x.longitude != 0)
                    .ToList();

                if (locations.Any())
                {
                    // 更新快取
                    _cache.Set(CacheKey, locations, _cacheOptions);
                    _logger.LogInformation($"成功取得 {locations.Count} 筆資料");
                    return locations;
                }

                _logger.LogWarning("沒有有效的位置資料");
                return new List<TruckLocationDto>();
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, $"JSON 解析錯誤: {content}");
                return new List<TruckLocationDto>();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "取得垃圾車位置時發生錯誤");
            return new List<TruckLocationDto>();
        }
    }
}