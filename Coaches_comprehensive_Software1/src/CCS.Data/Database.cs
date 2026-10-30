using System.Data;
using System.Data.SQLite;
using CCS.Core;
namespace CCS.Data
{
    public static class Database
    {
        public static IDbConnection Open()
        {
            var connection=new SQLiteConnection($"Data Source={AppPaths.DatabaseFile};Version=3;");
            connection.Open();
            return connection;
        }
    }
}