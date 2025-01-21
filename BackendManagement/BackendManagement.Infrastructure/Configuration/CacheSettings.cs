namespace BackendManagement.Infrastructure.Configuration;

public class CacheSettings
{
    public TimeSpan DefaultExpiration { get; set; } = TimeSpan.FromMinutes(30);
    public TimeSpan SlidingExpiration { get; set; } = TimeSpan.FromMinutes(5);
} 