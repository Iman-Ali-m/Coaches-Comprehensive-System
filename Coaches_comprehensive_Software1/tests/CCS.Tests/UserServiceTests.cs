using System;
using Dapper;
using Xunit;
using CCS.Core;
using CCS.Data;
using CCS.Services;
namespace CCS.Tests
{
    public class UserServiceTests
    {
        [Fact]
        public void Register_Login_ChangeCredentials_Flow()
        {
            AppPaths.EnsureCreated();
            DatabaseInitializer.Initialize();
            using(var connection=Database.Open())
            {
                connection.Execute("DELETE FROM Users");
            }
            var service=new UserService();
            Assert.False(service.HasUser());
            service.CreateUser("admin","Pass@123","Admin");
            Assert.True(service.HasUser());
            var user=service.VerifyCredentials("admin","Pass@123");
            Assert.NotNull(user);
            Assert.Equal("Admin",user.Role);
            Assert.Null(service.VerifyCredentials("admin","wrong"));
            Assert.Null(service.VerifyCredentials("wrong","Pass@123"));
            service.UpdateUser(user.Id,null,null,"Admin");
            Assert.NotNull(service.VerifyCredentials("admin","Pass@123"));
            service.UpdateUser(user.Id,null,"NewPass@456","Admin");
            Assert.Null(service.VerifyCredentials("admin","Pass@123"));
            Assert.NotNull(service.VerifyCredentials("admin","NewPass@456"));
            using(var connection=Database.Open())
            {
                connection.Execute("DELETE FROM Users");
            }
        }
    }
}