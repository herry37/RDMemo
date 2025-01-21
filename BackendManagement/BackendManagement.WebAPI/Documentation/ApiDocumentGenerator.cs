namespace BackendManagement.WebAPI.Documentation;

/// <summary>
/// API文件產生器
/// </summary>
public class ApiDocumentGenerator
{
    private readonly IWebHostEnvironment _environment;
    private readonly IConfiguration _configuration;

    public ApiDocumentGenerator(
        IWebHostEnvironment environment,
        IConfiguration configuration)
    {
        _environment = environment;
        _configuration = configuration;
    }

    /// <summary>
    /// 產生API文件
    /// </summary>
    public async Task GenerateAsync(string outputPath)
    {
        var document = new
        {
            Info = new
            {
                Title = "後台管理系統 API 文件",
                Version = "1.0",
                Description = "提供後台管理系統的API使用說明",
                Environment = _environment.EnvironmentName,
                GeneratedAt = DateTime.UtcNow
            },
            Authentication = new
            {
                Type = "Bearer",
                Description = "使用JWT進行身分驗證",
                Example = "Bearer {token}"
            },
            ApiEndpoints = GetApiEndpoints(),
            Models = GetModelDefinitions(),
            ErrorCodes = GetErrorCodes(),
            RateLimits = GetRateLimits(),
            Dependencies = GetDependencies()
        };

        var json = JsonSerializer.Serialize(document, new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await File.WriteAllTextAsync(outputPath, json);
    }

    private object GetApiEndpoints()
    {
        return new
        {
            Auth = new
            {
                Login = new
                {
                    Url = "/api/auth/login",
                    Method = "POST",
                    Description = "使用者登入",
                    Request = new
                    {
                        Username = "string",
                        Password = "string"
                    },
                    Response = new
                    {
                        AccessToken = "string",
                        RefreshToken = "string",
                        ExpiresIn = "int"
                    }
                }
            },
            Users = new
            {
                GetProfile = new
                {
                    Url = "/api/users/profile",
                    Method = "GET",
                    Description = "取得使用者資料",
                    Authorization = "Required"
                }
            },
            DisasterRecovery = new
            {
                CreateRecoveryPoint = new
                {
                    Url = "/api/disaster-recovery/recovery-points",
                    Method = "POST",
                    Description = "建立復原點",
                    Authorization = "Required",
                    Roles = new[] { "Administrator" }
                }
            }
        };
    }

    private object GetModelDefinitions()
    {
        return new
        {
            User = new
            {
                Properties = new
                {
                    Id = "Guid",
                    Username = "string",
                    Email = "string",
                    IsActive = "bool",
                    Roles = "string[]"
                }
            },
            RecoveryPoint = new
            {
                Properties = new
                {
                    Id = "Guid",
                    Description = "string",
                    Status = "enum",
                    CreatedAt = "DateTime"
                }
            }
        };
    }

    private object GetErrorCodes()
    {
        return new Dictionary<string, string>
        {
            { "400", "請求參數錯誤" },
            { "401", "未經授權的存取" },
            { "403", "存取被拒絕" },
            { "404", "資源不存在" },
            { "429", "請求過於頻繁" },
            { "500", "伺服器內部錯誤" }
        };
    }

    private object GetRateLimits()
    {
        return new
        {
            Default = new
            {
                Limit = 100,
                Window = "1分鐘"
            },
            Auth = new
            {
                Limit = 5,
                Window = "1分鐘"
            }
        };
    }

    private object GetDependencies()
    {
        return new
        {
            Database = new
            {
                Type = _configuration["DatabaseType"],
                Version = "Latest"
            },
            Redis = new
            {
                Version = "Alpine"
            },
            Elasticsearch = new
            {
                Version = "7.x"
            }
        };
    }
} 