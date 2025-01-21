using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Dapper;

namespace TableToModel
{
    /// <summary>
    /// 資料表轉換為 Model 的轉換器
    /// </summary>
    public class ModelConverter
    {
        private readonly IDatabaseConnectionFactory _connectionFactory;
        private const int CommandTimeout = 30;  // 設定命令超時時間為30秒

        public ModelConverter()
        {
            _connectionFactory = new SqlDatabaseConnectionFactory();
        }

        /// <summary>
        /// 取得 Model
        /// </summary>
        /// <param name="dbType">資料庫類型</param>
        /// <param name="server">伺服器位址</param>
        /// <param name="userId">使用者名稱</param>
        /// <param name="password">密碼</param>
        /// <param name="database">資料庫名稱</param>
        /// <param name="tableName">資料表名稱</param>
        /// <returns>Model 類別程式碼</returns>
        /// <exception cref="ArgumentNullException">當任何必要參數為 null 或空白時拋出</exception>
        /// <exception cref="DatabaseConnectionException">當資料庫連線失敗時拋出</exception>
        /// <exception cref="TableNotFoundException">當找不到指定的資料表時拋出</exception>
        /// <exception cref="ModelGenerationException">當生成 Model 時發生錯誤時拋出</exception>
        public string GetModel(DatabaseConnectionFactory.DatabaseType dbType, string server, string userId,
            string password, string database, string tableName)
        {
            ValidateInputParameters(server, userId, password, database, tableName);

            try
            {
                var factory = dbType == DatabaseConnectionFactory.DatabaseType.MySQL
                    ? (IDatabaseConnectionFactory)new MySqlDatabaseConnectionFactory()
                    : new SqlDatabaseConnectionFactory();

                var connectionString = factory.GetConnectionString(server, userId, password, database);
                using var connection = factory.CreateConnection(connectionString);

                try
                {
                    connection.Open();
                }
                catch (Exception ex)
                {
                    throw new DatabaseConnectionException("無法連接到資料庫，請檢查連線資訊是否正確", ex);
                }

                var columns = GetTableColumns(connection, tableName);
                if (!columns.Any())
                {
                    throw new TableNotFoundException($"找不到資料表 {tableName} 或該資料表沒有欄位");
                }

                return GenerateModelClass(tableName, columns);
            }
            catch (Exception ex) when (ex is not DatabaseConnectionException
                                    && ex is not TableNotFoundException
                                    && ex is not ArgumentNullException)
            {
                throw new ModelGenerationException("生成 Model 時發生錯誤", ex);
            }
        }

        /// <summary>
        /// 驗證輸入參數
        /// </summary>
        private void ValidateInputParameters(string server, string userId, string password,
            string database, string tableName)
        {
            if (string.IsNullOrWhiteSpace(server))
                throw new ArgumentNullException(nameof(server), "伺服器位址不能為空");

            if (string.IsNullOrWhiteSpace(userId))
                throw new ArgumentNullException(nameof(userId), "使用者名稱不能為空");

            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentNullException(nameof(password), "密碼不能為空");

            if (string.IsNullOrWhiteSpace(database))
                throw new ArgumentNullException(nameof(database), "資料庫名稱不能為空");

            if (string.IsNullOrWhiteSpace(tableName))
                throw new ArgumentNullException(nameof(tableName), "資料表名稱不能為空");

            // 移除可能的特殊字元，防止 SQL 注入
            if (tableName.Any(c => !char.IsLetterOrDigit(c) && c != '_'))
                throw new ArgumentException("資料表名稱只能包含字母、數字和底線", nameof(tableName));
        }

        /// <summary>
        /// 取得資料表欄位資訊
        /// </summary>
        private IEnumerable<dynamic> GetTableColumns(IDbConnection connection, string tableName)
        {
            try
            {
                string sql = connection is SqlConnection
                    ? @"SELECT 
                        c.COLUMN_NAME as ColumnName,
                        c.DATA_TYPE as DataType,
                        c.IS_NULLABLE as IsNullable,
                        c.CHARACTER_MAXIMUM_LENGTH as MaxLength,
                        c.COLUMN_DEFAULT as DefaultValue,
                        COLUMNPROPERTY(OBJECT_ID(c.TABLE_SCHEMA + '.' + c.TABLE_NAME), c.COLUMN_NAME, 'IsIdentity') as IsIdentity
                    FROM INFORMATION_SCHEMA.COLUMNS c
                    WHERE c.TABLE_NAME = @TableName
                    ORDER BY c.ORDINAL_POSITION"
                    : @"SELECT 
                        COLUMN_NAME as ColumnName,
                        DATA_TYPE as DataType,
                        IS_NULLABLE as IsNullable,
                        CHARACTER_MAXIMUM_LENGTH as MaxLength,
                        COLUMN_DEFAULT as DefaultValue,
                        CASE WHEN EXTRA = 'auto_increment' THEN 1 ELSE 0 END as IsIdentity
                    FROM INFORMATION_SCHEMA.COLUMNS
                    WHERE TABLE_NAME = @TableName
                    AND TABLE_SCHEMA = DATABASE()
                    ORDER BY ORDINAL_POSITION";

                return connection.Query(sql, new { TableName = tableName },
                    commandTimeout: CommandTimeout);
            }
            catch (Exception ex)
            {
                throw new DatabaseConnectionException($"取得資料表 {tableName} 的欄位資訊時發生錯誤", ex);
            }
        }

        /// <summary>
        /// 生成 Model 類別程式碼
        /// </summary>
        private string GenerateModelClass(string tableName, IEnumerable<dynamic> columns)
        {
            var sb = new StringBuilder();

            // 加入常用的 using
            sb.AppendLine("using System;");
            sb.AppendLine("using System.ComponentModel;");
            sb.AppendLine("using System.ComponentModel.DataAnnotations;");
            sb.AppendLine("using System.ComponentModel.DataAnnotations.Schema;");
            sb.AppendLine();

            sb.AppendLine("namespace TableToModel.Models");
            sb.AppendLine("{");
            sb.AppendLine($"    /// <summary>");
            sb.AppendLine($"    /// {tableName} 資料表的實體類別");
            sb.AppendLine($"    /// </summary>");
            sb.AppendLine($"    [Table(\"{tableName}\")]");
            sb.AppendLine($"    public class {tableName}");
            sb.AppendLine("    {");

            foreach (var column in columns)
            {
                // 加入欄位註解
                sb.AppendLine($"        /// <summary>");
                sb.AppendLine($"        /// {column.ColumnName}");
                sb.AppendLine($"        /// </summary>");

                // 加入資料驗證特性
                if (column.IsNullable == "NO")
                {
                    sb.AppendLine("        [Required]");
                }

                if (column.MaxLength != null)
                {
                    sb.AppendLine($"        [MaxLength({column.MaxLength})]");
                }

                if (column.IsIdentity == 1)
                {
                    sb.AppendLine("        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]");
                }

                // 加入欄位名稱特性
                sb.AppendLine($"        [Column(\"{column.ColumnName}\")]");

                // 加入顯示名稱特性
                sb.AppendLine($"        [DisplayName(\"{column.ColumnName}\")]");

                // 生成屬性
                string dataType = GetCSharpType(column.DataType.ToString(), column.IsNullable == "YES");
                sb.AppendLine($"        public {dataType} {column.ColumnName} {{ get; set; }}");
                sb.AppendLine();
            }

            sb.AppendLine("    }");
            sb.AppendLine("}");

            return sb.ToString();
        }

        /// <summary>
        /// 取得 C# 資料類型
        /// </summary>
        private string GetCSharpType(string sqlType, bool isNullable)
        {
            var type = sqlType.ToLower() switch
            {
                "bigint" => "long",
                "binary" => "byte[]",
                "bit" => "bool",
                "char" => "string",
                "date" => "DateTime",
                "datetime" => "DateTime",
                "datetime2" => "DateTime",
                "datetimeoffset" => "DateTimeOffset",
                "decimal" => "decimal",
                "float" => "double",
                "image" => "byte[]",
                "int" => "int",
                "money" => "decimal",
                "nchar" => "string",
                "ntext" => "string",
                "numeric" => "decimal",
                "nvarchar" => "string",
                "real" => "float",
                "smalldatetime" => "DateTime",
                "smallint" => "short",
                "smallmoney" => "decimal",
                "text" => "string",
                "time" => "TimeSpan",
                "timestamp" => "byte[]",
                "tinyint" => "byte",
                "uniqueidentifier" => "Guid",
                "varbinary" => "byte[]",
                "varchar" => "string",
                _ => "object"
            };

            return type == "string" || type == "byte[]" || type == "object"
                ? type
                : isNullable ? type + "?" : type;
        }
    }
}
