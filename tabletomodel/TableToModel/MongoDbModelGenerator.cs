using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using MongoDB.Bson;
using MongoDB.Driver;

namespace TableToModel
{
    /// <summary>
    /// MongoDB Model 生成器
    /// </summary>
    public class MongoDbModelGenerator
    {
        private readonly MongoDbConnectionFactory _connectionFactory;

        public MongoDbModelGenerator()
        {
            _connectionFactory = new MongoDbConnectionFactory();
        }

        /// <summary>
        /// 生成 Model 類別
        /// </summary>
        public async Task<string> GenerateModelAsync(string server, string userId, string password,
            string database, string collectionName)
        {
            try
            {
                var connectionString = _connectionFactory.GetConnectionString(server, userId, password, database);
                var mongoDb = _connectionFactory.GetMongoDatabase(connectionString);
                var schema = await _connectionFactory.GetCollectionSchema(collectionName);

                if (schema == null)
                {
                    throw new TableNotFoundException($"找不到集合 {collectionName} 或集合為空");
                }

                return GenerateModelClass(collectionName, schema);
            }
            catch (Exception ex) when (ex is not TableNotFoundException)
            {
                throw new ModelGenerationException("生成 MongoDB Model 時發生錯誤", ex);
            }
        }

        /// <summary>
        /// 生成 Model 類別程式碼
        /// </summary>
        private string GenerateModelClass(string collectionName, BsonDocument schema)
        {
            var sb = new StringBuilder();

            // 加入必要的 using
            sb.AppendLine("using System;");
            sb.AppendLine("using MongoDB.Bson;");
            sb.AppendLine("using MongoDB.Bson.Serialization.Attributes;");
            sb.AppendLine("using System.ComponentModel;");
            sb.AppendLine();

            sb.AppendLine("namespace TableToModel.Models");
            sb.AppendLine("{");
            sb.AppendLine($"    /// <summary>");
            sb.AppendLine($"    /// {collectionName} 集合的實體類別");
            sb.AppendLine($"    /// </summary>");
            sb.AppendLine($"    [BsonCollection(\"{collectionName}\")]");
            sb.AppendLine($"    public class {ToPascalCase(collectionName)}");
            sb.AppendLine("    {");

            // 加入 _id 欄位
            sb.AppendLine("        [BsonId]");
            sb.AppendLine("        public ObjectId Id { get; set; }");
            sb.AppendLine();

            foreach (var field in schema["fields"].AsBsonArray)
            {
                var fieldName = field["_id"].AsString;
                var types = field["types"].AsBsonArray.Select(t => t.AsString).ToList();

                // 加入欄位註解
                sb.AppendLine($"        /// <summary>");
                sb.AppendLine($"        /// {fieldName}");
                sb.AppendLine($"        /// </summary>");

                // 加入 MongoDB 特性
                sb.AppendLine($"        [BsonElement(\"{fieldName}\")]");

                // 加入顯示名稱特性
                sb.AppendLine($"        [DisplayName(\"{fieldName}\")]");

                // 確定欄位類型
                string csharpType = GetCSharpType(types);
                sb.AppendLine($"        public {csharpType} {ToPascalCase(fieldName)} {{ get; set; }}");
                sb.AppendLine();
            }

            sb.AppendLine("    }");
            sb.AppendLine("}");

            return sb.ToString();
        }

        /// <summary>
        /// 將 MongoDB 類型轉換為 C# 類型
        /// </summary>
        private string GetCSharpType(List<string> types)
        {
            // 如果有多種類型，使用 dynamic
            if (types.Count > 1)
                return "dynamic";

            return types[0] switch
            {
                "double" => "double?",
                "string" => "string",
                "object" => "BsonDocument",
                "array" => "BsonArray",
                "binData" => "byte[]",
                "objectId" => "ObjectId",
                "bool" => "bool?",
                "date" => "DateTime?",
                "int" => "int?",
                "long" => "long?",
                "decimal" => "decimal?",
                _ => "object"
            };
        }

        /// <summary>
        /// 將字串轉換為 PascalCase
        /// </summary>
        private string ToPascalCase(string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            var words = input.Split(new[] { '_', '-', ' ' }, StringSplitOptions.RemoveEmptyEntries);
            return string.Join("", words.Select(word =>
                char.ToUpper(word[0]) + word.Substring(1).ToLower()));
        }
    }
}
