using Grocery.Core.Data.Helpers;
using Grocery.Core.Interfaces.Repositories;
using Grocery.Core.Models;
using Microsoft.Data.Sqlite;

namespace Grocery.Core.Data.Repositories
{
    public class ProductRepository : DatabaseConnection, IProductRepository
    {
        public ProductRepository()
        {
            CreateTable(@"CREATE TABLE IF NOT EXISTS Product (
                            [Id] INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
                            [Name] NVARCHAR(100) NOT NULL,
                            [Stock] INTEGER NOT NULL,
                            [ShelfLife] DATE NOT NULL,
                            [Price] DECIMAL(10, 2) NOT NULL)");

            List<string> insertQueries = [
                @"INSERT OR IGNORE INTO Product(Id, Name, Stock, ShelfLife, Price) VALUES(1, 'Melk', 300, '2025-09-25', 0.95)",
                @"INSERT OR IGNORE INTO Product(Id, Name, Stock, ShelfLife, Price) VALUES(2, 'Kaas', 100, '2025-09-30', 7.98)",
                @"INSERT OR IGNORE INTO Product(Id, Name, Stock, ShelfLife, Price) VALUES(3, 'Brood', 400, '2025-09-12', 2.19)",
                @"INSERT OR IGNORE INTO Product(Id, Name, Stock, ShelfLife, Price) VALUES(4, 'Cornflakes', 0, '2025-12-31', 1.48)"
            ];
            InsertMultipleWithTransaction(insertQueries);
        }

        public List<Product> GetAll()
        {
            List<Product> products = [];
            string query = "SELECT Id, Name, Stock, ShelfLife, Price FROM Product";

            OpenConnection();
            using (SqliteCommand command = new(query, Connection))
            using (SqliteDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    products.Add(new Product(
                        reader.GetInt32(0),                         // Id
                        reader.GetString(1),                        // Name
                        reader.GetInt32(2),                         // Stock
                        DateOnly.FromDateTime(reader.GetDateTime(3)), // ShelfLife
                        reader.GetDecimal(4)                        // Price
                    ));
                }
            }
            CloseConnection();
            return products;
        }

        public Product? Get(int id)
        {
            Product? product = null;
            string query = "SELECT Id, Name, Stock, ShelfLife, Price FROM Product WHERE Id = @Id";

            OpenConnection();
            using (SqliteCommand command = new(query, Connection))
            {
                command.Parameters.AddWithValue("@Id", id);
                using (SqliteDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        product = new Product(
                            reader.GetInt32(0),
                            reader.GetString(1),
                            reader.GetInt32(2),
                            DateOnly.FromDateTime(reader.GetDateTime(3)),
                            reader.GetDecimal(4)
                        );
                    }
                }
            }
            CloseConnection();
            return product;
        }

        public Product Add(Product item)
        {
            string query = @"INSERT INTO Product(Name, Stock, ShelfLife, Price) 
                             VALUES(@Name, @Stock, @ShelfLife, @Price); SELECT last_insert_rowid();";

            OpenConnection();
            using (SqliteCommand command = new(query, Connection))
            {
                command.Parameters.AddWithValue("@Name", item.Name);
                command.Parameters.AddWithValue("@Stock", item.Stock);
                command.Parameters.AddWithValue("@ShelfLife", item.ShelfLife);
                command.Parameters.AddWithValue("@Price", item.Price);

                item.Id = Convert.ToInt32(command.ExecuteScalar());
            }
            CloseConnection();
            return item;
        }

        public Product? Delete(Product item)
        {
            string query = "DELETE FROM Product WHERE Id = @Id";

            OpenConnection();
            using (SqliteCommand command = new(query, Connection))
            {
                command.Parameters.AddWithValue("@Id", item.Id);
                int affected = command.ExecuteNonQuery();
                if (affected == 0)
                {
                    CloseConnection();
                    return null;
                }
            }
            CloseConnection();
            return item;
        }

        public Product? Update(Product item)
        {
            string query = @"UPDATE Product 
                             SET Name = @Name, Stock = @Stock, ShelfLife = @ShelfLife, Price = @Price 
                             WHERE Id = @Id";

            OpenConnection();
            using (SqliteCommand command = new(query, Connection))
            {
                command.Parameters.AddWithValue("@Name", item.Name);
                command.Parameters.AddWithValue("@Stock", item.Stock);
                command.Parameters.AddWithValue("@ShelfLife", item.ShelfLife);
                command.Parameters.AddWithValue("@Price", item.Price);
                command.Parameters.AddWithValue("@Id", item.Id);

                int affected = command.ExecuteNonQuery();
                if (affected == 0)
                {
                    CloseConnection();
                    return null;
                }
            }
            CloseConnection();
            return item;
        }
    }
}
