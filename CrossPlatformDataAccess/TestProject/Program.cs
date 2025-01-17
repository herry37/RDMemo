using CrossPlatformDataAccess.Core.DataAccess;
using Microsoft.EntityFrameworkCore;
using System.Data.SqlClient;

namespace TestProject
{
    public class Program
    {
        static async Task Main(string[] args)
        {
            try
            {
                // 資料庫連線字串
                string connectionString = "Server=localhost;Database=TestDB;Trusted_Connection=True;";

                // 測試資料庫連線
                await TestDatabaseConnection(connectionString);

                // 測試資料存取
                await TestDataAccess(connectionString);

                Console.WriteLine("所有測試完成!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"錯誤: {ex.Message}");
                Console.WriteLine($"堆疊追蹤: {ex.StackTrace}");
            }

            Console.WriteLine("按任意鍵結束...");
            Console.ReadKey();
        }

        private static async Task TestDatabaseConnection(string connectionString)
        {
            Console.WriteLine("測試資料庫連線...");
            using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                Console.WriteLine("資料庫連線成功!");

                // 測試取得所有資料表
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = @"
                        SELECT TABLE_NAME 
                        FROM INFORMATION_SCHEMA.TABLES 
                        WHERE TABLE_TYPE = 'BASE TABLE'";

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        Console.WriteLine("\n資料庫中的資料表:");
                        while (await reader.ReadAsync())
                        {
                            Console.WriteLine($"- {reader.GetString(0)}");
                        }
                    }
                }
            }
        }

        private static async Task TestDataAccess(string connectionString)
        {
            Console.WriteLine("\n測試資料存取功能...");

            // 這裡需要替換成你的實際實作
            IDataAccessStrategy dataAccess = GetDataAccessStrategy(connectionString);

            // 測試查詢
            Console.WriteLine("\n測試查詢功能:");
            try
            {
                // 這裡替換成實際的資料表和實體類型
                var results = await dataAccess.Query<dynamic>("SELECT TOP 5 * FROM YourTable").ToListAsync();
                Console.WriteLine($"查詢成功，返回 {results.Count()} 筆資料");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"查詢失敗: {ex.Message}");
            }

            // 測試交易
            Console.WriteLine("\n測試交易功能:");
            try
            {
                await dataAccess.ExecuteInTransactionAsync(async () =>
                {
                    // 在這裡執行你的交易測試
                    await dataAccess.ExecuteAsync("/* 你的測試 SQL */");
                    return true;
                });
                Console.WriteLine("交易測試成功");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"交易測試失敗: {ex.Message}");
            }
        }

        private static IDataAccessStrategy GetDataAccessStrategy(string connectionString)
        {
            // 這裡需要替換成你的實際實作
            throw new NotImplementedException("請實作你的資料存取策略");
        }
    }

    // 測試用的實體類別
    public class TestEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime CreateTime { get; set; }
    }
}
