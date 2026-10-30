using System.Collections.Generic;
using Dapper;
using CCS.Core;
using CCS.Data;
namespace CCS.Services
{
    public class ChessTrainingService
    {
        public long Create(ChessTrainingRecord record)
        {
            using(var connection=Database.Open())
            {
                record.CreatedAt=Clock.Now();
                return connection.ExecuteScalar<long>("INSERT INTO ChessTrainingRecords(CoachId,Year,ChessType,PlayDate,Score,Description,CreatedAt) VALUES(@CoachId,@Year,@ChessType,@PlayDate,@Score,@Description,@CreatedAt);SELECT last_insert_rowid();",record);
            }
        }
        public IEnumerable<ChessTrainingRecord> GetByCoachId(long coachId)
        {
            using(var connection=Database.Open())
            {
                return connection.Query<ChessTrainingRecord>("SELECT * FROM ChessTrainingRecords WHERE CoachId=@coachId",new{coachId});
            }
        }
        public void Delete(long id)
        {
            using(var connection=Database.Open())
            {
                connection.Execute("DELETE FROM ChessTrainingFiles WHERE RecordId=@id",new{id});
                connection.Execute("DELETE FROM ChessTrainingRecords WHERE Id=@id",new{id});
            }
        }
        public void AddFile(long recordId,string originalFileName,string storedFileName,long fileSize)
        {
            using(var connection=Database.Open())
            {
                connection.Execute("INSERT INTO ChessTrainingFiles(RecordId,OriginalFileName,StoredFileName,FileSize,UploadedAt) VALUES(@recordId,@originalFileName,@storedFileName,@fileSize,@uploadedAt)",new{recordId,originalFileName,storedFileName,fileSize,uploadedAt=Clock.Now()});
            }
        }
        public ChessTrainingFile GetFirstFileByRecordId(long recordId)
        {
            using(var connection=Database.Open())
            {
                return connection.QueryFirstOrDefault<ChessTrainingFile>("SELECT * FROM ChessTrainingFiles WHERE RecordId=@recordId ORDER BY Id DESC LIMIT 1",new{recordId});
            }
        }
    }
}