/*
刪除資料表腳本
注意：需要按照外鍵關聯的順序由內而外刪除
*/

-- 先刪除所有外鍵約束
DECLARE @sql NVARCHAR(MAX) = N'';

SELECT @sql += N'
    ALTER TABLE ' + QUOTENAME(OBJECT_SCHEMA_NAME(parent_object_id))
    + '.' + QUOTENAME(OBJECT_NAME(parent_object_id)) 
    + ' DROP CONSTRAINT ' + QUOTENAME(name) + ';'
FROM sys.foreign_keys
WHERE referenced_object_id > 0;

EXEC sp_executesql @sql;
GO

-- 1. 系統設定相關
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'SystemSettings')
    DROP TABLE [SystemSettings];
GO

-- 2. 備份記錄相關
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'BackupLogs')
    DROP TABLE [BackupLogs];
GO

-- 3. 稽核記錄相關
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'AuditLogs')
    DROP TABLE [AuditLogs];
GO

-- 4. 系統日誌相關
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'SystemLogs')
    DROP TABLE [SystemLogs];
GO

-- 5. 角色權限關聯
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'RolePermissions')
    DROP TABLE [RolePermissions];
GO

-- 6. 使用者角色關聯
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'UserRoles')
    DROP TABLE [UserRoles];
GO

-- 7. 權限資料表
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'Permissions')
    DROP TABLE [Permissions];
GO

-- 8. 角色資料表
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'Roles')
    DROP TABLE [Roles];
GO

-- 9. 使用者資料表
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'Users')
    DROP TABLE [Users];
GO

-- 10. 最後刪除租戶資料表
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'Tenants')
    DROP TABLE [Tenants];
GO

-- 3.1 稽核日誌類型
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'AuditLogTypes')
    DROP TABLE [AuditLogTypes];
GO 