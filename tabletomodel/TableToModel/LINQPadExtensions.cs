using System.Text;

namespace TableToModel
{
    public static class LINQPadExtensions
    {
        /// <summary>
        /// 欄位屬性類型對應表
        /// </summary>
        private static readonly Dictionary<Type, string> TypeAliases = new Dictionary<Type, string> {
        { typeof(int), "int" },
        { typeof(short), "short" },
        { typeof(byte), "byte" },
        { typeof(byte[]), "byte[]" },
        { typeof(long), "long" },
        { typeof(double), "double" },
        { typeof(decimal), "decimal" },
        { typeof(float), "float" },
        { typeof(bool), "bool" },
        { typeof(string), "string" }
    };

        /// <summary>
        /// 值是否可允許 NULL
        /// </summary>
        private static readonly HashSet<Type> NullableTypes = new HashSet<Type> {
        typeof(int),
        typeof(short),
        typeof(long),
        typeof(double),
        typeof(decimal),
        typeof(float),
        typeof(bool),
        typeof(DateTime)
    };

        /// <summary>
        /// 生成 C# 類的字符串
        /// </summary>
        /// <param name="connection">連線</param>
        /// <param name="sql">SQL 語句</param>
        /// <param name="className">類名</param>
        /// <returns>生成的類代碼</returns>
        public static string DumpClass(this IDbConnection connection, string sql, string className = null)
        {
            if (connection.State != ConnectionState.Open)
                connection.Open();

            var cmd = connection.CreateCommand();
            cmd.CommandText = sql;
            var reader = cmd.ExecuteReader(CommandBehavior.KeyInfo | CommandBehavior.SingleRow);

            var builder = new StringBuilder();
            do
            {
                if (reader.FieldCount <= 1) continue;

                var schema = reader.GetSchemaTable();
                foreach (DataRow row in schema.Rows)
                {
                    if (string.IsNullOrWhiteSpace(builder.ToString()))
                    {
                        // Table Name
                        var tableName = string.IsNullOrWhiteSpace(className) ? row["BaseTableName"] as string : className;
                        // Class Name
                        builder.AppendFormat("public class {0}{1}", tableName, Environment.NewLine);
                        builder.AppendLine("{");
                    }

                    var type = (Type)row["DataType"];

                    // 欄位屬性
                    var name = TypeAliases.ContainsKey(type) ? TypeAliases[type] : type.Name;

                    // 值是否允許為 NULL
                    var isNullable = (bool)row["AllowDBNull"] && NullableTypes.Contains(type);

                    // 欄位名
                    var columnName = (string)row["ColumnName"];

                    builder.AppendLine(string.Format("\tpublic {0}{1} {2} {{ get; set; }}", name, isNullable ? "?" : string.Empty, columnName));
                    builder.AppendLine();
                }

                builder.AppendLine("}");
                builder.AppendLine();
            } while (reader.NextResult());

            return builder.ToString();
        }
    }
}
