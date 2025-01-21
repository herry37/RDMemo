/*
租戶資料表
用途：支援多租戶架構，儲存租戶資訊
*/
CREATE TABLE [Tenants] (
    [Id] UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(), -- 租戶唯一識別碼
    [Name] NVARCHAR(100) NOT NULL UNIQUE,             -- 租戶名稱，不可重複
    [ConnectionString] NVARCHAR(MAX) NOT NULL,        -- 租戶專用資料庫連線字串
    [IsActive] BIT NOT NULL DEFAULT 1,                -- 租戶狀態：1啟用、0停用
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(), -- 建立時間
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(), -- 更新時間
    [CreatedBy] UNIQUEIDENTIFIER NULL,                -- 建立者ID
    [UpdatedBy] UNIQUEIDENTIFIER NULL                 -- 更新者ID
);
GO

EXEC sp_addextendedproperty 
    @name = N'MS_Description',
    @value = N'支援多租戶架構，儲存租戶資訊',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'Tenants';
GO

/*
使用者資料表
用途：儲存系統使用者的基本資料
*/
CREATE TABLE [Users] (
    [Id] UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(), -- 使用者唯一識別碼
    [Username] NVARCHAR(50) NOT NULL UNIQUE,           -- 使用者帳號，不可重複
    [Email] NVARCHAR(100) NOT NULL UNIQUE,            -- 電子郵件，不可重複
    [PasswordHash] NVARCHAR(MAX) NOT NULL,            -- 密碼雜湊值
    [Salt] NVARCHAR(MAX) NOT NULL,                    -- 密碼加密鹽值
    [IsActive] BIT NOT NULL DEFAULT 1,                -- 帳號狀態：1啟用、0停用
    [LastLoginTime] DATETIME2 NULL,                   -- 最後登入時間
    [TenantId] UNIQUEIDENTIFIER NULL,                 -- 租戶ID（NULL表示系統管理員）
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(), -- 建立時間
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(), -- 更新時間
    [CreatedBy] UNIQUEIDENTIFIER NULL,                -- 建立者ID
    [UpdatedBy] UNIQUEIDENTIFIER NULL,                -- 更新者ID
    FOREIGN KEY ([TenantId]) REFERENCES [Tenants]([Id]) -- 外鍵參考租戶表
);
GO

-- 更新 Tenants 表格的外鍵
ALTER TABLE [Tenants]
ADD FOREIGN KEY ([CreatedBy]) REFERENCES [Users]([Id]),
    FOREIGN KEY ([UpdatedBy]) REFERENCES [Users]([Id]);
GO

/*
角色資料表
用途：定義系統中的角色
*/
CREATE TABLE [Roles] (
    [Id] UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(), -- 角色唯一識別碼
    [Name] NVARCHAR(50) NOT NULL UNIQUE,              -- 角色名稱，不可重複
    [Description] NVARCHAR(200) NULL,                 -- 角色描述
    [IsSystem] BIT NOT NULL DEFAULT 0,                -- 是否為系統內建角色
    [TenantId] UNIQUEIDENTIFIER NULL,                 -- 租戶ID（NULL表示全域角色）
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(), -- 建立時間
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(), -- 更新時間
    [CreatedBy] UNIQUEIDENTIFIER NULL,                -- 建立者ID
    [UpdatedBy] UNIQUEIDENTIFIER NULL,                -- 更新者ID
    FOREIGN KEY ([TenantId]) REFERENCES [Tenants]([Id]), -- 外鍵參考租戶表
    FOREIGN KEY ([CreatedBy]) REFERENCES [Users]([Id]), -- 外鍵參考使用者表
    FOREIGN KEY ([UpdatedBy]) REFERENCES [Users]([Id])  -- 外鍵參考使用者表
);
GO

EXEC sp_addextendedproperty 
    @name = N'MS_Description',
    @value = N'定義系統中的角色',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'Roles';
GO

EXEC sp_addextendedproperty @name = N'MS_Description', @value = N'角色唯一識別碼',
    @level0type = N'SCHEMA', @level0name = N'dbo',
    @level1type = N'TABLE',  @level1name = N'Roles',
    @level2type = N'COLUMN', @level2name = N'Id';
GO

EXEC sp_addextendedproperty @name = N'MS_Description', @value = N'角色名稱，不可重複',
    @level0type = N'SCHEMA', @level0name = N'dbo',
    @level1type = N'TABLE',  @level1name = N'Roles',
    @level2type = N'COLUMN', @level2name = N'Name';
GO

EXEC sp_addextendedproperty @name = N'MS_Description', @value = N'角色描述',
    @level0type = N'SCHEMA', @level0name = N'dbo',
    @level1type = N'TABLE',  @level1name = N'Roles',
    @level2type = N'COLUMN', @level2name = N'Description';
GO

EXEC sp_addextendedproperty @name = N'MS_Description', @value = N'是否為系統內建角色',
    @level0type = N'SCHEMA', @level0name = N'dbo',
    @level1type = N'TABLE',  @level1name = N'Roles',
    @level2type = N'COLUMN', @level2name = N'IsSystem';
GO

EXEC sp_addextendedproperty @name = N'MS_Description', @value = N'租戶ID（NULL表示全域角色）',
    @level0type = N'SCHEMA', @level0name = N'dbo',
    @level1type = N'TABLE',  @level1name = N'Roles',
    @level2type = N'COLUMN', @level2name = N'TenantId';
GO

EXEC sp_addextendedproperty @name = N'MS_Description', @value = N'建立時間',
    @level0type = N'SCHEMA', @level0name = N'dbo',
    @level1type = N'TABLE',  @level1name = N'Roles',
    @level2type = N'COLUMN', @level2name = N'CreatedAt';
GO

EXEC sp_addextendedproperty @name = N'MS_Description', @value = N'更新時間',
    @level0type = N'SCHEMA', @level0name = N'dbo',
    @level1type = N'TABLE',  @level1name = N'Roles',
    @level2type = N'COLUMN', @level2name = N'UpdatedAt';
GO

EXEC sp_addextendedproperty @name = N'MS_Description', @value = N'建立者ID',
    @level0type = N'SCHEMA', @level0name = N'dbo',
    @level1type = N'TABLE',  @level1name = N'Roles',
    @level2type = N'COLUMN', @level2name = N'CreatedBy';
GO

EXEC sp_addextendedproperty @name = N'MS_Description', @value = N'更新者ID',
    @level0type = N'SCHEMA', @level0name = N'dbo',
    @level1type = N'TABLE',  @level1name = N'Roles',
    @level2type = N'COLUMN', @level2name = N'UpdatedBy';
GO

/*
權限資料表
用途：定義系統功能權限
*/
CREATE TABLE [Permissions] (
    [Id] UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(), -- 權限唯一識別碼
    [Code] NVARCHAR(50) NOT NULL UNIQUE,              -- 權限代碼，不可重複
    [Name] NVARCHAR(100) NOT NULL,                    -- 權限名稱
    [Description] NVARCHAR(200) NULL,                 -- 權限描述
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(), -- 建立時間
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(), -- 更新時間
    [CreatedBy] UNIQUEIDENTIFIER NULL,                -- 建立者ID
    [UpdatedBy] UNIQUEIDENTIFIER NULL,                -- 更新者ID
    FOREIGN KEY ([CreatedBy]) REFERENCES [Users]([Id]), -- 外鍵參考使用者表
    FOREIGN KEY ([UpdatedBy]) REFERENCES [Users]([Id])  -- 外鍵參考使用者表
);
GO

EXEC sp_addextendedproperty @name = N'MS_Description', @value = N'權限代碼',
    @level0type = N'SCHEMA', @level0name = N'dbo',
    @level1type = N'TABLE',  @level1name = N'Permissions',
    @level2type = N'COLUMN', @level2name = N'Code';
GO

EXEC sp_addextendedproperty @name = N'MS_Description', @value = N'權限名稱',
    @level0type = N'SCHEMA', @level0name = N'dbo',
    @level1type = N'TABLE',  @level1name = N'Permissions',
    @level2type = N'COLUMN', @level2name = N'Name';
GO

/*
使用者角色關聯表
用途：建立使用者與角色的多對多關聯
*/
CREATE TABLE [UserRoles] (
    [UserId] UNIQUEIDENTIFIER NOT NULL,               -- 使用者ID
    [RoleId] UNIQUEIDENTIFIER NOT NULL,               -- 角色ID
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(), -- 建立時間
    [CreatedBy] UNIQUEIDENTIFIER NULL,                -- 建立者ID
    PRIMARY KEY ([UserId], [RoleId]),                 -- 複合主鍵
    FOREIGN KEY ([UserId]) REFERENCES [Users]([Id]),  -- 外鍵參考使用者表
    FOREIGN KEY ([RoleId]) REFERENCES [Roles]([Id]),  -- 外鍵參考角色表
    FOREIGN KEY ([CreatedBy]) REFERENCES [Users]([Id]) -- 外鍵參考使用者表
);
GO

EXEC sp_addextendedproperty 
    @name = N'MS_Description',
    @value = N'使用者與角色的關聯資料表',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'UserRoles';
GO

/*
角色權限關聯表
用途：建立角色與權限的多對多關聯
*/
CREATE TABLE [RolePermissions] (
    [RoleId] UNIQUEIDENTIFIER NOT NULL,               -- 角色ID
    [PermissionId] UNIQUEIDENTIFIER NOT NULL,         -- 權限ID
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(), -- 建立時間
    [CreatedBy] UNIQUEIDENTIFIER NULL,                -- 建立者ID
    PRIMARY KEY ([RoleId], [PermissionId]),           -- 複合主鍵
    FOREIGN KEY ([RoleId]) REFERENCES [Roles]([Id]),  -- 外鍵參考角色表
    FOREIGN KEY ([PermissionId]) REFERENCES [Permissions]([Id]), -- 外鍵參考權限表
    FOREIGN KEY ([CreatedBy]) REFERENCES [Users]([Id]) -- 外鍵參考使用者表
);
GO

EXEC sp_addextendedproperty 
    @name = N'MS_Description',
    @value = N'角色與權限的關聯資料表',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'RolePermissions';
GO

/*
系統日誌資料表
用途：記錄系統運行時的各種日誌
*/
CREATE TABLE [SystemLogs] (
    [Id] UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(), -- 日誌唯一識別碼
    [Level] NVARCHAR(20) NOT NULL,                    -- 日誌等級（如：Info、Warning、Error）
    [Message] NVARCHAR(MAX) NOT NULL,                 -- 日誌訊息
    [Exception] NVARCHAR(MAX) NULL,                   -- 異常詳細資訊
    [Source] NVARCHAR(200) NULL,                      -- 日誌來源
    [TenantId] UNIQUEIDENTIFIER NULL,                 -- 租戶ID
    [UserId] UNIQUEIDENTIFIER NULL,                   -- 相關使用者ID
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(), -- 建立時間
    FOREIGN KEY ([TenantId]) REFERENCES [Tenants]([Id]), -- 外鍵參考租戶表
    FOREIGN KEY ([UserId]) REFERENCES [Users]([Id])   -- 外鍵參考使用者表
);

/*
操作記錄資料表
用途：記錄資料異動歷程，用於稽核追蹤
*/
CREATE TABLE [AuditLogs] (
    [Id] UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(), -- 記錄唯一識別碼
    [TableName] NVARCHAR(100) NOT NULL,               -- 被異動的資料表名稱
    [ActionType] NVARCHAR(20) NOT NULL,               -- 操作類型（新增、修改、刪除）
    [EntityId] NVARCHAR(100) NOT NULL,                -- 被異動資料的ID
    [OldValues] NVARCHAR(MAX) NULL,                   -- 異動前的值（JSON格式）
    [NewValues] NVARCHAR(MAX) NULL,                   -- 異動後的值（JSON格式）
    [TenantId] UNIQUEIDENTIFIER NULL,                 -- 租戶ID
    [UserId] UNIQUEIDENTIFIER NULL,                   -- 操作者ID
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(), -- 建立時間
    FOREIGN KEY ([TenantId]) REFERENCES [Tenants]([Id]), -- 外鍵參考租戶表
    FOREIGN KEY ([UserId]) REFERENCES [Users]([Id])   -- 外鍵參考使用者表
);

EXEC sp_addextendedproperty @name = N'MS_Description', @value = N'操作類型',
    @level0type = N'SCHEMA', @level0name = N'dbo',
    @level1type = N'TABLE',  @level1name = N'AuditLogs',
    @level2type = N'COLUMN', @level2name = N'ActionType';
GO

EXEC sp_addextendedproperty @name = N'MS_Description', @value = N'操作前的資料',
    @level0type = N'SCHEMA', @level0name = N'dbo',
    @level1type = N'TABLE',  @level1name = N'AuditLogs',
    @level2type = N'COLUMN', @level2name = N'OldValues';
GO

EXEC sp_addextendedproperty @name = N'MS_Description', @value = N'操作後的資料',
    @level0type = N'SCHEMA', @level0name = N'dbo',
    @level1type = N'TABLE',  @level1name = N'AuditLogs',
    @level2type = N'COLUMN', @level2name = N'NewValues';
GO

/*
備份記錄資料表
用途：記錄資料庫備份操作的歷程
*/
CREATE TABLE [BackupLogs] (
    [Id] UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(), -- 備份記錄唯一識別碼
    [BackupPath] NVARCHAR(500) NOT NULL,              -- 備份檔案路徑
    [Status] NVARCHAR(20) NOT NULL,                   -- 備份狀態（成功、失敗）
    [Description] NVARCHAR(500) NULL,                 -- 備份描述或備註
    [StartTime] DATETIME2 NOT NULL,                   -- 備份開始時間
    [EndTime] DATETIME2 NULL,                         -- 備份結束時間
    [TenantId] UNIQUEIDENTIFIER NULL,                 -- 租戶ID
    [CreatedBy] UNIQUEIDENTIFIER NULL,                -- 執行備份的使用者ID
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(), -- 建立時間
    FOREIGN KEY ([TenantId]) REFERENCES [Tenants]([Id]), -- 外鍵參考租戶表
    FOREIGN KEY ([CreatedBy]) REFERENCES [Users]([Id]) -- 外鍵參考使用者表
);

/*
系統設定資料表
用途：儲存系統各項參數設定
*/
CREATE TABLE [SystemSettings] (
    [Id] UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(), -- 設定唯一識別碼
    [Category] NVARCHAR(50) NOT NULL,                 -- 設定類別
    [Key] NVARCHAR(100) NOT NULL,                     -- 設定鍵值
    [Value] NVARCHAR(MAX) NOT NULL,                   -- 設定內容
    [Description] NVARCHAR(500) NULL,                 -- 設定描述
    [TenantId] UNIQUEIDENTIFIER NULL,                 -- 租戶ID（NULL表示全域設定）
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(), -- 建立時間
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(), -- 更新時間
    [CreatedBy] UNIQUEIDENTIFIER NULL,                -- 建立者ID
    [UpdatedBy] UNIQUEIDENTIFIER NULL,                -- 更新者ID
    FOREIGN KEY ([TenantId]) REFERENCES [Tenants]([Id]), -- 外鍵參考租戶表
    UNIQUE ([Category], [Key], [TenantId])            -- 確保每個租戶的設定鍵值不重複
);

EXEC sp_addextendedproperty @name = N'MS_Description', @value = N'設定類別',
    @level0type = N'SCHEMA', @level0name = N'dbo',
    @level1type = N'TABLE',  @level1name = N'SystemSettings',
    @level2type = N'COLUMN', @level2name = N'Category';
GO

EXEC sp_addextendedproperty @name = N'MS_Description', @value = N'設定鍵值',
    @level0type = N'SCHEMA', @level0name = N'dbo',
    @level1type = N'TABLE',  @level1name = N'SystemSettings',
    @level2type = N'COLUMN', @level2name = N'Key';
GO

/*
稽核日誌類型資料表
用途：定義系統中各種操作的稽核類型
*/
CREATE TABLE [AuditLogTypes] (
    [Id] UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(), -- 類型唯一識別碼
    [Code] NVARCHAR(50) NOT NULL UNIQUE,              -- 類型代碼，不可重複
    [Name] NVARCHAR(100) NOT NULL,                    -- 類型名稱
    [Description] NVARCHAR(200) NULL,                 -- 類型描述
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(), -- 建立時間
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(), -- 更新時間
    [CreatedBy] UNIQUEIDENTIFIER NULL,                -- 建立者ID
    [UpdatedBy] UNIQUEIDENTIFIER NULL,                -- 更新者ID
    FOREIGN KEY ([CreatedBy]) REFERENCES [Users]([Id]), -- 外鍵參考使用者表
    FOREIGN KEY ([UpdatedBy]) REFERENCES [Users]([Id])  -- 外鍵參考使用者表
);
GO

EXEC sp_addextendedproperty 
    @name = N'MS_Description',
    @value = N'定義系統中各種操作的稽核類型',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'AuditLogTypes';
GO 