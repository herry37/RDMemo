using BackendManagement.Domain.Interfaces;
using System.Text.Json;

namespace BackendManagement.Infrastructure.Services;

/// <summary>
/// Redis快取服務實作
/// </summary>
public class RedisCacheService : ICacheService
{
    private readonly IConnectionMultiplexer _redis;
    private readonly ILogger<RedisCacheService> _logger;
    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public RedisCacheService(
        IConnectionMultiplexer redis,
        ILogger<RedisCacheService> logger)
    {
        _redis = redis;
        _logger = logger;
    }

    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        try
        {
            var db = _redis.GetDatabase();
            var value = await db.StringGetAsync(key);

            if (!value.HasValue)
                return default;

            return System.Text.Json.JsonSerializer.Deserialize<T>(value!, _jsonOptions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "從快取讀取資料時發生錯誤: {Key}", key);
            return default;
        }
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? expiry = null, CancellationToken cancellationToken = default)
    {
        try
        {
            var db = _redis.GetDatabase();
            var serializedValue = System.Text.Json.JsonSerializer.Serialize(value, _jsonOptions);

            await db.StringSetAsync(
                key,
                serializedValue,
                expiry ?? TimeSpan.FromHours(1));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "設定Redis快取資料時發生錯誤: {key}", key);
        }
    }

    public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        try
        {
            var db = _redis.GetDatabase();
            await db.KeyDeleteAsync(key);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "移除Redis快取資料時發生錯誤: {key}", key);
        }
    }

    private string Serialize<T>(T obj)
    {
        return System.Text.Json.JsonSerializer.Serialize(obj, _jsonOptions);
    }

    private T? Deserialize<T>(string json)
    {
        return System.Text.Json.JsonSerializer.Deserialize<T>(json, _jsonOptions);
    }
} 