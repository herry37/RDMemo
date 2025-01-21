-- 初始化系統設定
INSERT INTO [SystemSettings] 
([Id], [Category], [Key], [Value], [Description], [CreatedAt], [UpdatedAt])
VALUES 
-- 系統基本設定
(NEWID(), 'System', 'SystemName', '後台管理系統', '系統名稱', GETUTCDATE(), GETUTCDATE()),
(NEWID(), 'System', 'Version', '1.0.0', '系統版本', GETUTCDATE(), GETUTCDATE()),
(NEWID(), 'System', 'MaintenanceMode', 'false', '維護模式開關', GETUTCDATE(), GETUTCDATE()),
(NEWID(), 'System', 'ApiVersion', '1', 'API版本', GETUTCDATE(), GETUTCDATE()),

-- 安全性設定
(NEWID(), 'Security', 'PasswordPolicy', '{
    "MinLength": 8,
    "RequireDigit": true,
    "RequireLowercase": true,
    "RequireUppercase": true,
    "RequireSpecialChar": true,
    "PasswordExpirationDays": 90,
    "PreventPasswordReuse": 5
}', '密碼政策', GETUTCDATE(), GETUTCDATE()),
(NEWID(), 'Security', 'LoginAttempts', '5', '登入嘗試次數限制', GETUTCDATE(), GETUTCDATE()),
(NEWID(), 'Security', 'LockoutDuration', '30', '帳號鎖定時間(分鐘)', GETUTCDATE(), GETUTCDATE()),
(NEWID(), 'Security', 'SessionTimeout', '60', '工作階段逾時時間(分鐘)', GETUTCDATE(), GETUTCDATE()),

-- 郵件設定
(NEWID(), 'Email', 'SmtpSettings', '{
    "Server": "smtp.example.com",
    "Port": 587,
    "UseSsl": true,
    "SenderName": "系統管理員",
    "SenderEmail": "admin@example.com"
}', 'SMTP伺服器設定', GETUTCDATE(), GETUTCDATE()),

-- 檔案上傳設定
(NEWID(), 'Upload', 'AllowedFileTypes', '.jpg,.jpeg,.png,.pdf,.doc,.docx,.xls,.xlsx', '允許的檔案類型', GETUTCDATE(), GETUTCDATE()),
(NEWID(), 'Upload', 'MaxFileSize', '10485760', '最大檔案大小(位元組)', GETUTCDATE(), GETUTCDATE());

-- 初始化管理員帳號
DECLARE @AdminId UNIQUEIDENTIFIER = NEWID();
DECLARE @AdminRoleId UNIQUEIDENTIFIER = NEWID();

-- 建立管理員角色
INSERT INTO [Roles]
([Id], [Name], [Description], [IsSystem], [CreatedAt], [UpdatedAt])
VALUES
(@AdminRoleId, 'Administrator', '系統管理員', 1, GETUTCDATE(), GETUTCDATE());

-- 建立管理員帳號
INSERT INTO [Users]
([Id], [Username], [Email], [PasswordHash], [Salt], [IsActive], [CreatedAt], [UpdatedAt])
VALUES
(
    @AdminId, 
    'admin',
    'admin@example.com',
    -- 預設密碼：Admin@123 (請在正式環境中更改)
    'AQAAAAIAAYagAAAAELPvxWwX6P6bqKyW+9MnJGjnGxN6mFP4JyZfLjjHh+0JS+RTKH6I1HJuqxE8zTGeqw==',
    'randomsalt',
    1,
    GETUTCDATE(),
    GETUTCDATE()
);

-- 設定管理員角色
INSERT INTO [UserRoles]
([UserId], [RoleId], [CreatedAt])
VALUES
(@AdminId, @AdminRoleId, GETUTCDATE());

-- 初始化權限
INSERT INTO [Permissions]
([Id], [Code], [Name], [Description], [CreatedAt])
VALUES
(NEWID(), 'USER_MANAGE', '使用者管理', '使用者的新增、修改、刪除等操作', GETUTCDATE()),
(NEWID(), 'ROLE_MANAGE', '角色管理', '角色的新增、修改、刪除等操作', GETUTCDATE()),
(NEWID(), 'SYSTEM_SETTINGS', '系統設定', '系統參數設定管理', GETUTCDATE()),
(NEWID(), 'AUDIT_LOG_VIEW', '稽核日誌查看', '查看系統操作日誌', GETUTCDATE()),
(NEWID(), 'BACKUP_MANAGE', '備份管理', '資料庫備份與還原', GETUTCDATE());

-- 將所有權限授予管理員角色
INSERT INTO [RolePermissions]
([RoleId], [PermissionId], [CreatedAt])
SELECT 
    @AdminRoleId,
    [Id],
    GETUTCDATE()
FROM [Permissions];

-- 初始化稽核日誌類型
INSERT INTO [AuditLogTypes]
([Id], [Code], [Name], [Description], [CreatedAt])
VALUES
(NEWID(), 'LOGIN', '登入', '使用者登入系統', GETUTCDATE()),
(NEWID(), 'LOGOUT', '登出', '使用者登出系統', GETUTCDATE()),
(NEWID(), 'CREATE', '新增', '新增資料', GETUTCDATE()),
(NEWID(), 'UPDATE', '修改', '修改資料', GETUTCDATE()),
(NEWID(), 'DELETE', '刪除', '刪除資料', GETUTCDATE()),
(NEWID(), 'EXPORT', '匯出', '匯出資料', GETUTCDATE()),
(NEWID(), 'IMPORT', '匯入', '匯入資料', GETUTCDATE()),
(NEWID(), 'BACKUP', '備份', '系統備份', GETUTCDATE()),
(NEWID(), 'RESTORE', '還原', '系統還原', GETUTCDATE()),
(NEWID(), 'CONFIG', '設定', '系統設定變更', GETUTCDATE()); 