using System.IO;
using Dapper;
using Xunit;
using CCS.Core;
using CCS.Data;
using CCS.Services;
namespace CCS.Tests.Services
{
    public class ExportServiceTests
    {
        [Fact]
        public void ExportZip_CreatesValidFile()
        {
            AppPaths.EnsureCreated();
            DatabaseInitializer.Initialize();
            long coachId;
            using(var connection=Database.Open())
            {
                connection.Execute("DELETE FROM CoachDocuments");
                connection.Execute("DELETE FROM Coaches");
                coachId=connection.ExecuteScalar<long>("INSERT INTO Coaches(FirstName,LastName,NationalCode,CreatedAt) VALUES('Test','User','1234567890',@c);SELECT last_insert_rowid();",new{c=Clock.Now()});
            }
            var fileService=new FileService();
            var tempFile=Path.GetTempFileName();
            File.WriteAllText(tempFile,"test");
            var stored=fileService.SaveToDisk(tempFile);
            fileService.RegisterDocument(coachId,DocumentCategories.NationalCardFrontBack,"test.txt",stored,4);
            var destZip=Path.Combine(Path.GetTempPath(),"test_export.zip");
            var exportService=new ExportService();
            exportService.ExportZip(new System.Collections.Generic.List<long>{coachId},destZip);
            Assert.True(File.Exists(destZip));
            Assert.True(new FileInfo(destZip).Length>0);
            if(File.Exists(destZip))File.Delete(destZip);
            if(File.Exists(tempFile))File.Delete(tempFile);
            using(var connection=Database.Open())
            {
                connection.Execute("DELETE FROM CoachDocuments");
                connection.Execute("DELETE FROM Coaches");
            }
        }
    }
}