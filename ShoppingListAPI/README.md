# Shopping List API

這是一個使用 ASP.NET Core 8.0 開發的購物清單 API 服務，提供購物清單的建立、查詢、修改和刪除功能。

## 功能特點

- 購物清單的 CRUD 操作
- WebSocket 即時通知
- 檔案式資料儲存
- Swagger API 文件
- 跨域資源共享 (CORS) 支援

## 系統需求

- .NET SDK 8.0.303 或更高版本
- Windows/macOS/Linux 作業系統

## 專案結構

```
ShoppingListAPI/
├── Controllers/         # API 控制器
├── Models/             # 資料模型
├── Services/           # 服務層
│   ├── FileDb/        # 檔案資料庫服務
│   └── WebSocket/     # WebSocket 服務
├── Data/              # 資料儲存
│   └── FileStore/     # 檔案儲存目錄
├── wwwroot/           # 靜態檔案
│   ├── index.html     # 前端頁面
│   ├── css/          # 樣式檔案
│   └── js/           # JavaScript 檔案
└── Utils/             # 工具類別
```

## 安裝與執行

1. 複製專案
```bash
git clone [repository-url]
cd ShoppingListAPI
```

2. 還原相依套件
```bash
dotnet restore
```

3. 執行專案
```bash
dotnet run
```

專案預設會在 http://localhost:5002 啟動。

## API 端點

### 購物清單

- `GET /api/shoppinglist` - 取得所有購物清單
- `GET /api/shoppinglist/{id}` - 取得特定購物清單
- `POST /api/shoppinglist` - 建立新的購物清單
- `PUT /api/shoppinglist/{id}` - 更新購物清單
- `DELETE /api/shoppinglist/{id}` - 刪除購物清單

### WebSocket

- `ws://localhost:5002/ws` - WebSocket 連接端點

## 資料儲存

本專案使用檔案系統進行資料儲存，所有資料都保存在 `Data/FileStore` 目錄下：
- `shoppinglists/` - 儲存購物清單資料

## 開發工具

- Visual Studio 2022 或 VS Code
- Swagger UI (開發環境可用)
- Postman 或其他 API 測試工具

## 設定檔說明

### appsettings.json
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```

### appsettings.Development.json
包含開發環境特定的設定，如詳細的日誌級別等。

## 安全性考量

- 使用 CORS 政策限制存取來源
- WebSocket 通訊加密
- 資料驗證和清理

## 效能最佳化

- 使用記憶體快取
- 非同步操作
- 檔案系統最佳化存取

## 錯誤處理

系統實作了全域的例外處理，所有的 API 回應都遵循統一的格式：

```json
{
  "success": true/false,
  "message": "操作訊息",
  "data": { ... }
}
```

## 貢獻指南

1. Fork 專案
2. 建立特性分支
3. 提交變更
4. 發送 Pull Request

## 授權

[授權類型]

## 作者

[作者資訊]

## 更新日誌

### 1.0.0
- 初始版本發布
- 基本的 CRUD 功能
- WebSocket 支援
- 檔案資料庫實作
