using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace BackendManagement.WebAPI.Documentation;

/// <summary>
/// Swagger文件過濾器
/// </summary>
public class SwaggerDocumentFilter : IDocumentFilter
{
    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        // 實作 Swagger 文件過濾邏輯
    }
} 