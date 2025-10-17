using Grocery.Core.Data.Helpers;
using Grocery.Core.Interfaces.Repositories;
using Grocery.Core.Models;
using Microsoft.Data.Sqlite;

namespace Grocery.Core.Data.Repositories
{
    public class GroceryListItemsRepository : DatabaseConnection, IGroceryListItemsRepository
    {
        private readonly List<GroceryListItem> groceryListItems = [];

        public GroceryListItemsRepository()
        {
            CreateTable(@"CREATE TABLE IF NOT EXISTS GroceryListItem (
                            [Id] INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
                            [GroceryListId] INTEGER NOT NULL,
                            [ProductId] INTEGER NOT NULL,
                            [Amount] INTEGER NOT NULL)");

            List<string> insertQueries = [
                @"INSERT OR IGNORE INTO GroceryListItem(Id, GroceryListId, ProductId, Amount) VALUES(1, 1, 1, 3)",
                @"INSERT OR IGNORE INTO GroceryListItem(Id, GroceryListId, ProductId, Amount) VALUES(2, 1, 2, 1)",
                @"INSERT OR IGNORE INTO GroceryListItem(Id, GroceryListId, ProductId, Amount) VALUES(3, 1, 3, 4)",
                @"INSERT OR IGNORE INTO GroceryListItem(Id, GroceryListId, ProductId, Amount) VALUES(4, 2, 1, 2)",
                @"INSERT OR IGNORE INTO GroceryListItem(Id, GroceryListId, ProductId, Amount) VALUES(5, 2, 2, 5)"
            ];
            InsertMultipleWithTransaction(insertQueries);
        }

        public List<GroceryListItem> GetAll()
        {
            List<GroceryListItem> items = [];
            string query = "SELECT Id, GroceryListId, ProductId, Amount FROM GroceryListItem";
            OpenConnection();
            using (SqliteCommand command = new(query, Connection))
            using (SqliteDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    items.Add(new GroceryListItem(
                        reader.GetInt32(0), // Id
                        reader.GetInt32(1), // GroceryListId
                        reader.GetInt32(2), // ProductId
                        reader.GetInt32(3)  // Amount
                    ));
                }
            }
            CloseConnection();
            return items;
        }

        public List<GroceryListItem> GetAllOnGroceryListId(int id)
        {
            List<GroceryListItem> items = [];
            string query = "SELECT Id, GroceryListId, ProductId, Amount FROM GroceryListItem WHERE GroceryListId = @Id";
            OpenConnection();
            using (SqliteCommand command = new(query, Connection))
            {
                command.Parameters.AddWithValue("@Id", id);
                using (SqliteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        items.Add(new GroceryListItem(
                            reader.GetInt32(0),
                            reader.GetInt32(1),
                            reader.GetInt32(2),
                            reader.GetInt32(3)
                        ));
                    }
                }
            }
            CloseConnection();
            return items;
        }

        public GroceryListItem Add(GroceryListItem item)
        {
            string query = @"INSERT INTO GroceryListItem(GroceryListId, ProductId, Amount) 
                             VALUES(@GroceryListId, @ProductId, @Amount); SELECT last_insert_rowid();";

            OpenConnection();
            using (SqliteCommand command = new(query, Connection))
            {
                command.Parameters.AddWithValue("@GroceryListId", item.GroceryListId);
                command.Parameters.AddWithValue("@ProductId", item.ProductId);
                command.Parameters.AddWithValue("@Amount", item.Amount);
                item.Id = Convert.ToInt32(command.ExecuteScalar());
            }
            CloseConnection();
            return item;
        }

        public GroceryListItem? Delete(GroceryListItem item)
        {
            string query = "DELETE FROM GroceryListItem WHERE Id = @Id";
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

        public GroceryListItem? Get(int id)
        {
            GroceryListItem? item = null;
            string query = "SELECT Id, GroceryListId, ProductId, Amount FROM GroceryListItem WHERE Id = @Id";
            OpenConnection();
            using (SqliteCommand command = new(query, Connection))
            {
                command.Parameters.AddWithValue("@Id", id);
                using (SqliteDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        item = new GroceryListItem(
                            reader.GetInt32(0),
                            reader.GetInt32(1),
                            reader.GetInt32(2),
                            reader.GetInt32(3)
                        );
                    }
                }
            }
            CloseConnection();
            return item;
        }

        public GroceryListItem? Update(GroceryListItem item)
        {
            string query = @"UPDATE GroceryListItem 
                             SET GroceryListId = @GroceryListId, ProductId = @ProductId, Amount = @Amount 
                             WHERE Id = @Id";

            OpenConnection();
            using (SqliteCommand command = new(query, Connection))
            {
                command.Parameters.AddWithValue("@GroceryListId", item.GroceryListId);
                command.Parameters.AddWithValue("@ProductId", item.ProductId);
                command.Parameters.AddWithValue("@Amount", item.Amount);
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