using System;
using System.IO;
namespace CCS.Core
{
    public static class AppPaths
    {
        public static readonly string Root=Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),"CoachesComprehensiveSystem");
        public static readonly string DatabaseFile=Path.Combine(Root,"data.db");
        public static readonly string Files=Path.Combine(Root,"files");
        public static readonly string Backups=Path.Combine(Root,"backups");
        public static readonly string Logs=Path.Combine(Root,"logs");
        public static void EnsureCreated()
        {
            Directory.CreateDirectory(Root);
            Directory.CreateDirectory(Files);
            Directory.CreateDirectory(Backups);
            Directory.CreateDirectory(Logs);
        }
    }
}