using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Threading;
using CCS.Core;
using CCS.Data;
using CCS.Services;
namespace CCS.App
{
    public partial class App:Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            AppPaths.EnsureCreated();
            DatabaseInitializer.Initialize();
            
            var logger=new LoggerService();
            ServiceContainer.Register(logger);
            ServiceContainer.Register(new SettingsService());
            var userService=new UserService();
            ServiceContainer.Register(userService);
            ServiceContainer.Register(new FileService());
            ServiceContainer.Register(new CoachService());
            ServiceContainer.Register(new CoachFormService());
            ServiceContainer.Register(new CoachingCourseService());
            ServiceContainer.Register(new BankAccountService());
            ServiceContainer.Register(new ChessTrainingService());
            ServiceContainer.Register(new PaymentService());
            ServiceContainer.Register(new CustomFieldService());
            ServiceContainer.Register(new ExportService());
            ServiceContainer.Register(new ExportPaymentService());
            ServiceContainer.Register(new ResumeExportService());
            ServiceContainer.Register(new CoachingCourseExportService());
            ServiceContainer.Register(new StatsService());
            ServiceContainer.Register(new RestoreService());
            
            DispatcherUnhandledException+=App_DispatcherUnhandledException;
            AppDomain.CurrentDomain.UnhandledException+=CurrentDomain_UnhandledException;
            
            PerformAutoBackup();
            
            base.OnStartup(e);
            
            userService.EnsureAdminExists();

            new LoginWindow().Show();
        }
        private void App_DispatcherUnhandledException(object sender,DispatcherUnhandledExceptionEventArgs e)
        {
            ServiceContainer.Get<LoggerService>().LogError(e.Exception,"UI Thread");
            MessageBox.Show("خطای پیش‌بینی نشده‌ای رخ داد. جزئیات در فایل لاگ ذخیره شد.","خطا",MessageBoxButton.OK,MessageBoxImage.Error);
            e.Handled=true;
        }
        private void CurrentDomain_UnhandledException(object sender,UnhandledExceptionEventArgs e)
        {
            var ex=e.ExceptionObject as Exception;
            if(ex!=null)ServiceContainer.Get<LoggerService>().LogError(ex,"Background Thread");
        }
        private void PerformAutoBackup()
        {
            try
            {
                var settings=ServiceContainer.Get<SettingsService>();
                var daysStr=settings.GetValue("AutoBackupDays");
                if(!int.TryParse(daysStr,out var days)||days<1)days=7;
                var lastBackupStr=settings.GetValue("LastAutoBackup");
                DateTime lastBackup=DateTime.MinValue;
                if(!string.IsNullOrEmpty(lastBackupStr))DateTime.TryParse(lastBackupStr,out lastBackup);
                if((DateTime.Now-lastBackup).TotalDays<days)return;
                var fileName=$"auto_backup_{DateTime.Now:yyyyMMdd_HHmmss}.zip";
                var dest=Path.Combine(AppPaths.Backups,fileName);
                ServiceContainer.Get<FileService>().BackupAllData(dest);
                settings.SetValue("LastAutoBackup",DateTime.Now.ToString("o"));
                var oldBackups=Directory.GetFiles(AppPaths.Backups,"auto_backup_*.zip").OrderByDescending(File.GetCreationTime).Skip(5);
                foreach(var old in oldBackups)
                {
                    if(File.Exists(old))File.Delete(old);
                }
                ServiceContainer.Get<LoggerService>().LogInfo($"Auto backup created: {fileName}");
            }
            catch(Exception ex)
            {
                if(ServiceContainer.Get<LoggerService>()!=null)ServiceContainer.Get<LoggerService>().LogError(ex,"AutoBackup");
            }
        }
    }
}