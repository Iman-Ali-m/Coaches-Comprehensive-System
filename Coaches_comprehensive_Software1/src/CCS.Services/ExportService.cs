using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using Dapper;
using CCS.Core;
using CCS.Data;
namespace CCS.Services
{
    public class ExportService
    {
        public void ExportZip(List<long> coachIds,string destinationZipPath)
        {
            if(File.Exists(destinationZipPath))File.Delete(destinationZipPath);
            var tempDir=Path.Combine(Path.GetTempPath(),Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(tempDir);
            using(var connection=Database.Open())
            {
                foreach(var coachId in coachIds)
                {
                    var coach=connection.QueryFirstOrDefault<Coach>("SELECT * FROM Coaches WHERE Id=@coachId",new{coachId});
                    if(coach==null)continue;
                    var safeName=$"{coach.FirstName}_{coach.LastName}_{coach.NationalCode}".Replace(" ","_");
                    var coachDir=Path.Combine(tempDir,safeName);
                    Directory.CreateDirectory(coachDir);
                    var docs=connection.Query<CoachDocument>("SELECT * FROM CoachDocuments WHERE CoachId=@coachId",new{coachId});
                    foreach(var doc in docs)
                    {
                        var categoryName=DocumentCategories.PersianNames.ContainsKey(doc.Category)?DocumentCategories.PersianNames[doc.Category]:doc.Category;
                        var safeCategory=SanitizeFolderName(categoryName);
                        var categoryDir=Path.Combine(coachDir,safeCategory);
                        Directory.CreateDirectory(categoryDir);
                        var source=Path.Combine(AppPaths.Files,doc.StoredFileName);
                        if(File.Exists(source))
                        {
                            var dest=Path.Combine(categoryDir,doc.OriginalFileName);
                            File.Copy(source,dest,true);
                        }
                    }
                    var courseFiles=connection.Query<CourseFile>("SELECT cf.* FROM CourseFiles cf INNER JOIN CoachingCourses cc ON cc.Id=cf.CourseId WHERE cc.CoachId=@coachId",new{coachId});
                    if(courseFiles.Any())
                    {
                        var courseDir=Path.Combine(coachDir,"دوره های مربیگری");
                        Directory.CreateDirectory(courseDir);
                        foreach(var file in courseFiles)
                        {
                            var source=Path.Combine(AppPaths.Files,file.StoredFileName);
                            if(File.Exists(source))File.Copy(source,Path.Combine(courseDir,file.OriginalFileName),true);
                        }
                    }
                    var chessFiles=connection.Query<ChessTrainingFile>("SELECT ctf.* FROM ChessTrainingFiles ctf INNER JOIN ChessTrainingRecords ctr ON ctr.Id=ctf.RecordId WHERE ctr.CoachId=@coachId",new{coachId});
                    if(chessFiles.Any())
                    {
                        var chessDir=Path.Combine(coachDir,"تداوم آموزش شطرنج");
                        Directory.CreateDirectory(chessDir);
                        foreach(var file in chessFiles)
                        {
                            var source=Path.Combine(AppPaths.Files,file.StoredFileName);
                            if(File.Exists(source))File.Copy(source,Path.Combine(chessDir,file.OriginalFileName),true);
                        }
                    }
                    var paymentFiles=connection.Query<PaymentFile>("SELECT pf.* FROM PaymentFiles pf INNER JOIN Payments p ON p.Id=pf.PaymentId WHERE p.CoachId=@coachId",new{coachId});
                    if(paymentFiles.Any())
                    {
                        var payDir=Path.Combine(coachDir,"رسیدهای پرداخت");
                        Directory.CreateDirectory(payDir);
                        foreach(var file in paymentFiles)
                        {
                            var source=Path.Combine(AppPaths.Files,file.StoredFileName);
                            if(File.Exists(source))File.Copy(source,Path.Combine(payDir,file.OriginalFileName),true);
                        }
                    }
                    var customFiles=connection.Query<CustomFieldFile>("SELECT cff.* FROM CustomFieldFiles cff INNER JOIN CustomFieldValues cfv ON cfv.Id=cff.ValueId INNER JOIN CustomFields cf ON cf.Id=cfv.FieldId WHERE cfv.CoachId=@coachId",new{coachId});
                    if(customFiles.Any())
                    {
                        var customDir=Path.Combine(coachDir,"ویژگی های پویا");
                        Directory.CreateDirectory(customDir);
                        foreach(var file in customFiles)
                        {
                            var source=Path.Combine(AppPaths.Files,file.StoredFileName);
                            if(File.Exists(source))File.Copy(source,Path.Combine(customDir,file.OriginalFileName),true);
                        }
                    }
                }
            }
            ZipFile.CreateFromDirectory(tempDir,destinationZipPath,CompressionLevel.Optimal,false);
            Directory.Delete(tempDir,true);
        }

        public List<long> GetCoachIdsByFileType(string fileType)
        {
            using(var connection=Database.Open())
            {
                if(string.IsNullOrEmpty(fileType))
                {
                    return connection.Query<long>("SELECT Id FROM Coaches").ToList();
                }
                return connection.Query<long>("SELECT Id FROM Coaches WHERE FileType=@fileType",new{fileType}).ToList();
            }
        }

        private static string SanitizeFolderName(string name)
        {
            var invalid=Path.GetInvalidFileNameChars();
            var chars=name.Select(c=>invalid.Contains(c)?'_':c).ToArray();
            return new string(chars);
        }
    }
}