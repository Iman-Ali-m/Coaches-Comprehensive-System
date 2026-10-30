using System;
using System.IO;
using CCS.Core;
namespace CCS.Services
{
    public class LoggerService
    {
        public void LogError(Exception ex,string context)
        {
            var path=Path.Combine(AppPaths.Logs,$"log_{DateTime.Now:yyyyMMdd}.txt");
            var text=$"[{DateTime.Now:HH:mm:ss}] ERROR in {context}: {ex.Message}\n{ex.StackTrace}\n";
            File.AppendAllText(path,text);
        }
        public void LogInfo(string message)
        {
            var path=Path.Combine(AppPaths.Logs,$"log_{DateTime.Now:yyyyMMdd}.txt");
            var text=$"[{DateTime.Now:HH:mm:ss}] INFO: {message}\n";
            File.AppendAllText(path,text);
        }
    }
}