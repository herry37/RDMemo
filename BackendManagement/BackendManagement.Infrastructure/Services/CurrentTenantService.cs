using Microsoft.AspNetCore.Http;
using BackendManagement.Application.Common.Interfaces;
using System;

namespace BackendManagement.Infrastructure.Services;

public class CurrentTenantService : ICurrentTenantService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private const string TenantIdHeaderName = "X-TenantId";

    public CurrentTenantService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid? GetCurrentTenantId()
    {
        var tenantIdStr = _httpContextAccessor.HttpContext?.Request.Headers[TenantIdHeaderName].ToString();
        
        if (string.IsNullOrEmpty(tenantIdStr))
        {
            return null;
        }

        return Guid.TryParse(tenantIdStr, out Guid tenantId) ? tenantId : null;
    }
} 