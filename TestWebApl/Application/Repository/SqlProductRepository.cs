using System.Data.SqlClient;
using TestWebApl.Entitie;
using TestWebApl.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace TestWebApl.Application.Repository
{
    public class SqlProductRepository : IProductRepository
    {
        private readonly string _connectionString;

        public SqlProductRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            var products = new List<Product>();
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand("SELECT ID, Name, Quantity FROM Products", connection))
                {
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            products.Add(new Product
                            {
                                ID = reader.GetInt32(0),
                                Name = reader.GetString(1),
                                Quantity = reader.IsDBNull(2) ? null : reader.GetInt32(2)
                            });
                        }
                    }
                }
            }
            return products;
        }

        public async Task<Product> GetByIdAsync(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand("SELECT ID, Name, Quantity FROM Products WHERE ID = @Id", connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            return new Product
                            {
                                ID = reader.GetInt32(0),
                                Name = reader.GetString(1),
                                Quantity = reader.IsDBNull(2) ? null : reader.GetInt32(2)
                            };
                        }
                    }
                }
            }
            return null;
        }

        public async Task AddAsync(Product product)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand("INSERT INTO Products (Name, Quantity) VALUES (@Name, @Quantity); SELECT SCOPE_IDENTITY();", connection))
                {
                    command.Parameters.AddWithValue("@Name", product.Name);
                    command.Parameters.AddWithValue("@Quantity", product.Quantity.HasValue ? (object)product.Quantity.Value : DBNull.Value);

                    var id = Convert.ToInt32(await command.ExecuteScalarAsync());
                    product.ID = id;
                }
            }
        }

        public async Task UpdateAsync(Product product)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand("UPDATE Products SET Name = @Name, Quantity = @Quantity WHERE ID = @Id", connection))
                {
                    command.Parameters.AddWithValue("@Id", product.ID);
                    command.Parameters.AddWithValue("@Name", product.Name);
                    command.Parameters.AddWithValue("@Quantity", product.Quantity.HasValue ? (object)product.Quantity.Value : DBNull.Value);

                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task DeleteAsync(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand("DELETE FROM Products WHERE ID = @Id", connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    await command.ExecuteNonQueryAsync();
                }
            }
        }
    }
}
