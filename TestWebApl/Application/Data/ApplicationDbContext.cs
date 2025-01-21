using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace TestWebApl.Application.Data
{
    public class ApplicationDbContext : DbContext
    {
        /// <summary>
        /// 初始化ApplicationDbContext類別的新實例。
        /// </summary>
        /// <param name="options">ApplicationDbContext的DbContextOptions。</param>
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        /// <summary>
        /// 設定DbContext的模型。
        /// </summary>
        /// <param name="modelBuilder">DbContext的ModelBuilder實例。</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // 自動掃描所有實體類型並映射到對應的資料表
            var assembly = Assembly.GetExecutingAssembly();
            var entityTypes = assembly.GetTypes()
                .Where(t => t.GetCustomAttributes<TableAttribute>().Any()
                    || (t.IsClass && t.Namespace == "TestWebApl.Entitie"));

            foreach (var type in entityTypes)
            {
                modelBuilder.Entity(type).ToTable(type.Name);
            }
        }
    }
}
