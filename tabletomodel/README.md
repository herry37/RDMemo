# TableToModel - 資料表轉 Model 工具

這是一個用於將資料庫表格自動轉換為 C# Model 類別的工具。支援 SQL Server、MySQL 和 MongoDB 資料庫，可以快速生成包含完整資料註解和驗證特性的實體類別。

## 功能特點

- 支援多種資料庫
  - Microsoft SQL Server
  - MySQL
  - MongoDB
- 自動生成 Model 類別
  - 自動對應資料類型
  - 支援可為空類型
  - 包含欄位驗證特性
  - 自動生成文件註解
  - MongoDB 特定功能：
    - 支援 BsonElement 特性
    - 動態類型支援
    - ObjectId 處理
    - 巢狀文檔支援
- 完整的錯誤處理
  - 參數驗證
  - 連線測試
  - 詳細的錯誤訊息
- 安全性考量
  - SQL 注入防護
  - 密碼安全處理
  - 連線加密

## 系統需求

- .NET 8.0 或更高版本
- Windows 作業系統
- Visual Studio 2022 或更高版本
- 支援的資料庫：
  - SQL Server 2012 或更高版本
  - MySQL 5.7 或更高版本
  - MongoDB 4.0 或更高版本

## 安裝說明

1. 下載並解壓縮專案檔案
2. 使用 Visual Studio 2022 開啟解決方案
3. 還原 NuGet 套件：
   - Dapper
   - System.Data.SqlClient
   - MySql.Data
   - MongoDB.Driver
4. 編譯專案

## 使用說明

### 使用者介面

1. 啟動應用程式
2. 輸入資料庫連線資訊：
   - 伺服器位址
     - SQL Server/MySQL：localhost 或 IP 位址
     - MongoDB：mongodb://localhost:27017
   - 資料庫名稱
   - 使用者名稱
   - 密碼
3. 選擇資料庫類型（SQL Server、MySQL 或 MongoDB）
4. 輸入要轉換的資料表/集合名稱
5. 點擊「產生 Model」按鈕

### 程式碼範例

```csharp
// SQL Server / MySQL
var converter = new ModelConverter();
string modelCode = converter.GetModel(
    DatabaseConnectionFactory.DatabaseType.MSSQL, // 或 MySQL
    "localhost",
    "sa",
    "password",
    "DatabaseName",
    "TableName"
);

// MongoDB
var mongoGenerator = new MongoDbModelGenerator();
string mongoModelCode = await mongoGenerator.GenerateModelAsync(
    "localhost:27017",
    "username",
    "password",
    "DatabaseName",
    "CollectionName"
);
```

### MongoDB 特別說明

MongoDB 是文件型資料庫，其結構與關聯式資料庫不同：

1. **集合結構分析**
   - 系統會分析集合中的文件來推斷結構
   - 支援動態類型和巢狀文件
   - 自動處理不同類型的值

2. **類型映射**
   - ObjectId → ObjectId
   - Array → BsonArray
   - Document → BsonDocument
   - Number → int?, long?, double?, decimal?
   - Date → DateTime?
   - Binary → byte[]
   - Boolean → bool?

3. **特殊處理**
   - 自動添加 [BsonId] 特性
   - 使用 [BsonElement] 標記欄位名稱
   - 支援巢狀文件的序列化
   - 處理陣列和子文件

## 專案結構

```
TableToModel/
├── Program.cs                    # 程式進入點
├── TableToModel.cs              # Windows Form 主視窗
├── ModelConverter.cs            # Model 轉換邏輯
├── DatabaseConnectionFactory.cs # 資料庫連線工廠
├── MongoDbModelGenerator.cs    # MongoDB Model 生成器
├── MongoDbConnectionFactory.cs # MongoDB 連線工廠
├── Exceptions/                  # 例外處理類別
│   ├── DatabaseConnectionException.cs
│   ├── TableNotFoundException.cs
│   └── ModelGenerationException.cs
└── Models/                      # 生成的 Model 類別目標資料夾
```

## 主要元件說明

### ModelConverter
負責將關聯式資料庫表格結構轉換為 C# Model 類別。

### MongoDbModelGenerator
專門處理 MongoDB 集合到 C# Model 的轉換：
- 分析集合結構
- 處理動態類型
- 生成 MongoDB 特定特性
- 支援巢狀文件

### DatabaseConnectionFactory
處理資料庫連線相關操作，支援：
- SQL Server
- MySQL
- MongoDB

## 錯誤處理

系統定義了以下例外類型：

1. `DatabaseConnectionException`
   - 資料庫連線失敗
   - 連線字串錯誤
   - 驗證失敗

2. `TableNotFoundException`
   - 找不到指定的資料表/集合
   - 資料表/集合沒有欄位
   - MongoDB 集合為空

3. `ModelGenerationException`
   - Model 生成過程中的錯誤
   - 資料類型轉換錯誤
   - MongoDB 結構分析錯誤

## 安全性考量

1. 參數驗證
   - 所有輸入參數都經過驗證
   - 防止 SQL 注入攻擊
   - 特殊字元過濾
   - MongoDB 查詢安全性

2. 連線安全
   - 使用參數化查詢
   - 支援加密連線
   - 密碼安全處理
   - MongoDB 連線字串轉義

## 最佳實踐

1. 命名規範
   - 使用有意義的類別名稱
   - 保持資料表/集合名稱的原始大小寫
   - MongoDB 欄位名稱轉換規則

2. 資料類型對應
   - 使用適當的可為空類型
   - 正確處理特殊資料類型
   - MongoDB 動態類型處理

3. 效能考量
   - 設定適當的連線超時
   - 使用連線池
   - 資源適時釋放
   - MongoDB 索引使用建議

## 常見問題

1. **連線失敗**
   - 檢查連線字串是否正確
   - 確認資料庫服務是否執行中
   - 驗證使用者權限
   - MongoDB URL 格式是否正確

2. **找不到資料表/集合**
   - 確認名稱大小寫
   - 檢查資料庫名稱是否正確
   - 確認使用者是否有存取權限
   - MongoDB 集合是否為空

3. **型別轉換錯誤**
   - 檢查資料表欄位型別
   - 確認是否有不支援的資料類型
   - MongoDB 動態類型處理問題
   - 巢狀文件結構問題

## 版本歷程

### 1.1.0 (2025-01-22)
- 新增 MongoDB 支援
- 加入動態類型處理
- 支援巢狀文件結構
- 改進錯誤處理機制

### 1.0.0 (2025-01-22)
- 初始版本
- 支援 SQL Server 和 MySQL
- 基本的 Model 生成功能
- Windows Forms 使用者介面

## 授權資訊

本專案採用 MIT 授權。詳細內容請參考 LICENSE 檔案。

## 作者資訊

開發者：[Jesse](https://github.com/herry37/RDMemo)

## 貢獻指南

歡迎提供意見和建議，或提交 Pull Request。請確保：
1. 程式碼符合專案的程式碼風格
2. 新功能包含適當的測試
3. 更新相關文件
