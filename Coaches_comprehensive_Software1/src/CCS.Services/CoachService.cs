using System.Collections.Generic;
using Dapper;
using CCS.Core;
using CCS.Data;
namespace CCS.Services
{
    public class CoachService
    {
        public long Create(Coach coach)
        {
            using(var connection=Database.Open())
            {
                coach.CreatedAt=Clock.Now();
                return connection.ExecuteScalar<long>("INSERT INTO Coaches(FirstName,LastName,FatherName,NationalCode,BirthCertificateNumber,PersonnelCode,BirthDate,Gender,MaritalStatus,ChildrenCount,FieldOfStudy,Education,MembershipType,EntryDate,LastPosition,FileType,DistrictName,MilitaryStatus,PhysicalStatus,Weight,Height,BloodType,SacrificeStatus,Job,WorkAddress,HomeAddress,Phone1,Phone2,CreatedAt) VALUES(@FirstName,@LastName,@FatherName,@NationalCode,@BirthCertificateNumber,@PersonnelCode,@BirthDate,@Gender,@MaritalStatus,@ChildrenCount,@FieldOfStudy,@Education,@MembershipType,@EntryDate,@LastPosition,@FileType,@DistrictName,@MilitaryStatus,@PhysicalStatus,@Weight,@Height,@BloodType,@SacrificeStatus,@Job,@WorkAddress,@HomeAddress,@Phone1,@Phone2,@CreatedAt);SELECT last_insert_rowid();",coach);
            }
        }
        public void Update(Coach coach)
        {
            using(var connection=Database.Open())
            {
                coach.UpdatedAt=Clock.Now();
                connection.Execute("UPDATE Coaches SET FirstName=@FirstName,LastName=@LastName,FatherName=@FatherName,NationalCode=@NationalCode,BirthCertificateNumber=@BirthCertificateNumber,PersonnelCode=@PersonnelCode,BirthDate=@BirthDate,Gender=@Gender,MaritalStatus=@MaritalStatus,ChildrenCount=@ChildrenCount,FieldOfStudy=@FieldOfStudy,Education=@Education,MembershipType=@MembershipType,EntryDate=@EntryDate,LastPosition=@LastPosition,FileType=@FileType,DistrictName=@DistrictName,MilitaryStatus=@MilitaryStatus,PhysicalStatus=@PhysicalStatus,Weight=@Weight,Height=@Height,BloodType=@BloodType,SacrificeStatus=@SacrificeStatus,Job=@Job,WorkAddress=@WorkAddress,HomeAddress=@HomeAddress,Phone1=@Phone1,Phone2=@Phone2,UpdatedAt=@UpdatedAt WHERE Id=@Id",coach);
            }
        }
        public Coach GetById(long id)
        {
            using(var connection=Database.Open())
            {
                return connection.QueryFirstOrDefault<Coach>("SELECT * FROM Coaches WHERE Id=@id",new{id});
            }
        }
        public IEnumerable<Coach> Search(string firstName,string lastName,string fatherName,string nationalCode)
        {
            using(var connection=Database.Open())
            {
                var sql="SELECT * FROM Coaches WHERE 1=1";
                if(!string.IsNullOrWhiteSpace(firstName))sql+=" AND FirstName LIKE '%'||@FirstName||'%'";
                if(!string.IsNullOrWhiteSpace(lastName))sql+=" AND LastName LIKE '%'||@LastName||'%'";
                if(!string.IsNullOrWhiteSpace(fatherName))sql+=" AND FatherName LIKE '%'||@FatherName||'%'";
                if(!string.IsNullOrWhiteSpace(nationalCode))sql+=" AND NationalCode LIKE '%'||@NationalCode||'%'";
                return connection.Query<Coach>(sql,new{FirstName=firstName,LastName=lastName,FatherName=fatherName,NationalCode=nationalCode});
            }
        }
        public void Delete(long id)
        {
            using(var connection=Database.Open())
            using(var transaction=connection.BeginTransaction())
            {
                connection.Execute("DELETE FROM CustomFieldFiles WHERE ValueId IN (SELECT Id FROM CustomFieldValues WHERE CoachId=@id)",new{id},transaction);
                connection.Execute("DELETE FROM CustomFieldValues WHERE CoachId=@id",new{id},transaction);
                connection.Execute("DELETE FROM PaymentFiles WHERE PaymentId IN (SELECT Id FROM Payments WHERE CoachId=@id)",new{id},transaction);
                connection.Execute("DELETE FROM Payments WHERE CoachId=@id",new{id},transaction);
                connection.Execute("DELETE FROM ChessTrainingFiles WHERE RecordId IN (SELECT Id FROM ChessTrainingRecords WHERE CoachId=@id)",new{id},transaction);
                connection.Execute("DELETE FROM ChessTrainingRecords WHERE CoachId=@id",new{id},transaction);
                connection.Execute("DELETE FROM BankAccounts WHERE CoachId=@id",new{id},transaction);
                connection.Execute("DELETE FROM CourseFiles WHERE CourseId IN (SELECT Id FROM CoachingCourses WHERE CoachId=@id)",new{id},transaction);
                connection.Execute("DELETE FROM CoachingCourses WHERE CoachId=@id",new{id},transaction);
                connection.Execute("DELETE FROM CoachForms WHERE CoachId=@id",new{id},transaction);
                connection.Execute("DELETE FROM CoachDocuments WHERE CoachId=@id",new{id},transaction);
                connection.Execute("DELETE FROM Coaches WHERE Id=@id",new{id},transaction);
                transaction.Commit();
            }
        }
    }
}