# �إߥ��n���ؿ�
$libPath = "wwwroot/lib"
$leafletPath = "$libPath/leaflet"
New-Item -ItemType Directory -Force -Path "$leafletPath/images" | Out-Null

# �U�� Leaflet
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

# �U���ɮ�
foreach ($file in $leafletFiles) {
    $dest = "$leafletPath/$($file.name)"
    $destDir = Split-Path -Parent $dest
    
    if (!(Test-Path $destDir)) {
        New-Item -ItemType Directory -Force -Path $destDir | Out-Null
    }
    
    Write-Host "�U��: $($file.url) -> $dest"
    Invoke-WebRequest -Uri $file.url -OutFile $dest
}

Write-Host "Leaflet v$leafletVersion �U�������I" 