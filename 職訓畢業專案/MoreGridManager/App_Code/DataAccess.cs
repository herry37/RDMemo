using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;

/// <summary>
/// DataAccess 的摘要描述
/// </summary>
public class DataAccess
{

		/// <summary>
        /// Retrieve an individual record from database.
        /// </summary>
        /// <param name="id">Record id</param>
        /// <returns>A found record</returns>
        public Article GetArticle(int id)
        {
            //設定搜尋語法
            List<Article> articles = QueryList("select * from 商品類別 left join 商品 on 商品類別.商品類別代碼=商品.商品類別代碼 where 商品.商品編號=" + id.ToString());

            // Only return the first record.
            if (articles.Count > 0)
            {
                return articles[0];
            }
            return null;
        }

        /// <summary>
        /// Retrieve all records from database.
        /// </summary>
        /// <returns></returns>
        public List<Article> GetAll()
        {
            return QueryList("select * from 商品類別 left join 商品 on 商品類別.商品類別代碼=商品.商品類別代碼");
        }

        /// <summary>
        /// Search records from database.
        /// </summary>
        /// <param name="keywords">the list of keywords</param>
        /// <returns>all found records</returns>
        public List<Article> Search(List<string> keywords)
        {
            // Generate a complex Sql command.
            StringBuilder sqlBuilder = new StringBuilder();
            sqlBuilder.Append("select * from 商品類別 left join 商品 on 商品類別.商品類別代碼=商品.商品類別代碼 where ");
            foreach (string item in keywords)
            {
                sqlBuilder.AppendFormat("([商品名稱] like '%{0}%' or [單價] like '%{0}%') and ", item);
            }

            // Remove unnecessary string " and " at the end of the command.
            string sql = sqlBuilder.ToString(0, sqlBuilder.Length - 5);

            return QueryList(sql);
        }

        #region Helpers

        /// <summary>
        /// Create a connected SqlCommand object.
        /// </summary>
        /// <param name="cmdText">Command text</param>
        /// <returns>SqlCommand object</returns>
        protected SqlCommand GenerateSqlCommand(string cmdText)
        {
            // Read Connection String from web.config file.
            SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["C501_04ConnectionString"].ConnectionString);
            SqlCommand cmd = new SqlCommand(cmdText, conn);
            cmd.Connection.Open();
            return cmd;
        }

        /// <summary>
        /// Create an Article object from a SqlDataReader object.
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        protected Article ReadArticle(SqlDataReader reader)
        {
            Article article = new Article();

            //設定搜尋的值
            article.id = (int)reader["商品編號"];
            article.name = (string)reader["商品名稱"];
            article.picture = (string)reader["商品大圖片1"];
            article.picture1 = (string)reader["商品小圖片"];
            return article;
        }

        /// <summary>
        /// Excute a Sql command.
        /// </summary>
        /// <param name="cmdText">Command text</param>
        /// <returns></returns>
        protected List<Article> QueryList(string cmdText)
        {
            List<Article> articles = new List<Article>();

            SqlCommand cmd = GenerateSqlCommand(cmdText);
            using (cmd.Connection)
            {
                SqlDataReader reader = cmd.ExecuteReader();

                // Transform records to a list.
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        articles.Add(ReadArticle(reader));
                    }
                }
            }
            return articles;
        }

        #endregion
	
}