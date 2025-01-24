using TodoTaskManagementAPI.Domain;

namespace TodoTaskManagementAPI.Infrastructure
{
    /// <summary>
    /// 數據庫種子數據生成器
    /// 用於初始化數據庫時填充示例數據
    /// </summary>
    public static class DataSeeder
    {
        private static readonly string[] TaskTitles = new[]
        {
            "完成專案文檔",
            "修復Bug",
            "參加團隊會議",
            "更新依賴包",
            "代碼審查",
            "系統測試",
            "部署應用程序",
            "資料庫優化",
            "用戶界面設計",
            "性能調優"
        };

        private static readonly string[] TaskDescriptions = new[]
        {
            "撰寫詳細的技術文檔和使用說明",
            "修復用戶回報的系統問題",
            "與團隊討論工作進度和計劃",
            "更新專案依賴到最新版本",
            "審查團隊成員的代碼提交",
            "執行系統集成測試",
            "將新版本部署到生產環境",
            "優化資料庫查詢效能",
            "改進用戶界面和交互體驗",
            "提升系統整體運行效能"
        };

        /// <summary>
        /// 生成示例任務數據
        /// </summary>
        /// <returns>示例任務列表</returns>
        public static IEnumerable<TodoTask> GetSeedTasks()
        {
            var random = new Random(42); // 使用固定種子以確保生成的數據一致
            var tasks = new List<TodoTask>();

            for (int i = 1; i <= 100; i++)
            {
                var titleIndex = random.Next(TaskTitles.Length);
                var descIndex = random.Next(TaskDescriptions.Length);
                var daysToAdd = random.Next(1, 30); // 1-30天的隨機截止日期
                var priority = (Priority)random.Next(4); // 隨機優先級

                var task = TodoTask.CreateForSeed(
                    id: i,
                    title: $"{TaskTitles[titleIndex]} #{i}",
                    description: $"{TaskDescriptions[descIndex]} (Task #{i})",
                    dueDate: DateTime.UtcNow.AddDays(daysToAdd),
                    priority: priority
                );

                tasks.Add(task);
            }

            return tasks;
        }
    }
}
