using Grocery.Core.Data.Helpers;
using Microsoft.Data.Sqlite;

namespace Grocery.Core.Data
{
    public abstract class DatabaseConnection : IDisposable
    {
        protected SqliteConnection Connection { get; }
        readonly string databaseName;

        public DatabaseConnection()
        {
            databaseName = ConnectionHelper.ConnectionStringValue("GroceryAppDb");
            string projectDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string dbpath = "Data Source=" + Path.Combine(projectDirectory, databaseName);
            Connection = new SqliteConnection(dbpath);
        }

        protected void OpenConnection()
        {
            if (Connection.State != System.Data.ConnectionState.Open)
                Connection.Open();
        }

        protected void CloseConnection()
        {
            if (Connection.State != System.Data.ConnectionState.Closed)
                Connection.Close();
        }

        public SqliteConnection GetConnection() => Connection;

        public void CreateTable(string commandText)
        {
            OpenConnection();
            using var command = Connection.CreateCommand();
            command.CommandText = commandText;
            command.ExecuteNonQuery();
        }

        public void InsertMultipleWithTransaction(List<string> linesToInsert)
        {
            OpenConnection();
            using var transaction = Connection.BeginTransaction();
            try
            {
                foreach (var line in linesToInsert)
                    Connection.ExecuteNonQuery(line);

                transaction.Commit();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                transaction.Rollback();
            }
        }

        public void Dispose()
        {
            CloseConnection();
            Connection?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
