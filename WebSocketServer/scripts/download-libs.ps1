# 建立必要的目錄
$libPath = "wwwroot/lib"
$leafletPath = "$libPath/leaflet"
New-Item -ItemType Directory -Force -Path $leafletPath | Out-Null

# 下載 Leaflet
$leafletVersion = "1.9.4"
$leafletFiles = @(
    "leaflet.js",
    "leaflet.css",
    "images/layers.png",
    "images/layers-2x.png",
    "images/marker-icon.png",
    "images/marker-icon-2x.png",
    "images/marker-shadow.png"
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
        $dest = "$leafletPath/$file"
        if (!(Test-Path $dest)) {
            $needsDownload = $true
            Write-Host "檔案遺失：$file"
            break
        }
    }
}

if ($needsDownload) {
    Write-Host "開始下載 Leaflet v$leafletVersion..."
    
    foreach ($file in $leafletFiles) {
        $url = "https://unpkg.com/leaflet@$leafletVersion/dist/$file"
        $dest = "$leafletPath/$file"
        
        # 確保目標目錄存在
        $directory = Split-Path -Parent $dest
        if (!(Test-Path $directory)) {
            New-Item -ItemType Directory -Force -Path $directory | Out-Null
        }
        
        # 下載檔案
        Write-Host "下載 $file..."
        try {
            Invoke-WebRequest -Uri $url -OutFile $dest
        } catch {
            Write-Error "下載 $file 失敗: $_"
            exit 1
        }
    }
    
    # 儲存版本資訊
    $leafletVersion | Out-File $versionFile -NoNewline
    Write-Host "Leaflet v$leafletVersion 下載完成！"
} else {
    Write-Host "Leaflet v$leafletVersion 已經是最新版本，無需更新。"
} 