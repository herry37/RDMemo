-- 建立資料庫
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'Test')
BEGIN
    CREATE DATABASE [Test];
END
GO

USE [Test];
GO

-- 建立登入帳號
IF NOT EXISTS (SELECT * FROM sys.server_principals WHERE name = 'test')
BEGIN
    CREATE LOGIN [test] WITH PASSWORD = 'test741852';
END
GO

-- 建立資料庫使用者
IF NOT EXISTS (SELECT * FROM sys.database_principals WHERE name = 'test')
BEGIN
    CREATE USER [test] FOR LOGIN [test];
    
    -- 授予資料庫角色
    ALTER ROLE db_owner ADD MEMBER [test];
    -- 或者使用以下更細緻的權限控制：
    -- GRANT SELECT, INSERT, UPDATE, DELETE ON SCHEMA::dbo TO [test];
    -- GRANT EXECUTE ON SCHEMA::dbo TO [test];
END
GO

-- 確認 SQL Server 允許 SQL 認證登入
EXEC sp_configure 'show advanced options', 1;
GO
RECONFIGURE;
GO
EXEC sp_configure 'sql server authentication', 1;
GO
RECONFIGURE;
GO 