# 高雄市垃圾車即時位置系統

這是一個用於追蹤高雄市垃圾車即時位置的網頁應用程式。系統會定期從高雄市政府開放資料平台獲取垃圾車位置資訊，並在地圖上即時顯示。

## 功能特點

- 即時顯示垃圾車位置
- 自動更新位置資訊（每 3-5 秒）
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
- Leaflet.js 地圖函式庫 (CDN)
- OpenStreetMap 地圖資料
- 響應式設計

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

3. 執行專案：

   ```bash
   dotnet run
   ```

4. 開啟瀏覽器：
   - 開發環境：http://localhost:5000
   - 測試 API：http://localhost:5000/api/trucks

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
   - [ ] 確認 CSP 設定正確

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

### 資源載入

1. Leaflet CSS (CDN)
2. Leaflet JS (CDN)
3. TruckMap 模組（動態載入）

### 主要元件

- TruckMap 類別：負責地圖初始化和資料管理
- 側邊欄：顯示垃圾車列表和狀態資訊
- 響應式設計：支援桌面和行動裝置

### 錯誤處理機制

- 自動重試機制
- 指數退避策略
- 使用者友善錯誤訊息
- 詳細的除錯日誌

## 安全性設定

### Content Security Policy

```
default-src 'self' 'unsafe-inline' 'unsafe-eval' https://cdnjs.cloudflare.com https://*.openstreetmap.org data:
img-src 'self' data: https://*.openstreetmap.org https://cdnjs.cloudflare.com blob: *
style-src 'self' 'unsafe-inline' https://cdnjs.cloudflare.com
font-src 'self' data: https://cdnjs.cloudflare.com
```

## 疑難排解指南

### 常見問題與解決方案

1. 地圖無法載入

   - 檢查網路連線
   - 確認 CDN 資源可訪問
   - 查看瀏覽器控制台錯誤訊息

2. API 請求失敗

   - 確認 API 端點設定正確
   - 檢查 CORS 設定
   - 查看伺服器日誌

3. 資源載入錯誤
   - 確認 CSP 設定正確
   - 檢查檔案路徑
   - 驗證檔案權限

## 維護指南

### 日常維護

- 定期檢查日誌
- 監控系統效能
- 更新相依套件

### 版本更新

- 遵循語意化版本規範
- 保持向下相容性
- 詳細記錄更新內容

## 授權資訊

本專案採用 MIT 授權。詳見 [LICENSE](LICENSE) 檔案。

## 貢獻指南

歡迎提交 Issue 和 Pull Request。請確保：

1. 遵循現有的程式碼風格
2. 新增適當的測試
3. 更新相關文件
