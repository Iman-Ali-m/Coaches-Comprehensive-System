using System.IO;
using System.IO.Compression;
using CCS.Core;
namespace CCS.Services
{
    public class RestoreService
    {
        public void Restore(string zipPath)
        {
            var temp=Path.Combine(Path.GetTempPath(),"ccs_restore");
            if(Directory.Exists(temp))Directory.Delete(temp,true);
            ZipFile.ExtractToDirectory(zipPath,temp);
            if(File.Exists(AppPaths.DatabaseFile))File.Delete(AppPaths.DatabaseFile);
            var sourceDb=Path.Combine(temp,"data.db");
            if(File.Exists(sourceDb))File.Copy(sourceDb,AppPaths.DatabaseFile);
            var sourceFiles=Path.Combine(temp,"files");
            if(Directory.Exists(sourceFiles))
            {
                if(Directory.Exists(AppPaths.Files))Directory.Delete(AppPaths.Files,true);
                Directory.CreateDirectory(AppPaths.Files);
                foreach(var file in Directory.GetFiles(sourceFiles))
                {
                    File.Copy(file,Path.Combine(AppPaths.Files,Path.GetFileName(file)));
                }
            }
            Directory.Delete(temp,true);
        }
    }
}