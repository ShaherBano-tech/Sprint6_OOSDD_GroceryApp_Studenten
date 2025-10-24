using Grocery.Core.Interfaces.Repositories;
using Grocery.Core.Models;
using Microsoft.Data.Sqlite;
using System.Collections.Generic;


namespace Grocery.Core.Data.Repositories
{
    public class GroceryListItemsRepository : DatabaseConnection, IGroceryListItemsRepository
    {
        public GroceryListItemsRepository()
        {
            CreateTable(@"
                CREATE TABLE IF NOT EXISTS GroceryListItem (
                    [Id] INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
                    [GroceryListId] INTEGER NOT NULL,
                    [ProductId] INTEGER NOT NULL,
                    [Amount] INTEGER NOT NULL,
                    Notes TEXT
                )");
        }

        public List<GroceryListItem> GetAll()
        {
            OpenConnection();
            using var cmd = Connection.CreateCommand();
            cmd.CommandText = "SELECT Id, GroceryListId, ProductId, Amount FROM GroceryListItem";
            using var reader = cmd.ExecuteReader();

            var list = new List<GroceryListItem>();
            while (reader.Read())
            {
                list.Add(new GroceryListItem(
                    reader.GetInt32(0),
                    reader.GetInt32(1),
                    reader.GetInt32(2),
                    reader.GetInt32(3)
                ));
            }
            CloseConnection();
            return list;
        }

        public List<GroceryListItem> GetAllOnGroceryListId(int id)
        {
            OpenConnection();
            using var cmd = Connection.CreateCommand();
            cmd.CommandText = "SELECT Id, GroceryListId, ProductId, Amount FROM GroceryListItem WHERE GroceryListId = @GroceryListId";
            cmd.Parameters.Add(new SqliteParameter("@GroceryListId", id));

            using var reader = cmd.ExecuteReader();
            var list = new List<GroceryListItem>();
            while (reader.Read())
            {
                list.Add(new GroceryListItem(
                    reader.GetInt32(0),
                    reader.GetInt32(1),
                    reader.GetInt32(2),
                    reader.GetInt32(3)
                ));
            }
            CloseConnection();
            return list;
        }

        public GroceryListItem Add(GroceryListItem item)
        {
            OpenConnection();
            using var cmd = Connection.CreateCommand();
            cmd.CommandText = "INSERT INTO GroceryListItem (GroceryListId, ProductId, Amount) VALUES (@GroceryListId, @ProductId, @Amount); SELECT last_insert_rowid();";
            cmd.Parameters.Add(new SqliteParameter("@GroceryListId", item.GroceryListId));
            cmd.Parameters.Add(new SqliteParameter("@ProductId", item.ProductId));
            cmd.Parameters.Add(new SqliteParameter("@Amount", item.Amount));

            long newId = (long)cmd.ExecuteScalar();
            item.Id = (int)newId;
            CloseConnection();

            return Get(item.Id)!;
        }

        public GroceryListItem? Get(int id)
        {
            OpenConnection();
            using var cmd = Connection.CreateCommand();
            cmd.CommandText = "SELECT Id, GroceryListId, ProductId, Amount FROM GroceryListItem WHERE Id = @Id";
            cmd.Parameters.Add(new SqliteParameter("@Id", id));
            using var reader = cmd.ExecuteReader();
            GroceryListItem? found = null;
            if (reader.Read())
            {
                found = new GroceryListItem(
                    reader.GetInt32(0),
                    reader.GetInt32(1),
                    reader.GetInt32(2),
                    reader.GetInt32(3)
                );
            }
            CloseConnection();
            return found;
        }
        public GroceryListItem? Update(GroceryListItem item)
        {
            OpenConnection();
            using var cmd = Connection.CreateCommand();
            cmd.CommandText = "UPDATE GroceryListItem SET GroceryListId = @GroceryListId, ProductId = @ProductId, Amount = @Amount WHERE Id = @Id";
            cmd.Parameters.Add(new SqliteParameter("@GroceryListId", item.GroceryListId));
            cmd.Parameters.Add(new SqliteParameter("@ProductId", item.ProductId));
            cmd.Parameters.Add(new SqliteParameter("@Amount", item.Amount));
            cmd.Parameters.Add(new SqliteParameter("@Id", item.Id));

            var rows = cmd.ExecuteNonQuery();
            CloseConnection();

            return rows > 0 ? Get(item.Id) : null;
        }

        public GroceryListItem? Delete(GroceryListItem item)
        {
            OpenConnection();
            using var cmd = Connection.CreateCommand();
            cmd.CommandText = "DELETE FROM GroceryListItem WHERE Id = @Id";
            cmd.Parameters.Add(new SqliteParameter("@Id", item.Id));
            var rows = cmd.ExecuteNonQuery();
            CloseConnection();
            return rows > 0 ? item : null;
        }
    }
}