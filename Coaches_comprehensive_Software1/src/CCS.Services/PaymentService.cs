using System.Collections.Generic;
using Dapper;
using CCS.Core;
using CCS.Data;
namespace CCS.Services
{
    public class PaymentService
    {
        public long Create(Payment payment)
        {
            using(var connection=Database.Open())
            {
                payment.CreatedAt=Clock.Now();
                return connection.ExecuteScalar<long>("INSERT INTO Payments(CoachId,Subject,PaymentDate,StartTime,EndTime,Location,DaysCount,ClassesCount,PaidAmount,ReturnedAmount,PaidDate,CreatedAt) VALUES(@CoachId,@Subject,@PaymentDate,@StartTime,@EndTime,@Location,@DaysCount,@ClassesCount,@PaidAmount,@ReturnedAmount,@PaidDate,@CreatedAt);SELECT last_insert_rowid();",payment);
            }
        }
        public IEnumerable<Payment> GetByCoachId(long coachId)
        {
            using(var connection=Database.Open())
            {
                return connection.Query<Payment>("SELECT * FROM Payments WHERE CoachId=@coachId ORDER BY Id DESC",new{coachId});
            }
        }
        public void Delete(long id)
        {
            using(var connection=Database.Open())
            {
                connection.Execute("DELETE FROM PaymentFiles WHERE PaymentId=@id",new{id});
                connection.Execute("DELETE FROM Payments WHERE Id=@id",new{id});
            }
        }
        public void AddFile(long paymentId,string originalFileName,string storedFileName,long fileSize)
        {
            using(var connection=Database.Open())
            {
                connection.Execute("INSERT INTO PaymentFiles(PaymentId,OriginalFileName,StoredFileName,FileSize,UploadedAt) VALUES(@paymentId,@originalFileName,@storedFileName,@fileSize,@uploadedAt)",new{paymentId,originalFileName,storedFileName,fileSize,uploadedAt=Clock.Now()});
            }
        }
        public PaymentFile GetFirstFileByPaymentId(long paymentId)
        {
            using(var connection=Database.Open())
            {
                return connection.QueryFirstOrDefault<PaymentFile>("SELECT * FROM PaymentFiles WHERE PaymentId=@paymentId ORDER BY Id DESC LIMIT 1",new{paymentId});
            }
        }
    }
}