using Grocery.Core.Interfaces.Repositories;
using Grocery.Core.Models;
using Microsoft.Data.Sqlite;
using System.Globalization;
using System.Security.Cryptography;

namespace Grocery.Core.Data.Repositories
{
    public class ProductRepository : DatabaseConnection, IProductRepository
    {
        private readonly List<Product> products = [];
        //private object cmd;

        public ProductRepository()
        {
            CreateTable(
                @"CREATE TABLE IF NOT EXISTS Products (
                    Id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
                    Name NVARCHAR(80) UNIQUE NOT NULL,
                    Stock INTEGER NOT NULL,
                    ShelfLife DATE NULL,
                    Price REAL NOT NULL
                )");

            OpenConnection();
            using (var cmd = Connection.CreateCommand())
            {
                cmd.CommandText = @"INSERT OR IGNORE INTO PRODUCTS (Name, Stock, ShelfLife, Price) 
                                    VALUES ('Melk', 300, '2025-09-25', 0.95), 
                                    ('Kaas', 100, '2025-09-30', 7.98), 
                                    ('Brood', 400, '2025-09-12', 2.19), 
                                    ('Cornflakes', 0, '2025-12-31', 1.48);";
                cmd.ExecuteNonQuery();
            }
            CloseConnection();
            GetAll();
        }

        public List<Product> GetAll()
        {
            products.Clear();
            const string sql = "SELECT Id, Name, Stock, ShelfLife, Price FROM Products";
            OpenConnection();
            using (var cmd = Connection.CreateCommand())
            {
                cmd.CommandText = sql;
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int id = reader.GetInt32(0);
                        string name = reader.GetString(1);
                        int stock = reader.GetInt32(2);
                        DateOnly shelfLife = DateOnly.FromDateTime(reader.GetDateTime(3));
                        decimal price = Convert.ToDecimal(reader.GetDouble(4), CultureInfo.InvariantCulture);
                        products.Add(new Product(id, name, stock, shelfLife, price));
                    }
                }

                CloseConnection();
                return products;
                }
            }

            public Product? Get(int id)
            {
                OpenConnection();
            using (SqliteCommand cmd = Connection.CreateCommand())
            //using var reader = cmd.ExecuteReader();
            {
                cmd.CommandText = "SELECT Id, Name, Stock, date(ShelfLife), Price FROM Products WHERE Id = @Id";
                cmd.Parameters.AddWithValue("@Id", id);

                using (SqliteDataReader reader = cmd.ExecuteReader())
                //using var reader = cmd.ExecuteReader();
                {
                    if (reader.Read())
                    {
                        int pid = reader.GetInt32(0);
                        string name = reader.GetString(1);
                        int stock = reader.GetInt32(2);
                        DateOnly shelfLife = reader.IsDBNull(3)
                            ? default
                            : DateOnly.FromDateTime(reader.GetDateTime(3));
                        decimal price = Convert.ToDecimal(reader.GetDouble(4), System.Globalization.CultureInfo.InvariantCulture);

                        CloseConnection();
                        return new Product(pid, name, stock, shelfLife, price);
                    }
                }
            }
                CloseConnection();
                return null;

        }
        public Product Add(Product item)
        {
            const string sql = "INSERT INTO Products (Name, Stock, ShelfLife, Price) VALUES (@Name, @Stock, @ShelfLife, @Price); SELECT last_insert_rowid();";
            OpenConnection();
            using var cmd = Connection.CreateCommand();
            cmd.CommandText = sql;
            cmd.Parameters.AddWithValue("@Name", item.Name);
            cmd.Parameters.AddWithValue("@Stock", item.Stock);
            cmd.Parameters.AddWithValue("@ShelfLife", new DateTime(item.ShelfLife.Year, item.ShelfLife.Month, item.ShelfLife.Day));
            cmd.Parameters.AddWithValue("@Price", item.Price);

            item.Id = Convert.ToInt32((long)cmd.ExecuteScalar());
            CloseConnection();
            return item;
        }

        public Product? Update(Product item)
        {
            const string sql = "UPDATE Products SET Name = @Name, Stock = @Stock, ShelfLife = @ShelfLife, Price = @Price WHERE Id = @Id";
            OpenConnection();
            using var cmd = Connection.CreateCommand();
            cmd.CommandText = sql;
            cmd.Parameters.AddWithValue("@Name", item.Name);
            cmd.Parameters.AddWithValue("@Stock", item.Stock);
            cmd.Parameters.AddWithValue("@ShelfLife", new DateTime(item.ShelfLife.Year, item.ShelfLife.Month, item.ShelfLife.Day));
            cmd.Parameters.AddWithValue("@Price", item.Price);
            cmd.Parameters.AddWithValue("@Id", item.Id);

            var rows = cmd.ExecuteNonQuery();
            CloseConnection();
            return rows > 0 ? Get(item.Id) : null;
        }

        public Product? Delete(Product item)
        {
            const string sql = @"DELETE FROM Products WHERE Id = @Id";
            OpenConnection();
            using var cmd = Connection.CreateCommand();
            cmd.CommandText = sql;
            cmd.Parameters.AddWithValue("@Id", item.Id);
            var rows = cmd.ExecuteNonQuery();
            CloseConnection();
            return rows > 0 ? item : null;
        }

        public List<(string Name, int SoldQuantity, int Stock)> GetBestSellingProducts()
        {
            var results = new List<(string Name, int SoldQuantity, int Stock)>();

            OpenConnection();
            using (var cmd = Connection.CreateCommand())
            {
                cmd.CommandText = @"
                    SELECT p.Name, 
                    COUNT(g.Id) AS SoldQuantity, p.Stock, 
                    FROM Products p 
                    LEFT JOIN GroceryListItems g ON g.ProductID = p.Id
                    GROUP BY p.Name, p.Stock
                    ORDER BY SoldQuantity DESC, p.Stock DESC;";

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string name = reader.GetString(0);
                        int soldQuantity = reader.GetInt32(1);
                        int stock = reader.GetInt32(2);
                        results.Add((name, soldQuantity, stock));
                    }
                }
            }
            CloseConnection();
            return results;
        }
    }
}
