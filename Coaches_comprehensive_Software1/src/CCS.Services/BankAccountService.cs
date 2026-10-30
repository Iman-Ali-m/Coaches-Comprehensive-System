using System.Collections.Generic;
using Dapper;
using CCS.Core;
using CCS.Data;
namespace CCS.Services
{
    public class BankAccountService
    {
        public long Create(BankAccount account)
        {
            using(var connection=Database.Open())
            {
                account.CreatedAt=Clock.Now();
                return connection.ExecuteScalar<long>("INSERT INTO BankAccounts(CoachId,BankName,AccountNumber,IBAN,CardNumber,CreatedAt) VALUES(@CoachId,@BankName,@AccountNumber,@IBAN,@CardNumber,@CreatedAt);SELECT last_insert_rowid();",account);
            }
        }
        public IEnumerable<BankAccount> GetByCoachId(long coachId)
        {
            using(var connection=Database.Open())
            {
                return connection.Query<BankAccount>("SELECT * FROM BankAccounts WHERE CoachId=@coachId",new{coachId});
            }
        }
        public void Delete(long id)
        {
            using(var connection=Database.Open())
            {
                connection.Execute("DELETE FROM BankAccounts WHERE Id=@id",new{id});
            }
        }
    }
}