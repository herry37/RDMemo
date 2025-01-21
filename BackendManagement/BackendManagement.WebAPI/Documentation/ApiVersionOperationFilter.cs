using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace BackendManagement.WebAPI.Documentation;

/// <summary>
/// API版本操作過濾器
/// </summary>
public class ApiVersionOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        // 實作 API 版本過濾邏輯
    }
} 