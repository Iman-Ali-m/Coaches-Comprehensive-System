using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using Dapper;
using CCS.Core;
using CCS.Data;
namespace CCS.Services
{
    public class FileService
    {
        public string SaveToDisk(string sourcePath)
        {
            var extension=Path.GetExtension(sourcePath);
            var storedName=Guid.NewGuid().ToString("N")+extension;
            var destination=Path.Combine(AppPaths.Files,storedName);
            File.Copy(sourcePath,destination,true);
            return storedName;
        }
        public void RegisterDocument(long coachId,string category,string originalFileName,string storedFileName,long fileSize)
        {
            using(var connection=Database.Open())
            {
                connection.Execute("INSERT INTO CoachDocuments(CoachId,Category,OriginalFileName,StoredFileName,FileSize,UploadedAt) VALUES(@CoachId,@Category,@OriginalFileName,@StoredFileName,@FileSize,@UploadedAt)",new{CoachId=coachId,Category=category,OriginalFileName=originalFileName,StoredFileName=storedFileName,FileSize=fileSize,UploadedAt=Clock.Now()});
            }
        }
        public IEnumerable<CoachDocument> GetDocumentsByCoachId(long coachId)
        {
            using(var connection=Database.Open())
            {
                return connection.Query<CoachDocument>("SELECT * FROM CoachDocuments WHERE CoachId=@coachId ORDER BY UploadedAt DESC",new{coachId});
            }
        }
        public void OpenDocument(string storedFileName)
        {
            var path=Path.Combine(AppPaths.Files,storedFileName);
            if(File.Exists(path))
            {
                Process.Start(new ProcessStartInfo(path){UseShellExecute=true});
            }
        }
        public void DeleteDocument(long id,string storedFileName)
        {
            using(var connection=Database.Open())
            {
                connection.Execute("DELETE FROM CoachDocuments WHERE Id=@id",new{id});
            }
            var path=Path.Combine(AppPaths.Files,storedFileName);
            if(File.Exists(path))
            {
                File.Delete(path);
            }
        }
        public void BackupAllData(string destinationZipPath)
        {
            if(File.Exists(destinationZipPath))
            {
                File.Delete(destinationZipPath);
            }
            var tempDir=Path.Combine(Path.GetTempPath(),Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(tempDir);
            var tempFiles=Path.Combine(tempDir,"files");
            Directory.CreateDirectory(tempFiles);
            if(File.Exists(AppPaths.DatabaseFile))
            {
                File.Copy(AppPaths.DatabaseFile,Path.Combine(tempDir,"data.db"));
            }
            if(Directory.Exists(AppPaths.Files))
            {
                foreach(var file in Directory.GetFiles(AppPaths.Files))
                {
                    File.Copy(file,Path.Combine(tempFiles,Path.GetFileName(file)));
                }
            }
            ZipFile.CreateFromDirectory(tempDir,destinationZipPath,CompressionLevel.Optimal,false);
            Directory.Delete(tempDir,true);
        }
    }
}