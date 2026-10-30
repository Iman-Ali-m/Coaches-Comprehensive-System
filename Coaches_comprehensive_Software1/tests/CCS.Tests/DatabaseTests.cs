using System;
using Xunit;
using CCS.Core;
using CCS.Data;
namespace CCS.Tests
{
    public class DatabaseTests
    {
        [Fact]
        public void Initialize_CreatesAllTables()
        {
            AppPaths.EnsureCreated();
            DatabaseInitializer.Initialize();
            using(var connection=Database.Open())
            using(var command=connection.CreateCommand())
            {
                command.CommandText="SELECT COUNT(*) FROM sqlite_master WHERE type='table' AND name IN ('Users','Coaches','CoachDocuments','CoachForms','CoachingCourses','CourseFiles','BankAccounts','ChessTrainingRecords','ChessTrainingFiles','Payments','PaymentFiles','CustomFields','CustomFieldValues','CustomFieldFiles','SchemaVersion','AppSettings')";
                Assert.Equal(16L,Convert.ToInt64(command.ExecuteScalar()));
            }
        }
    }
}