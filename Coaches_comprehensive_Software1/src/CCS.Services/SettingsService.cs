using Dapper;
using CCS.Core;
using CCS.Data;
namespace CCS.Services
{
    public class SettingsService
    {
        public string GetValue(string key)
        {
            using(var connection=Database.Open())
            {
                return connection.QueryFirstOrDefault<string>("SELECT Value FROM AppSettings WHERE Key=@key",new{key});
            }
        }
        public void SetValue(string key,string value)
        {
            using(var connection=Database.Open())
            {
                var existing=connection.ExecuteScalar<long>("SELECT COUNT(*) FROM AppSettings WHERE Key=@key",new{key});
                if(existing>0)
                {
                    connection.Execute("UPDATE AppSettings SET Value=@value WHERE Key=@key",new{key,value});
                    return;
                }
                connection.Execute("INSERT INTO AppSettings(Key,Value) VALUES(@key,@value)",new{key,value});
            }
        }
    }
}