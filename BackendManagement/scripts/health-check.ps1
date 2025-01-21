# 健康檢查腳本
param(
    [string]$BaseUrl = "http://localhost:5000",
    [string]$LogPath = "health-check.log"
)

function Write-Log {
    param($Message)
    $logMessage = "$(Get-Date -Format 'yyyy-MM-dd HH:mm:ss'): $Message"
    Add-Content -Path $LogPath -Value $logMessage
    Write-Host $logMessage
}

# 檢查 API 健康狀態
try {
    $response = Invoke-RestMethod -Uri "$BaseUrl/health" -Method Get
    if ($response.status -eq "Healthy") {
        Write-Log "API 健康狀態正常"
    }
    else {
        Write-Log "警告：API 健康狀態異常 - $($response.status)"
    }
}
catch {
    Write-Log "錯誤：無法連接到 API - $_"
}

# 檢查資料庫連線
try {
    $response = Invoke-RestMethod -Uri "$BaseUrl/health/database" -Method Get
    Write-Log "資料庫連線狀態：$($response.status)"
}
catch {
    Write-Log "錯誤：資料庫健康檢查失敗 - $_"
}

# 檢查 Redis 連線
try {
    $response = Invoke-RestMethod -Uri "$BaseUrl/health/redis" -Method Get
    Write-Log "Redis 連線狀態：$($response.status)"
}
catch {
    Write-Log "錯誤：Redis 健康檢查失敗 - $_"
} 