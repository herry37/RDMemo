# 任務管理系統 (Task Management API)

這是一個基於 .NET Core 開發的任務管理系統，提供了直觀的網頁界面來管理待辦事項。系統採用最新的 WebSocket 技術實現即時更新，並提供完整的中文化支持。

## 功能特點

### 1. 任務管理
- **創建任務**
  - 設置任務標題（必填，最多 200 字符）
  - 添加任務描述（可選，最多 1000 字符）
  - 設置截止日期（可選）
  - 設置優先級（低、中、高、緊急）
  - 自動記錄創建時間

- **查看任務**
  - 分頁顯示所有任務
  - 顯示任務的完成狀態
  - 顯示任務的優先級（使用不同顏色標識）
  - 顯示任務的截止日期
  - 支持多種篩選條件

- **更新任務**
  - 標記任務為已完成/未完成
  - 修改任務內容
  - 更新截止日期
  - 調整優先級
  - 自動記錄更新時間

- **刪除任務**
  - 刪除不需要的任務
  - 確認提示防止誤刪

### 2. 用戶界面特性
- **響應式設計**：適配不同設備尺寸
- **即時更新**
  - WebSocket 實時推送更新
  - 自動刷新任務狀態
  - 多客戶端同步
- **直觀的操作界面**
  - 彈出式表單進行任務創建
  - 點擊即可完成任務狀態切換
  - 優先級顏色標識（紅色：緊急，橙色：高優先級，黃色：中優先級，綠色：低優先級）
  - 逾期任務特殊標記

### 3. 技術特性
- **時區處理**
  - 自動處理本地時間與 UTC 時間的轉換
  - 支持台北時區（UTC+8）
  - 準確顯示任務截止時間
  - 自動檢測逾期任務

- **資料驗證**
  - 必填字段驗證
  - 字符長度限制
  - 日期格式驗證
  - 優先級範圍驗證

- **錯誤處理**
  - 友好的錯誤提示
  - 完整的錯誤日誌
  - 異常狀態恢復機制

## 技術架構

### 前端
- HTML5
- CSS3
- JavaScript
- WebSocket API
- 原生 Fetch API 進行 HTTP 請求

### 後端
- .NET Core Web API
- Entity Framework Core
- WebSocket 服務
- SQLite 數據庫
- 非同步任務處理

## API 端點

### 任務管理
- `GET /api/TodoTasks`: 獲取任務列表（支持分頁和篩選）
- `POST /api/TodoTasks`: 創建新任務
- `PUT /api/TodoTasks/{id}`: 更新任務
- `DELETE /api/TodoTasks/{id}`: 刪除任務
- `PATCH /api/TodoTasks/{id}/status`: 更新任務狀態
- `GET /api/TodoTasks/overdue`: 獲取逾期任務
- `GET /api/TodoTasks/priority/{priority}`: 按優先級獲取任務
- `ws://hostname/ws`: WebSocket 連接端點

## 數據結構

### 任務（TodoTask）
```json
{
    "id": "整數，任務唯一標識符",
    "title": "字符串，任務標題（必填，最多 200 字符）",
    "description": "字符串，任務描述（可選，最多 1000 字符）",
    "isCompleted": "布爾值，任務完成狀態",
    "priority": "枚舉（Low/Medium/High/Urgent），任務優先級",
    "dueDate": "日期時間，任務截止日期（可選）",
    "createdAt": "日期時間，任務創建時間（自動生成）",
    "updatedAt": "日期時間，任務最後更新時間（自動更新）",
    "completedAt": "日期時間，任務完成時間（可選）"
}
```

### 分頁結果（PagedResult）
```json
{
    "items": "任務數組",
    "totalItems": "整數，總記錄數",
    "currentPage": "整數，當前頁碼",
    "pageSize": "整數，每頁記錄數",
    "totalPages": "整數，總頁數"
}
```

## 使用說明

1. **創建新任務**
   - 點擊「新增任務」按鈕
   - 填寫任務信息（標題為必填）
   - 可選擇設置優先級和截止日期
   - 點擊「保存」提交

2. **管理任務**
   - 點擊任務列表中的複選框切換完成狀態
   - 點擊編輯圖標修改任務內容
   - 點擊刪除圖標移除任務
   - 使用篩選器查看不同狀態的任務

3. **查看任務列表**
   - 任務默認按優先級和創建時間排序
   - 使用分頁導航查看更多任務
   - 即時接收其他用戶的更新

4. **監控任務狀態**
   - 系統自動標記逾期任務
   - 通過顏色區分不同優先級
   - 即時顯示任務更新狀態

## 注意事項

1. 所有時間都以台北時區（UTC+8）顯示
2. 任務標題為必填項且不能超過 200 字符
3. 系統會自動保存所有更改並即時同步到其他客戶端
4. 定期備份數據庫文件以防數據丟失
5. WebSocket 連接斷開時會自動重連
6. 建議定期清理已完成的舊任務
