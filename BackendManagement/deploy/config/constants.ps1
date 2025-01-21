# 定義部署相關常數
$script:REQUIRED_TOOLS = @{
    "dotnet" = "8.0"
    "docker" = "24.0"
    "kubectl" = "1.28"
    "helm" = "3.0"
}

$script:NAMESPACES = @(
    "monitoring",
    "logging",
    "cert-manager",
    "backend-management"
)

$script:RESOURCE_LIMITS = @{
    "cpu" = @{
        "request" = "200m"
        "limit" = "500m"
    }
    "memory" = @{
        "request" = "256Mi"
        "limit" = "512Mi"
    }
}

$script:HEALTH_CHECK = @{
    "path" = "/health"
    "initialDelay" = 5
    "period" = 10
}

# 錯誤訊息常數
$script:ERROR_MESSAGES = @{
    "TOOL_NOT_FOUND" = "找不到必要工具: {0}"
    "VERSION_MISMATCH" = "工具版本不符: {0} (需要 {1})"
    "CONNECTION_FAILED" = "無法連線到服務: {0}"
    "CONFIG_MISSING" = "找不到配置檔案: {0}"
    "DEPLOYMENT_FAILED" = "部署失敗: {0}"
} 