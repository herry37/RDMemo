using module "./Constants.psm1"

function Test-Prerequisites {
    [CmdletBinding()]
    param()

    Write-Host "檢查必要工具..."
    
    # 檢查工具版本
    foreach ($tool in $REQUIRED_TOOLS.Keys) {
        if (-not (Get-Command $tool -ErrorAction SilentlyContinue)) {
            throw ($ERROR_MESSAGES.TOOL_NOT_FOUND -f $tool)
        }
        
        $version = & $tool --version
        if (-not $version -match $REQUIRED_TOOLS[$tool]) {
            throw ($ERROR_MESSAGES.VERSION_MISMATCH -f $tool, $REQUIRED_TOOLS[$tool])
        }
    }
}

function Test-Configuration {
    [CmdletBinding()]
    param(
        [Parameter(Mandatory)]
        [string]$ConfigPath
    )
    
    if (-not (Test-Path $ConfigPath)) {
        throw ($ERROR_MESSAGES.CONFIG_MISSING -f $ConfigPath)
    }

    $config = Get-Content $ConfigPath | ConvertFrom-Json

    # 驗證必要配置
    $requiredFields = @(
        "environment",
        "registry.url",
        "database.connectionString",
        "redis.connectionString"
    )

    foreach ($field in $requiredFields) {
        $value = Invoke-Expression "`$config.$field"
        if ([string]::IsNullOrEmpty($value)) {
            throw "配置缺少必要欄位: $field"
        }
    }

    return $config
}

function Test-ServiceHealth {
    [CmdletBinding()]
    param(
        [Parameter(Mandatory)]
        [string]$Namespace,
        [Parameter(Mandatory)]
        [string]$ServiceName
    )

    $status = kubectl get pods -l app=$ServiceName -n $Namespace -o jsonpath="{.items[*].status.phase}"
    if ($status -ne "Running") {
        Write-Warning "服務狀態異常: $ServiceName"
        kubectl describe pods -l app=$ServiceName -n $Namespace
        return $false
    }

    return $true
}

Export-ModuleMember -Function Test-Prerequisites, Test-Configuration, Test-ServiceHealth 