# 高雄市垃圾車即時位置系統

這是一個用於追蹤高雄市垃圾車即時位置的網頁應用程式。系統會定期從高雄市政府開放資料平台獲取垃圾車位置資訊，並在地圖上即時顯示。

## 功能特點

- 即時顯示垃圾車位置
- 自動更新位置資訊
- 顯示車輛行進方向
- 支援行動裝置瀏覽
- 智慧重試機制
- 完整的錯誤處理和日誌記錄

## 技術架構

### 後端

- ASP.NET Core 7.0
- Memory Cache
- HTTP Client Factory
- 健康檢查機制

### 前端

- HTML5, JavaScript (原生)
- Leaflet.js 地圖函式庫
- 動態模組載入
- 自動重試機制

### 基礎設施

- 部署環境：Somee.com
- 資料來源：高雄市政府開放資料平台
- IIS URL Rewrite Module

## 系統需求

- .NET 7.0 SDK
- 支援 ES6+ 的現代瀏覽器
- 網際網路連線

## 安裝說明

### 本機開發環境

1. 安裝必要工具：

   ```bash
   # 檢查 .NET SDK 版本
   dotnet --version   # 應為 7.0.x 或更高
   ```

2. 複製專案：

   ```bash
   git clone [repository-url]
   cd WebSocketServer
   ```

3. 下載前端相依套件：

   ```bash
   # Windows PowerShell
   ./scripts/download-libs.ps1
   ```

4. 執行專案：

   ```bash
   dotnet run
   ```

5. 開啟瀏覽器：
   - 開發環境：http://localhost:44366
   - 測試 API：http://localhost:44366/api/trucks

### 正式環境部署 (Somee)

1. 建置發布檔案：

   ```bash
   dotnet publish -c Release
   ```

2. 部署檢查清單：
   - [ ] 上傳 publish 資料夾內容
   - [ ] 確認 web.config 位於根目錄
   - [ ] 檢查 URL 重寫規則
   - [ ] 驗證靜態檔案存取權限

## 路由配置

### 開發環境

```
API 端點: /api/trucks
靜態檔案: /
```

### 正式環境

```
API 端點: /WebSocketServer/api/trucks
靜態檔案: /WebSocketServer/
```

## 前端架構

### 資源載入順序

1. 基礎函數和工具
2. Leaflet CSS
3. DOM 結構
4. Leaflet JS（動態載入）
5. TruckMap 模組（動態載入）

### 錯誤處理機制

- 資源載入重試
- 自動重新載入
- 使用者友善錯誤訊息
- 詳細的除錯日誌

## 疑難排解指南

### 常見問題與解決方案

1. 地圖無法載入

   ```javascript
   // 檢查 Leaflet 載入狀態
   console.log(typeof L !== "undefined");
   // 檢查地圖容器
   console.log(document.getElementById("map"));
   ```

2. API 請求失敗

   ```javascript
   // 檢查 API 基礎路徑
   console.log(getApiBaseUrl());
   // 檢查網路請求
   fetch(getApiBaseUrl() + "/trucks").then(console.log);
   ```

3. 路徑解析問題
   ```javascript
   // 檢查當前路徑
   console.log(window.location.pathname);
   // 檢查靜態資源路徑
   console.log(getStaticPath("/js/truckMap.js"));
   ```

### 除錯工具

1. 瀏覽器開發者工具

   - Network 面板：檢查資源載入
   - Console 面板：查看錯誤訊息
   - Sources 面板：除錯 JavaScript

2. 伺服器日誌
   - 應用程式日誌：`logs/stdout`
   - IIS 日誌：檢查請求處理

## 維護指南

### 日常維護

- 定期檢查日誌
- 監控系統效能
- 更新相依套件

### 效能優化

- 使用快取機制
- 最小化資源載入
- 錯誤重試策略

### 安全性考量

- HTTPS 傳輸
- 輸入驗證
- 錯誤訊息過濾

## 貢獻指南

1. 程式碼風格

   - 使用 ESLint
   - 遵循 C# 編碼規範
   - 保持一致的命名風格

2. 提交規範

   - 清晰的提交訊息
   - 包含相關 issue 編號
   - 提供測試案例

3. 審查流程
   - 程式碼審查
   - 測試覆蓋
   - 文件更新
