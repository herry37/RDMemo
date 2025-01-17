using CrossPlatformDataAccess.Common.Configuration;
using CrossPlatformDataAccess.Core.Interfaces;
using CrossPlatformDataAccess.Infrastructure.Context;
using CrossPlatformDataAccess.Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace CrossPlatformDataAccess.Infrastructure.Extensions
{
    /// <summary>
    ///   依賴注入容器中註冊資料存取相關的服務
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        ///   依賴注入容器中註冊資料存取相關的服務
        /// </summary>
        /// <param name="services">IServiceCollection  服務集合實例</param>
        /// <param name="dbConfig">關聯式資料庫的設定物件</param>
        /// <param name="mongoConfig">MongoDB 的設定物件，  null</param>
        /// <returns>  依賴注入容器</returns>
        /// <remarks>
        ///   以下操作：
        /// 1.   services   dbConfig   null
        /// 2.   關聯式資料庫的相關服務（DbContext、UnitOfWork、Repository）
        /// 3.   MongoDB   null   關聯式資料庫的相關服務
        /// </remarks>
        /// <exception cref="ArgumentNullException">services   dbConfig   null   </exception>
        public static IServiceCollection AddDataAccess(
            this IServiceCollection services,
            DatabaseConfig dbConfig,
            MongoDbConfig mongoConfig = null!)
        {
            //   services   null           
            ArgumentNullException.ThrowIfNull(services);

            //   dbConfig   null            
            ArgumentNullException.ThrowIfNull(dbConfig);

            //   關聯式資料庫的相關服務
            services.AddScoped<IDbContext>(provider => new DatabaseContext(dbConfig));
            services.AddScoped<IUnitOfWork, UnitOfWork.UnitOfWork>();
            services.AddScoped(typeof(IRepository<>), typeof(GenericRepository<>));

            //   MongoDB   null   關聯式資料庫的相關服務
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