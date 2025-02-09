using System.Text.Json;
using WebSocketServer.Dtos;
using WebSocketServer.Response;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Hosting;

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
    private readonly IHttpClientFactory _clientFactory;
    private readonly ILogger<TruckLocationService> _logger;
    private readonly IMemoryCache _cache;
    private readonly string _baseUrl;
    private const string CacheKey = "TruckLocations";
    private readonly IWebHostEnvironment _environment;

    /// <summary>
    ///   建構函數
    /// </summary>
    /// <param name="clientFactory">HttpClientFactory</param>
    /// <param name="logger">Logger</param>
    /// <param name="cache">MemoryCache</param>
    /// <param name="configuration">Configuration</param>
    /// <param name="environment">Web Host Environment</param>
    public TruckLocationService(
        IHttpClientFactory clientFactory,
        ILogger<TruckLocationService> logger,
        IMemoryCache cache,
        IConfiguration configuration,
        IWebHostEnvironment environment)
    {
        _clientFactory = clientFactory;
        _logger = logger;
        _cache = cache;
        _environment = environment;
        
        // 根據環境選擇正確的 API 基礎路徑
        _baseUrl = _environment.IsDevelopment()
            ? configuration.GetValue<string>("ApiSettings:LocalBaseUrl")
            : configuration.GetValue<string>("ApiSettings:ProductionBaseUrl");

        if (string.IsNullOrEmpty(_baseUrl))
        {
            throw new ArgumentNullException("ApiSettings:BaseUrl", "API BaseUrl is not configured");
        }
    }

    /// <summary>
    ///   取得垃圾車位置
    /// </summary>
    /// <param name="cancellationToken">取消代碼</param>
    /// <returns>垃圾車位置</returns>
    public async Task<List<TruckLocationDto>> GetTruckLocationsAsync(CancellationToken cancellationToken)
    {
        try
        {
            // 嘗試從快取中獲取數據
            if (_cache.TryGetValue(CacheKey, out List<TruckLocationDto>? cachedLocations) && cachedLocations != null)
            {
                _logger.LogInformation("從快取中獲取垃圾車位置資料");
                return cachedLocations;
            }

            using var client = _clientFactory.CreateClient("TruckLocation");
            var timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            var requestUrl = $"{_baseUrl}?$format=json&_={timestamp}";
            
            using var response = await client.GetAsync(requestUrl, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
            
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("API 請求失敗: {StatusCode} {ReasonPhrase}", response.StatusCode, response.ReasonPhrase);
                return new List<TruckLocationDto>();
            }

            var jsonContent = await response.Content.ReadAsStringAsync(cancellationToken);
            
            if (string.IsNullOrEmpty(jsonContent))
            {
                _logger.LogWarning("API 回應內容為空");
                return new List<TruckLocationDto>();
            }

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var apiResponse = JsonSerializer.Deserialize<ApiResponse>(jsonContent, options);
            
            if (apiResponse?.data == null)
            {
                _logger.LogWarning("API 回應資料為空");
                return new List<TruckLocationDto>();
            }

            var result = apiResponse.data
                .Where(item => !string.IsNullOrEmpty(item?.car) && 
                             !string.IsNullOrEmpty(item?.location) &&
                             !string.IsNullOrEmpty(item?.x) &&
                             !string.IsNullOrEmpty(item?.y) &&
                             double.TryParse(item.x, out _) && 
                             double.TryParse(item.y, out _))
                .Select(item => new TruckLocationDto
                {
                    car = item.car!,
                    time = item.time,
                    location = item.location!,
                    longitude = double.Parse(item.x!),
                    latitude = double.Parse(item.y!)
                })
                .ToList();

            // 將結果存入快取 30 秒
            _cache.Set(CacheKey, result, TimeSpan.FromSeconds(30));

            _logger.LogInformation("成功獲取 {Count} 筆垃圾車位置資料", result.Count);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "獲取垃圾車位置時發生錯誤: {Message}", ex.Message);
            return new List<TruckLocationDto>();
        }
    }
}