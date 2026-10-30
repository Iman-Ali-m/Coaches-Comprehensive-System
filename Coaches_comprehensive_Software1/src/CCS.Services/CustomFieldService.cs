using System.Collections.Generic;
using Dapper;
using CCS.Core;
using CCS.Data;
namespace CCS.Services
{
    public class CustomFieldService
    {
        public IEnumerable<CustomField> GetAll()
        {
            using(var connection=Database.Open())
            {
                return connection.Query<CustomField>("SELECT * FROM CustomFields ORDER BY DisplayOrder,Id");
            }
        }
        public long Create(CustomField field)
        {
            using(var connection=Database.Open())
            {
                field.CreatedAt=Clock.Now();
                return connection.ExecuteScalar<long>("INSERT INTO CustomFields(Title,FieldType,MinValue,MaxValue,Options,DisplayOrder,TemplateOriginalFileName,TemplateStoredFileName,TemplateFileSize,CreatedAt) VALUES(@Title,@FieldType,@MinValue,@MaxValue,@Options,@DisplayOrder,@TemplateOriginalFileName,@TemplateStoredFileName,@TemplateFileSize,@CreatedAt);SELECT last_insert_rowid();",field);
            }
        }
        public void Update(CustomField field)
        {
            using(var connection=Database.Open())
            {
                connection.Execute("UPDATE CustomFields SET Title=@Title,FieldType=@FieldType,MinValue=@MinValue,MaxValue=@MaxValue,Options=@Options,DisplayOrder=@DisplayOrder,TemplateOriginalFileName=@TemplateOriginalFileName,TemplateStoredFileName=@TemplateStoredFileName,TemplateFileSize=@TemplateFileSize WHERE Id=@Id",field);
            }
        }
        public void Delete(long id)
        {
            using(var connection=Database.Open())
            using(var transaction=connection.BeginTransaction())
            {
                connection.Execute("DELETE FROM CustomFieldFiles WHERE ValueId IN (SELECT Id FROM CustomFieldValues WHERE FieldId=@id)",new{id},transaction);
                connection.Execute("DELETE FROM CustomFieldValues WHERE FieldId=@id",new{id},transaction);
                connection.Execute("DELETE FROM CustomFields WHERE Id=@id",new{id},transaction);
                transaction.Commit();
            }
        }
        public IEnumerable<CustomFieldValue> GetValuesByCoachId(long coachId)
        {
            using(var connection=Database.Open())
            {
                return connection.Query<CustomFieldValue>("SELECT * FROM CustomFieldValues WHERE CoachId=@coachId",new{coachId});
            }
        }
        public void SaveValue(long coachId,long fieldId,string value)
        {
            using(var connection=Database.Open())
            {
                var existing=connection.ExecuteScalar<long>("SELECT Id FROM CustomFieldValues WHERE FieldId=@fieldId AND CoachId=@coachId",new{fieldId,coachId});
                if(existing>0)
                {
                    connection.Execute("UPDATE CustomFieldValues SET Value=@value WHERE Id=@existing",new{value,existing});
                    return;
                }
                connection.Execute("INSERT INTO CustomFieldValues(FieldId,CoachId,Value) VALUES(@fieldId,@coachId,@value)",new{fieldId,coachId,value});
            }
        }
        public long GetValueId(long coachId,long fieldId)
        {
            using(var connection=Database.Open())
            {
                return connection.ExecuteScalar<long>("SELECT Id FROM CustomFieldValues WHERE FieldId=@fieldId AND CoachId=@coachId",new{fieldId,coachId});
            }
        }
        public void SaveFile(long valueId,string originalFileName,string storedFileName,long fileSize)
        {
            using(var connection=Database.Open())
            {
                connection.Execute("INSERT INTO CustomFieldFiles(ValueId,OriginalFileName,StoredFileName,FileSize,UploadedAt) VALUES(@valueId,@originalFileName,@storedFileName,@fileSize,@uploadedAt)",new{valueId,originalFileName,storedFileName,fileSize,uploadedAt=Clock.Now()});
            }
        }
        public IEnumerable<CustomFieldFile> GetFilesByValueId(long valueId)
        {
            using(var connection=Database.Open())
            {
                return connection.Query<CustomFieldFile>("SELECT * FROM CustomFieldFiles WHERE ValueId=@valueId",new{valueId});
            }
        }
    }
}