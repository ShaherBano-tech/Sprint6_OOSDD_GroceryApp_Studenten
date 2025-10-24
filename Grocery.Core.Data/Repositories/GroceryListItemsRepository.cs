using Grocery.Core.Interfaces.Repositories;
using Grocery.Core.Models;
using Microsoft.Data.Sqlite;

namespace Grocery.Core.Data.Repositories
{
    public class GroceryListItemsRepository : DatabaseConnection, IGroceryListItemsRepository
    {
        public GroceryListItemsRepository()
        {

            CreateTable(@"CREATE TABLE IF NOT EXISTS GroceryListItems(
                        Id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
                        GroceryListId INTEGER NOT NULL,
                        ProductId INTEGER NOT NULL,
                        Amount INTEGER NOT NULL);"
                        );


            OpenConnection();
            using (var cmd = Connection.CreateCommand())
            {
                cmd.CommandText = "SELECT COUNT(1) FROM GroceryListItems;";
                var count = Convert.ToInt32((long)cmd.ExecuteScalar());
                if (count == 0)
                {
                    InsertMultipleWithTransaction(new List<string>
                    {
                     
                        "INSERT INTO GroceryListItems (GroceryListId, ProductId, Amount) VALUES (1, 1, 3);",
                        "INSERT INTO GroceryListItems (GroceryListId, ProductId, Amount) VALUES (1, 2, 1);",
                        "INSERT INTO GroceryListItems (GroceryListId, ProductId, Amount) VALUES (1, 3, 4);",
                     
                        "INSERT INTO GroceryListItems (GroceryListId, ProductId, Amount) VALUES (2, 1, 2);",
                        "INSERT INTO GroceryListItems (GroceryListId, ProductId, Amount) VALUES (2, 2, 5);"
                    });
                }
            }
            CloseConnection();
        }

        public List<GroceryListItem> GetAll()
        {
            var items = new List<GroceryListItem>();
            OpenConnection();
            using (var cmd = Connection.CreateCommand())
            {
                cmd.CommandText = "SELECT Id, GroceryListId, ProductId, Amount FROM GroceryListItems;";
                using (var r = cmd.ExecuteReader())
                {
                    while (r.Read())
                    {
                        items.Add(new GroceryListItem(
                            r.GetInt32(0),
                            r.GetInt32(1),
                            r.GetInt32(2),
                            r.GetInt32(3)));
                    }
                }
            }
            CloseConnection();
            return items;
        }

        public List<GroceryListItem> GetAllOnGroceryListId(int groceryListId)
        {
            var items = new List<GroceryListItem>();
            OpenConnection();
            using (var cmd = Connection.CreateCommand())
            {
                cmd.CommandText = @"SELECT Id, GroceryListId, ProductId, Amount
                                    FROM GroceryListItems
                                    WHERE GroceryListId = @Gid;";
                cmd.Parameters.AddWithValue("@Gid", groceryListId);

                using (var r = cmd.ExecuteReader())
                {
                    while (r.Read())
                    {
                        items.Add(new GroceryListItem(
                            r.GetInt32(0),
                            r.GetInt32(1),
                            r.GetInt32(2),
                            r.GetInt32(3)));
                    }
                }
            }
            CloseConnection();
            return items;
        }

        public GroceryListItem Add(GroceryListItem item)
        {
            OpenConnection();
            using (var cmd = Connection.CreateCommand())
            {
                cmd.CommandText = @"INSERT INTO GroceryListItems (GroceryListId, ProductId, Amount)
                                    VALUES (@Gid, @Pid, @Amount);
                                    SELECT last_insert_rowid();";
                cmd.Parameters.AddWithValue("@Gid", item.GroceryListId);
                cmd.Parameters.AddWithValue("@Pid", item.ProductId);
                cmd.Parameters.AddWithValue("@Amount", item.Amount);

                item.Id = Convert.ToInt32((long)cmd.ExecuteScalar());
            }
            CloseConnection();
            return Get(item.Id)!;
        }

        public GroceryListItem? Update(GroceryListItem item)
        {
            OpenConnection();
            using (var cmd = Connection.CreateCommand())
            {
                cmd.CommandText = @"UPDATE GroceryListItems
                                    SET GroceryListId = @Gid, ProductId = @Pid, Amount = @Amount
                                    WHERE Id = @Id;";
                cmd.Parameters.AddWithValue("@Gid", item.GroceryListId);
                cmd.Parameters.AddWithValue("@Pid", item.ProductId);
                cmd.Parameters.AddWithValue("@Amount", item.Amount);
                cmd.Parameters.AddWithValue("@Id", item.Id);

                var rows = cmd.ExecuteNonQuery();
                CloseConnection();
                return rows > 0 ? Get(item.Id) : null;
            }
        }

        public GroceryListItem? Delete(GroceryListItem item)
        {
            OpenConnection();
            using (var cmd = Connection.CreateCommand())
            {
                cmd.CommandText = "DELETE FROM GroceryListItems WHERE Id = @Id;";
                cmd.Parameters.AddWithValue("@Id", item.Id);
                var rows = cmd.ExecuteNonQuery();
                CloseConnection();
                return rows > 0 ? item : null;
            }
        }

        public GroceryListItem? Get(int id)
        {
            GroceryListItem? gi = null;
            OpenConnection();
            using (var cmd = Connection.CreateCommand())
            {
                cmd.CommandText = @"SELECT Id, GroceryListId, ProductId, Amount
                                    FROM GroceryListItems
                                    WHERE Id = @Id;";
                cmd.Parameters.AddWithValue("@Id", id);

                using (var r = cmd.ExecuteReader())
                {
                    if (r.Read())
                    {
                        gi = new GroceryListItem(
                            r.GetInt32(0),
                            r.GetInt32(1),
                            r.GetInt32(2),
                            r.GetInt32(3));
                    }
                }
            }
            CloseConnection();
            return gi;
        }
    }
}