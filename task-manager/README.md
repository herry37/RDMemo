# 待辦事項管理系統

現代化的任務管理系統，使用 React + Vite 前端和 Node.js + Express 後端開發。

## 功能特點

- 📋 任務管理（新增、編輯、刪除）
- ✅ 任務狀態切換（完成/未完成）
- 🏷️ 優先級設定（高/中/低）
- 📅 截止日期管理
- 🔍 多條件篩選（狀態、優先級）

## 技術架構

### 前端

- React 18 + Vite
- Tailwind CSS
- Axios
- PropTypes
- ESLint

### 後端

- Node.js + Express
- SQLite 資料庫
- Express Rate Limit
- 錯誤處理中間件

## 開始使用

### 環境需求

- Node.js >= 14
- npm >= 6

### 安裝步驟

1. 後端設定

```bash
cd backend
npm install
npm start
```

2. 前端設定

```bash
cd frontend
npm install

# 設定環境變數
# .env.development
VITE_API_URL=http://localhost:5000

npm run dev
```

## API 文件

| 方法   | 端點       | 說明     | 請求體範例              |
| ------ | ---------- | -------- | ----------------------- |
| GET    | /tasks     | 取得列表 | -                       |
| POST   | /tasks     | 新增任務 | `{ "title": "新任務" }` |
| PUT    | /tasks/:id | 更新任務 | `{ "completed": true }` |
| DELETE | /tasks/:id | 刪除任務 | -                       |

## 專案結構

```
project/
├── frontend/
│   ├── src/
│   │   ├── components/  # React 元件
│   │   ├── hooks/      # 自定義 Hooks
│   │   └── App.jsx
│   └── package.json
└── backend/
    ├── middleware/    # Express 中間件
    ├── repositories/  # 資料存取層
    └── server.js
```

## 開發指南

### 程式碼規範

- 使用 ESLint 進行程式碼檢查
- 使用 PropTypes 進行型別檢查
- 保持適當的程式碼註解

### 錯誤處理

- 使用 try-catch 處理非同步操作
- 顯示友善的錯誤訊息
- 實作錯誤回復機制

## 部署說明

### 前端部署

```bash
npm run build
# 部署 dist 目錄內容
```

### 後端部署

```bash
npm install --production
npm start
```

## 注意事項

- 確保環境變數正確設定
- 定期更新相依套件
- 遵循 Git Flow 工作流程
- 進行充分的錯誤處理

## 授權

本專案採用 MIT 授權

## 版本資訊

- 版本：1.0.0
- 更新日期：2025-02-09
- 開發者：buddy_anybody0h@icloud.com
