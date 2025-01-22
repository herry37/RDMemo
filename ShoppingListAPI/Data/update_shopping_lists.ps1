# Get all JSON files in the shoppinglists directory
$files = Get-ChildItem -Path "FileStore/shoppinglists" -Filter "*.json"

foreach ($file in $files) {
    # Read the JSON content
    $content = Get-Content $file.FullName -Raw | ConvertFrom-Json
    
    # Add BuyData if it doesn't exist
    if (-not $content.BuyData) {
        # Extract date from title if possible, otherwise use CreatedAt date
        if ($content.Title -match "\d{2}/\d{2}") {
            $dateFromTitle = $matches[0]
            $content | Add-Member -NotePropertyName "BuyData" -NotePropertyValue "2024-$dateFromTitle"
        } else {
            $createdDate = [DateTime]::Parse($content.CreatedAt)
            $content | Add-Member -NotePropertyName "BuyData" -NotePropertyValue $createdDate.ToString("yyyy-MM-dd")
        }
        
        # Convert back to JSON and save
        $content | ConvertTo-Json -Depth 10 | Set-Content $file.FullName
    }
}
