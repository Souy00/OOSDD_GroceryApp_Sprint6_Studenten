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
            // Maak de tabel aan
            CreateTable(@"CREATE TABLE IF NOT EXISTS GroceryListItem (
                            [Id] INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
                            [GroceryListId] INTEGER NOT NULL,
                            [ProductId] INTEGER NOT NULL,
                            [Amount] INTEGER NOT NULL,
                            FOREIGN KEY(GroceryListId) REFERENCES GroceryList(Id),
                            FOREIGN KEY(ProductId) REFERENCES Product(Id))");

            // Voeg voorbeelddata toe
            List<string> insertQueries = [
                @"INSERT OR IGNORE INTO GroceryListItem (GroceryListId, ProductId, Amount) VALUES (1, 1, 3)",
                @"INSERT OR IGNORE INTO GroceryListItem (GroceryListId, ProductId, Amount) VALUES (1, 2, 2)",
                @"INSERT OR IGNORE INTO GroceryListItem (GroceryListId, ProductId, Amount) VALUES (2, 3, 1)"
            ];

            InsertMultipleWithTransaction(insertQueries);
        }

        public List<GroceryListItem> GetAll()
        {
            groceryListItems.Clear();

            using (var connection = GetConnection())
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "SELECT Id, GroceryListId, ProductId, Amount FROM GroceryListItem";

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        groceryListItems.Add(new GroceryListItem(
                            reader.GetInt32(0),  // Id
                            reader.GetInt32(1),  // GroceryListId
                            reader.GetInt32(2),  // ProductId
                            reader.GetInt32(3)   // Amount
                        ));
                    }
                }
            }

            return groceryListItems;
        }

        public List<GroceryListItem> GetAllOnGroceryListId(int id)
        {
            var items = new List<GroceryListItem>();

            using (var connection = GetConnection())
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "SELECT Id, GroceryListId, ProductId, Amount FROM GroceryListItem WHERE GroceryListId = @id";
                command.Parameters.AddWithValue("@id", id);

                using (var reader = command.ExecuteReader())
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

            return items;
        }

        public GroceryListItem Add(GroceryListItem item)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = @"INSERT INTO GroceryListItem (GroceryListId, ProductId, Amount)
                                        VALUES (@listId, @productId, @amount);
                                        SELECT last_insert_rowid();";
                command.Parameters.AddWithValue("@listId", item.GroceryListId);
                command.Parameters.AddWithValue("@productId", item.ProductId);
                command.Parameters.AddWithValue("@amount", item.Amount);

                var newId = (long)command.ExecuteScalar();
                return new GroceryListItem((int)newId, item.GroceryListId, item.ProductId, item.Amount);
            }
        }

        public GroceryListItem? Delete(GroceryListItem item)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "DELETE FROM GroceryListItem WHERE Id = @id";
                command.Parameters.AddWithValue("@id", item.Id);

                int rows = command.ExecuteNonQuery();
                return rows > 0 ? item : null;
            }
        }

        public GroceryListItem? Get(int id)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "SELECT Id, GroceryListId, ProductId, Amount FROM GroceryListItem WHERE Id = @id";
                command.Parameters.AddWithValue("@id", id);

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new GroceryListItem(
                            reader.GetInt32(0),
                            reader.GetInt32(1),
                            reader.GetInt32(2),
                            reader.GetInt32(3)
                        );
                    }
                }
            }

            return null;
        }

        public GroceryListItem? Update(GroceryListItem item)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = @"UPDATE GroceryListItem 
                                        SET GroceryListId = @listId, 
                                            ProductId = @productId, 
                                            Amount = @amount
                                        WHERE Id = @id";
                command.Parameters.AddWithValue("@id", item.Id);
                command.Parameters.AddWithValue("@listId", item.GroceryListId);
                command.Parameters.AddWithValue("@productId", item.ProductId);
                command.Parameters.AddWithValue("@amount", item.Amount);

                int rows = command.ExecuteNonQuery();
                return rows > 0 ? item : null;
            }
        }
    }
}
