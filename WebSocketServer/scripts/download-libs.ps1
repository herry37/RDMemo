# 建立必要的目錄
$libPath = "wwwroot/lib"
$leafletPath = "$libPath/leaflet"
New-Item -ItemType Directory -Force -Path $leafletPath | Out-Null

# 下載 Leaflet
$leafletVersion = "1.9.4"
$leafletFiles = @(
    @{
        name = "leaflet.js"
        url = "https://cdnjs.cloudflare.com/ajax/libs/leaflet/1.9.4/leaflet.js"
    },
    @{
        name = "leaflet.css"
        url = "https://cdnjs.cloudflare.com/ajax/libs/leaflet/1.9.4/leaflet.css"
    },
    @{
        name = "images/layers.png"
        url = "https://cdnjs.cloudflare.com/ajax/libs/leaflet/1.9.4/images/layers.png"
    },
    @{
        name = "images/layers-2x.png"
        url = "https://cdnjs.cloudflare.com/ajax/libs/leaflet/1.9.4/images/layers-2x.png"
    },
    @{
        name = "images/marker-icon.png"
        url = "https://cdnjs.cloudflare.com/ajax/libs/leaflet/1.9.4/images/marker-icon.png"
    },
    @{
        name = "images/marker-icon-2x.png"
        url = "https://cdnjs.cloudflare.com/ajax/libs/leaflet/1.9.4/images/marker-icon-2x.png"
    },
    @{
        name = "images/marker-shadow.png"
        url = "https://cdnjs.cloudflare.com/ajax/libs/leaflet/1.9.4/images/marker-shadow.png"
    }
)

# 檢查版本檔案
$versionFile = "$leafletPath/version.txt"
$currentVersion = ""
if (Test-Path $versionFile) {
    $currentVersion = Get-Content $versionFile
}

# 如果版本相同且檔案都存在，則跳過下載
$needsDownload = $false
if ($currentVersion -ne $leafletVersion) {
    $needsDownload = $true
    Write-Host "需要更新：目前版本 $currentVersion -> 新版本 $leafletVersion"
} else {
    # 檢查所有檔案是否存在
    foreach ($file in $leafletFiles) {
        $dest = "$leafletPath/$($file.name)"
        if (!(Test-Path $dest)) {
            $needsDownload = $true
            Write-Host "檔案遺失：$($file.name)"
            break
        }
    }
}

function Download-WithRetry {
    param (
        [string]$url,
        [string]$dest,
        [int]$maxRetries = 3,
        [int]$retryDelaySeconds = 2
    )
    
    $attempt = 0
    do {
        $attempt++
        try {
            Write-Host "下載中... 嘗試 $attempt/$maxRetries"
            
            # 使用 System.Net.WebClient (較穩定的下載方式)
            $webClient = New-Object System.Net.WebClient
            $webClient.Headers.Add("User-Agent", "PowerShell Script")
            $webClient.DownloadFile($url, $dest)
            
            Write-Host "下載成功！"
            return $true
        }
        catch {
            Write-Host "下載失敗: $_"
            if ($attempt -lt $maxRetries) {
                Write-Host "等待 $retryDelaySeconds 秒後重試..."
                Start-Sleep -Seconds $retryDelaySeconds
                $retryDelaySeconds *= 2  # 指數退避
            }
        }
        finally {
            if ($webClient) {
                $webClient.Dispose()
            }
        }
    } while ($attempt -lt $maxRetries)
    
    return $false
}

if ($needsDownload) {
    Write-Host "開始下載 Leaflet v$leafletVersion..."
    $downloadSuccess = $true
    
    foreach ($file in $leafletFiles) {
        $dest = "$leafletPath/$($file.name)"
        
        # 確保目標目錄存在
        $directory = Split-Path -Parent $dest
        if (!(Test-Path $directory)) {
            New-Item -ItemType Directory -Force -Path $directory | Out-Null
        }
        
        # 下載檔案
        Write-Host "下載 $($file.name)..."
        if (!(Download-WithRetry -url $file.url -dest $dest)) {
            $downloadSuccess = $false
            Write-Error "下載 $($file.name) 失敗"
            break
        }
    }
    
    if ($downloadSuccess) {
        # 儲存版本資訊
        $leafletVersion | Out-File $versionFile -NoNewline
        Write-Host "Leaflet v$leafletVersion 下載完成！"
    }
} else {
    Write-Host "Leaflet v$leafletVersion 已經是最新版本，無需更新。"
} 