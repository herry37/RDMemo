/// <summary>
/// Entity Framework Core 數據訪問層
/// 包含數據庫上下文和實體配置
/// </summary>

using Microsoft.EntityFrameworkCore;
using Microsoft.Data.Sqlite;
using TodoTaskManagementAPI.Domain;
using System.IO;

namespace TodoTaskManagementAPI.Infrastructure
{
    /// <summary>
    /// 待辦事項任務數據庫上下文
    /// 負責與SQLite數據庫的交互和實體映射配置
    /// </summary>
    /// <remarks>
    /// 此類別提供：
    /// 1. 數據庫連接管理
    /// 2. 實體與數據表的映射配置
    /// 3. 數據庫初始化和遷移支持
    /// 4. 實體關係和約束的定義
    /// 
    /// 使用說明：
    /// - 通過依賴注入獲取實例
    /// - 自動創建數據庫（如果不存在）
    /// - 支持事務管理
    /// - 實現數據庫版本控制
    /// </remarks>
    public class TodoTaskContext : DbContext
    {
        /// <summary>
        /// 任務數據集
        /// 提供對Tasks表的訪問和操作
        /// </summary>
        /// <remarks>
        /// - 支持LINQ查詢
        /// - 提供變更追蹤
        /// - 處理實體狀態管理
        /// - 實現延遲加載
        /// </remarks>
        public DbSet<TodoTask> TodoTasks { get; set; }

        /// <summary>
        /// 數據庫上下文構造函數
        /// </summary>
        /// <param name="options">數據庫配置選項，通過依賴注入提供</param>
        /// <remarks>
        /// 初始化過程：
        /// 1. 調用基類構造函數設置配置選項
        /// 2. 確保數據庫存在
        /// 3. 自動執行待處理的遷移
        /// 
        /// 注意事項：
        /// - 應通過依賴注入容器創建實例
        /// - 不要直接在應用代碼中調用
        /// - 確保數據庫連接字符串配置正確
        /// </remarks>
        public TodoTaskContext(DbContextOptions<TodoTaskContext> options)
            : base(options)
        {
            var dbExists = Database.GetAppliedMigrations().Any() || Database.CanConnect();
            
            // 如果資料庫不存在，則創建並填充種子資料
            if (!dbExists)
            {
                Database.EnsureCreated();
                
                // TODO: 這段代碼僅用於開發環境，在正式環境中應該註釋掉
                // 如果數據庫為空，則添加種子數據
                if (!TodoTasks.Any())
                {
                    TodoTasks.AddRange(DataSeeder.GetSeedTasks());
                    SaveChanges();
                }
            }
        }

        /// <summary>
        /// 配置實體模型和數據庫映射關係
        /// </summary>
        /// <param name="modelBuilder">用於配置實體和關係的模型構建器</param>
        /// <remarks>
        /// 配置內容：
        /// 1. 表名和主鍵設置
        /// 2. 屬性約束和驗證規則
        /// 3. 索引和外鍵關係
        /// 4. 默認值和計算列
        /// 
        /// 具體配置：
        /// - 表名：Tasks
        /// - 主鍵：Id（自增）
        /// - 標題：必填，最大長度200
        /// - 描述：可選，最大長度1000
        /// - 優先級：存儲為整數
        /// - 時間戳：自動設置當前時間
        /// - 完成狀態：默認為false
        /// </remarks>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<TodoTask>(entity =>
            {
                // 配置表名和主鍵
                entity.ToTable("Tasks");
                entity.HasKey(e => e.Id);
                
                // 配置標題屬性
                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(200);
                
                // 配置描述屬性
                entity.Property(e => e.Description)
                    .HasMaxLength(1000);

                // 配置優先級屬性
                entity.Property(e => e.Priority)
                    .HasConversion<int>()
                    .IsRequired();
                
                // 配置創建時間
                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("datetime('now')")
                    .IsRequired();
                
                // 配置更新時間
                entity.Property(e => e.UpdatedAt)
                    .HasDefaultValueSql("datetime('now')")
                    .IsRequired();
                
                // 配置完成狀態
                entity.Property(e => e.IsCompleted)
                    .HasDefaultValue(false)
                    .IsRequired();
                
                // 配置截止日期
                entity.Property(e => e.DueDate)
                    .IsRequired(false);
            });
        }
    }
}
