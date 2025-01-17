# CrossPlatformDataAccess

跨平台資料存取類別庫，支援多種資料庫和存取方式。

## 功能特點

- 支援多種資料庫（SQL Server、MySQL、PostgreSQL、MongoDB等）
- 支援多種存取方式（EF Core、Dapper、ADO.NET）
- 內建交易管理
- 完整的日誌記錄
- 支援非同步操作
- 支援查詢建構器
- 支援原生SQL查詢

## 使用方式

1. 安裝套件：

```bash
dotnet add package CrossPlatformDataAccess
```

2. 設定依賴注入：

```csharp
services.AddDataAccess(options =>
{
    options.UseDatabase(DatabaseType.SqlServer, configuration.GetConnectionString("DefaultConnection"));
});
```

3. 使用範例：

```csharp
public class UserService
{
    private readonly GenericRepository<User> _repository;

    public UserService(GenericRepository<User> repository)
    {
        _repository = repository;
    }

    public async Task<User> CreateUserAsync(User user)
    {
        return await _repository.AddAsync(user);
    }

    public async Task<IEnumerable<User>> GetActiveUsersAsync()
    {
        return await _repository.GetAsync(u => u.IsActive);
    }

    // 使用原生SQL
    public async Task<IEnumerable<User>> GetUsersByRoleAsync(string role)
    {
        return await _repository.QueryWithSqlAsync(
            "SELECT * FROM Users WHERE Role = @Role",
            new { Role = role }
        );
    }

    // 使用查詢建構器
    public async Task<IEnumerable<User>> GetPaginatedUsersAsync(int page, int pageSize)
    {
        return await _repository.Query()
            .OrderBy(u => u.Name)
            .Page(page, pageSize)
            .ToListAsync();
    }
}
```

## 交易管理

庫內建交易管理，自動處理交易的開始、提交和回滾：

```csharp
public async Task TransferMoneyAsync(int fromAccountId, int toAccountId, decimal amount)
{
    await _repository.ExecuteInTransactionAsync(async () =>
    {
        var fromAccount = await _repository.GetAsync(a => a.Id == fromAccountId);
        var toAccount = await _repository.GetAsync(a => a.Id == toAccountId);

        // 執行轉帳操作
        fromAccount.Balance -= amount;
        toAccount.Balance += amount;

        await _repository.UpdateAsync(fromAccount);
        await _repository.UpdateAsync(toAccount);
    }, "轉帳操作");
}
```

## 日誌記錄

所有資料庫操作都會自動記錄日誌：

- 操作開始和完成
- 錯誤和異常
- 效能追蹤
- SQL查詢記錄（可配置）

## 擴展性

可以輕鬆擴展支援新的資料庫或存取方式：

1. 實作 IDataAccessStrategy 介面
2. 實作對應的 QueryBuilder
3. 註冊新的策略

## 注意事項

- 確保正確配置連線字串
- 在生產環境中適當配置日誌級別
- 根據需求選擇適當的資料存取策略
- 大量資料操作時注意使用批次處理
