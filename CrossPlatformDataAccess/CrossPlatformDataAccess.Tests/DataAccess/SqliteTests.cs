using CrossPlatformDataAccess.Core.DataAccess;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace CrossPlatformDataAccess.Tests.DataAccess
{
    public class TestEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class TestDbContext : DbContext
    {
        public TestDbContext(DbContextOptions<TestDbContext> options)
            : base(options)
        {
        }

        public DbSet<TestEntity> TestEntities { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TestEntity>().HasKey(e => e.Id);
        }
    }

    public class SqliteTests : IDisposable
    {
        private readonly SqliteConnection _connection;
        private readonly TestDbContext _context;
        private readonly IDataAccessStrategy _dataAccess;

        public SqliteTests()
        {
            // 建立 SQLite 記憶體資料庫
            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open();

            // 設定 DbContext
            var options = new DbContextOptionsBuilder<TestDbContext>()
                .UseSqlite(_connection)
                .Options;

            _context = new TestDbContext(options);
            _context.Database.EnsureCreated();

            // 初始化測試資料
            _context.TestEntities.AddRange(
                new TestEntity { Id = 1, Name = "Test1" },
                new TestEntity { Id = 2, Name = "Test2" },
                new TestEntity { Id = 3, Name = "Test3" }
            );
            _context.SaveChanges();

            // 初始化資料存取策略
            // _dataAccess = new YourDataAccessStrategyImplementation(_context);
        }

        [Fact]
        public async Task GetById_ShouldReturnCorrectEntity()
        {
            // Arrange
            var id = 1;

            // Act
            var entity = await _dataAccess.GetByIdAsync<TestEntity>(id);

            // Assert
            Assert.NotNull(entity);
            Assert.Equal(id, entity.Id);
            Assert.Equal("Test1", entity.Name);
        }

        [Fact]
        public async Task Query_WithCondition_ShouldReturnFilteredResults()
        {
            // Arrange
            var expectedName = "Test2";

            // Act
            var results = await _dataAccess.Query<TestEntity>()
                .Where(e => e.Name == expectedName)
                .ToListAsync();

            // Assert
            Assert.Single(results);
            Assert.Equal(expectedName, results.First().Name);
        }

        [Fact]
        public async Task Add_ShouldInsertNewEntity()
        {
            // Arrange
            var newEntity = new TestEntity { Id = 4, Name = "Test4" };

            // Act
            var result = await _dataAccess.AddAsync(newEntity);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(4, result.Id);
            Assert.Equal("Test4", result.Name);

            // Verify in database
            var savedEntity = await _context.TestEntities.FindAsync(4);
            Assert.NotNull(savedEntity);
            Assert.Equal("Test4", savedEntity.Name);
        }

        [Fact]
        public async Task Update_ShouldModifyExistingEntity()
        {
            // Arrange
            var entity = await _context.TestEntities.FindAsync(1);
            entity.Name = "Updated";

            // Act
            var result = await _dataAccess.UpdateAsync(entity);

            // Assert
            Assert.True(result);

            // Verify in database
            var updatedEntity = await _context.TestEntities.FindAsync(1);
            Assert.Equal("Updated", updatedEntity.Name);
        }

        [Fact]
        public async Task Delete_ShouldRemoveEntity()
        {
            // Arrange
            var entity = await _context.TestEntities.FindAsync(1);

            // Act
            var result = await _dataAccess.DeleteAsync(entity);

            // Assert
            Assert.True(result);

            // Verify in database
            var deletedEntity = await _context.TestEntities.FindAsync(1);
            Assert.Null(deletedEntity);
        }

        [Fact]
        public async Task ExecuteTransaction_ShouldCommitAllChanges()
        {
            // Arrange
            var newEntity = new TestEntity { Id = 4, Name = "Test4" };

            // Act
            var result = await _dataAccess.ExecuteInTransactionAsync(async () =>
            {
                var added = await _dataAccess.AddAsync(newEntity);
                var existing = await _dataAccess.GetByIdAsync<TestEntity>(1);
                existing.Name = "UpdatedInTransaction";
                await _dataAccess.UpdateAsync(existing);
                return true;
            });

            // Assert
            Assert.True(result);

            // Verify in database
            var addedEntity = await _context.TestEntities.FindAsync(4);
            Assert.NotNull(addedEntity);
            Assert.Equal("Test4", addedEntity.Name);

            var updatedEntity = await _context.TestEntities.FindAsync(1);
            Assert.Equal("UpdatedInTransaction", updatedEntity.Name);
        }

        public void Dispose()
        {
            _context.Dispose();
            _connection.Dispose();
        }
    }
}
