using Xunit;
using CCS.Core;
using CCS.Data;
using CCS.Services;
namespace CCS.Tests
{
    public class CoachServiceTests
    {
        [Fact]
        public void Create_And_Get_Coach()
        {
            AppPaths.EnsureCreated();
            DatabaseInitializer.Initialize();
            var service=new CoachService();
            var coach=new Coach{FirstName="Test",LastName="User",NationalCode="1234567890",FileType=FileTypes.Official};
            var id=service.Create(coach);
            var loaded=service.GetById(id);
            Assert.NotNull(loaded);
            Assert.Equal("Test",loaded.FirstName);
            Assert.Equal("1234567890",loaded.NationalCode);
            Assert.Equal(FileTypes.Official,loaded.FileType);
        }
    }
}