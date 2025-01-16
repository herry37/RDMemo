using CrossPlatformDataAccess.Common.Configuration;
using CrossPlatformDataAccess.Core.Interfaces;
using CrossPlatformDataAccess.Infrastructure.Context;
using CrossPlatformDataAccess.Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace CrossPlatformDataAccess.Infrastructure.Extensions
{
    /// <summary>
    /// 提供擴充方法用於註冊資料存取相關的服務
    /// </summary>
    /// <remarks>
    /// 此類別包含擴充方法，用於設定和註冊資料庫相關的依賴注入服務，
    /// 支援同時註冊關聯式資料庫和 MongoDB 的服務
    /// </remarks>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// 註冊資料存取相關的服務到依賴注入容器中
        /// </summary>
        /// <param name="services">IServiceCollection 服務集合實例</param>
        /// <param name="dbConfig">關聯式資料庫的設定物件</param>
        /// <param name="mongoConfig">MongoDB 的設定物件，可為 null</param>
        /// <returns>設定完成的服務集合</returns>
        /// <remarks>
        /// 此方法會執行以下操作：
        /// 1. 驗證必要的參數是否為 null
        /// 2. 註冊關聯式資料庫的相關服務（DbContext、UnitOfWork、Repository）
        /// 3. 如果提供了 MongoDB 設定，則註冊 MongoDB 相關服務
        /// </remarks>
        /// <exception cref="ArgumentNullException">當 services 或 dbConfig 為 null 時拋出</exception>
        public static IServiceCollection AddDataAccess(
            this IServiceCollection services,
            DatabaseConfig dbConfig,
            MongoDbConfig mongoConfig = null)
        {
            // 驗證服務集合參數是否為空
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            // 驗證資料庫設定參數是否為空
            if (dbConfig == null)
                throw new ArgumentNullException(nameof(dbConfig));

            // 註冊關聯式資料庫的相關服務
            // 使用 Scoped 生命週期確保每個請求使用相同的資料庫上下文
            services.AddScoped<IDbContext>(provider => new DatabaseContext(dbConfig));
            services.AddScoped<IUnitOfWork, UnitOfWork.UnitOfWork>();
            services.AddScoped(typeof(IRepository<>), typeof(GenericRepository<>));

            // 如果有提供 MongoDB 設定，則註冊 MongoDB 相關服務
            // MongoDB 設定使用 Singleton 生命週期，確保配置只被建立一次
            if (mongoConfig != null)
            {
                services.AddSingleton(mongoConfig);
                services.AddScoped<IMongoDbContext, MongoDbContext>();
                services.AddScoped(typeof(IMongoRepository<>), typeof(MongoRepository<>));
            }

            return services;
        }
    }
}