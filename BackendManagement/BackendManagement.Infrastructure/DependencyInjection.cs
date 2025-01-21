using System.Reflection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using BackendManagement.Domain.Interfaces;
using BackendManagement.Infrastructure.Services;
using BackendManagement.Infrastructure.Persistence;
using BackendManagement.Infrastructure.Performance;
using StackExchange.Redis;
using BackendManagement.Application.Common.Interfaces;
using BackendManagement.Infrastructure.Persistence.Interceptors;
using Microsoft.AspNetCore.Http;

namespace BackendManagement.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddHttpContextAccessor();
        
        services.AddScoped<AuditableEntitySaveChangesInterceptor>();
        services.AddScoped<IDomainEventService, DomainEventService>();
        services.AddScoped<Application.Common.Interfaces.IDateTime, DateTimeService>();
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddScoped<ICurrentTenantService, CurrentTenantService>();

        var connectionString = configuration.GetConnectionString("DefaultConnection");
        services.AddDbContext<ApplicationDbContext>((sp, options) =>
        {
            var interceptor = sp.GetRequiredService<AuditableEntitySaveChangesInterceptor>();
            var dateTime = sp.GetRequiredService<Application.Common.Interfaces.IDateTime>();
            var currentUserService = sp.GetRequiredService<ICurrentUserService>();

            options.UseMySql(
                connectionString!,
                ServerVersion.AutoDetect(connectionString),
                x => x.EnableRetryOnFailure()
            );

            options.AddInterceptors(new AuditableEntitySaveChangesInterceptor(currentUserService, dateTime));
        });

        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = configuration.GetConnectionString("Redis");
            options.InstanceName = "BackendManagement:";
        });

        return services;
    }
}

public enum DatabaseType
{
    MySql,
    PostgreSQL,
    SQLite
} 