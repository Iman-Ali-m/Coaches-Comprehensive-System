using System.Linq;
using Dapper;
using Xunit;
using CCS.Core;
using CCS.Data;
using CCS.Services;
namespace CCS.Tests.Services
{
    public class CustomFieldServiceTests
    {
        [Fact]
        public void CreateAndSaveValue_Works()
        {
            AppPaths.EnsureCreated();
            DatabaseInitializer.Initialize();
            using(var connection=Database.Open())
            {
                connection.Execute("DELETE FROM CustomFieldValues");
                connection.Execute("DELETE FROM CustomFields");
            }
            var service=new CustomFieldService();
            var field=new CustomField{Title="TestField",FieldType=CustomFieldTypes.Text,DisplayOrder=1};
            var id=service.Create(field);
            service.SaveValue(999,id,"TestValue");
            var val=service.GetValuesByCoachId(999);
            Assert.Single(val);
            Assert.Equal("TestValue",val.First().Value);
            using(var connection=Database.Open())
            {
                connection.Execute("DELETE FROM CustomFieldValues");
                connection.Execute("DELETE FROM CustomFields");
            }
        }
    }
}