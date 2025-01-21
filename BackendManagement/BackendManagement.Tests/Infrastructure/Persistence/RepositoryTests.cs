namespace BackendManagement.Tests.Infrastructure.Persistence;

public class RepositoryTests : TestBase, IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly RepositoryBase<TestEntity> _repository;

    public RepositoryTests()
    {
        _context = new ApplicationDbContext(CreateNewContextOptions(), LogServiceMock.Object);
        _repository = new TestRepository(_context, LogServiceMock.Object, CacheServiceMock.Object);
    }

    [Fact]
    public async Task GetByIdAsync_WhenEntityExists_ShouldReturnEntity()
    {
        // Arrange
        var entity = new TestEntity { Id = Guid.NewGuid() };
        await _context.Set<TestEntity>().AddAsync(entity);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByIdAsync(entity.Id);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(entity.Id);
    }

    [Fact]
    public async Task GetByIdAsync_WhenEntityDoesNotExist_ShouldThrowEntityNotFoundException()
    {
        // Arrange
        var id = Guid.NewGuid();

        // Act & Assert
        await Assert.ThrowsAsync<EntityNotFoundException>(
            () => _repository.GetByIdAsync(id));
    }

    [Fact]
    public async Task AddAsync_ShouldAddEntityAndReturnIt()
    {
        // Arrange
        var entity = new TestEntity { Id = Guid.NewGuid() };

        // Act
        var result = await _repository.AddAsync(entity);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(entity.Id);
        _context.Set<TestEntity>().Should().Contain(e => e.Id == entity.Id);
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateEntity()
    {
        // Arrange
        var entity = new TestEntity { Id = Guid.NewGuid() };
        await _context.Set<TestEntity>().AddAsync(entity);
        await _context.SaveChangesAsync();

        // Act
        entity.Name = "Updated";
        await _repository.UpdateAsync(entity);

        // Assert
        var updated = await _context.Set<TestEntity>().FindAsync(entity.Id);
        updated.Should().NotBeNull();
        updated!.Name.Should().Be("Updated");
    }

    [Fact]
    public async Task DeleteAsync_ShouldRemoveEntity()
    {
        // Arrange
        var entity = new TestEntity { Id = Guid.NewGuid() };
        await _context.Set<TestEntity>().AddAsync(entity);
        await _context.SaveChangesAsync();

        // Act
        await _repository.DeleteAsync(entity);

        // Assert
        var deleted = await _context.Set<TestEntity>().FindAsync(entity.Id);
        deleted.Should().BeNull();
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    private class TestEntity : EntityBase
    {
        public string Name { get; set; } = string.Empty;
    }

    private class TestRepository : RepositoryBase<TestEntity>
    {
        public TestRepository(
            ApplicationDbContext context,
            ILogService logService,
            ICacheService? cacheService = null)
            : base(context, logService, cacheService)
        {
        }
    }
} 