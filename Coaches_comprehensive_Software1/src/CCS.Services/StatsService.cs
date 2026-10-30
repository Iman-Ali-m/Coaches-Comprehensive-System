using System.Collections.Generic;
using Dapper;
using CCS.Core;
using CCS.Data;
namespace CCS.Services
{
    public class StatsService
    {
        public int GetTotalCoaches()
        {
            using(var connection=Database.Open())
            {
                return connection.ExecuteScalar<int>("SELECT COUNT(*) FROM Coaches");
            }
        }
        public Dictionary<string,int> GetCoachCountsByType()
        {
            using(var connection=Database.Open())
            {
                var dict=new Dictionary<string,int>();
                foreach(var key in FileTypes.PersianNames.Keys)dict[key]=0;
                var rows=connection.Query("SELECT FileType,COUNT(*) as Count FROM Coaches GROUP BY FileType");
                foreach(var row in rows)
                {
                    var type=(string)row.FileType;
                    if(!string.IsNullOrEmpty(type)&&dict.ContainsKey(type))dict[type]=System.Convert.ToInt32(row.Count);
                }
                return dict;
            }
        }
        public int GetTotalCourses()
        {
            using(var connection=Database.Open())
            {
                return connection.ExecuteScalar<int>("SELECT COUNT(*) FROM CoachingCourses");
            }
        }
        public int GetTotalPayments()
        {
            using(var connection=Database.Open())
            {
                return connection.ExecuteScalar<int>("SELECT COUNT(*) FROM Payments");
            }
        }
    }
}