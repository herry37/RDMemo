using Microsoft.EntityFrameworkCore;
using Microsoft.Data.Sqlite;
using TodoTaskManagementAPI.Domain;
using System.IO;

namespace TodoTaskManagementAPI.Infrastructure
{
    /// <summary>
    /// 待辦事項任務數據庫上下文
    /// </summary>
    public class TodoTaskContext : DbContext
    {
        /// <summary>
        /// 任務DbSet
        /// </summary>
        public DbSet<TodoTask> TodoTasks { get; set; }

        /// <summary>
        /// 構造函數
        /// </summary>
        public TodoTaskContext(DbContextOptions<TodoTaskContext> options)
            : base(options)
        {
            // 確保數據庫目錄存在
            var dbPath = Path.Combine(Directory.GetCurrentDirectory(), "Data");
            if (!Directory.Exists(dbPath))
            {
                Directory.CreateDirectory(dbPath);
            }

            // 確保數據庫存在並已創建
            Database.EnsureCreated();
        }

        /// <summary>
        /// 配置模型
        /// </summary>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<TodoTask>(entity =>
            {
                entity.ToTable("Tasks");
                entity.HasKey(e => e.Id);
                
                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(200);
                
                entity.Property(e => e.Description)
                    .HasMaxLength(1000);

                // 明確配置 Priority 屬性的存儲方式
                entity.Property(e => e.Priority)
                    .HasConversion<int>()
                    .IsRequired();
                
                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("datetime('now')")
                    .IsRequired();
                
                entity.Property(e => e.UpdatedAt)
                    .HasDefaultValueSql("datetime('now')")
                    .IsRequired();
                
                entity.Property(e => e.IsCompleted)
                    .HasDefaultValue(false)
                    .IsRequired();
                
                entity.Property(e => e.DueDate)
                    .IsRequired(false);
            });
        }
    }
}
