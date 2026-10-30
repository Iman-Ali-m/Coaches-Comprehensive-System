using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using Dapper;
using CCS.Core;
using CCS.Data;
namespace CCS.Services
{
    public class UserService
    {
        public bool HasUser()
        {
            using(var connection=Database.Open())
            {
                return connection.ExecuteScalar<long>("SELECT COUNT(*) FROM Users")>0;
            }
        }
        public void EnsureAdminExists()
        {
            using(var connection=Database.Open())
            {
                if(connection.ExecuteScalar<long>("SELECT COUNT(*) FROM Users")>0)return;
                CreateUser("admin","admin","Admin");
            }
        }
        public void CreateUser(string username,string password,string role)
        {
            if(string.IsNullOrWhiteSpace(username))throw new ArgumentException("invalid-username");
            if(string.IsNullOrEmpty(password))throw new ArgumentException("invalid-password");
            var salt=new byte[16];
            using(var rng=RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }
            byte[] hash;
            using(var derive=new Rfc2898DeriveBytes(password,salt,100000,HashAlgorithmName.SHA256))
            {
                hash=derive.GetBytes(32);
            }
            using(var connection=Database.Open())
            {
                if(connection.ExecuteScalar<long>("SELECT COUNT(*) FROM Users WHERE Username=@u",new{u=username})>0)
                {
                    throw new InvalidOperationException("duplicate-username");
                }
                connection.Execute("INSERT INTO Users(Username,PasswordHash,PasswordSalt,Role,CreatedAt) VALUES(@u,@h,@s,@r,@c)",new{u=username,h=Convert.ToBase64String(hash),s=Convert.ToBase64String(salt),r=role,c=Clock.Now()});
            }
        }
        public User VerifyCredentials(string username,string password)
        {
            using(var connection=Database.Open())
            {
                var user=connection.QueryFirstOrDefault<User>("SELECT * FROM Users WHERE Username=@u",new{u=username});
                if(user==null)return null;
                byte[] computed;
                using(var derive=new Rfc2898DeriveBytes(password,Convert.FromBase64String(user.PasswordSalt),100000,HashAlgorithmName.SHA256))
                {
                    computed=derive.GetBytes(32);
                }
                var stored=Convert.FromBase64String(user.PasswordHash);
                if(stored.Length!=computed.Length)return null;
                int diff=0;
                for(int i=0;i<stored.Length;i++)diff|=stored[i]^computed[i];
                return diff==0?user:null;
            }
        }
        public IEnumerable<User> GetAllUsers()
        {
            using(var connection=Database.Open())
            {
                return connection.Query<User>("SELECT * FROM Users ORDER BY Id");
            }
        }
        public void UpdateUser(long id,string username,string password,string role)
        {
            using(var connection=Database.Open())
            {
                var user=connection.QueryFirstOrDefault<User>("SELECT * FROM Users WHERE Id=@id",new{id});
                if(user==null)return;
                if(user.Role=="Admin"&&role!="Admin")throw new InvalidOperationException("cannot-demote-admin");
                var finalUsername=string.IsNullOrWhiteSpace(username)?user.Username:username;
                var newSalt=user.PasswordSalt;
                var newHash=user.PasswordHash;
                if(!string.IsNullOrEmpty(password))
                {
                    var salt=new byte[16];
                    using(var rng=RandomNumberGenerator.Create())rng.GetBytes(salt);
                    using(var derive=new Rfc2898DeriveBytes(password,salt,100000,HashAlgorithmName.SHA256))
                    {
                        newHash=Convert.ToBase64String(derive.GetBytes(32));
                        newSalt=Convert.ToBase64String(salt);
                    }
                }
                connection.Execute("UPDATE Users SET Username=@u,PasswordHash=@h,PasswordSalt=@s,Role=@r WHERE Id=@id",new{u=finalUsername,h=newHash,s=newSalt,r=role,id});
            }
        }
        public void DeleteUser(long id)
        {
            using(var connection=Database.Open())
            {
                var user=connection.QueryFirstOrDefault<User>("SELECT * FROM Users WHERE Id=@id",new{id});
                if(user==null)return;
                if(user.Role=="Admin")throw new InvalidOperationException("cannot-delete-admin");
                connection.Execute("DELETE FROM Users WHERE Id=@id",new{id});
            }
        }
    }
}