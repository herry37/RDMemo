namespace BackendManagement.Tests;

/// <summary>
/// 測試基礎類別
/// </summary>
public abstract class TestBase
{
    protected readonly Mock<ILogService> LogServiceMock;
    protected readonly Mock<ICacheService> CacheServiceMock;
    protected readonly Mock<IPublisher> PublisherMock;

    protected TestBase()
    {
        LogServiceMock = new Mock<ILogService>();
        CacheServiceMock = new Mock<ICacheService>();
        PublisherMock = new Mock<IPublisher>();
    }

    protected static DbContextOptions<ApplicationDbContext> CreateNewContextOptions()
    {
        return new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
    }
} 