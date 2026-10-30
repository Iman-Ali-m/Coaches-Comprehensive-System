using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using Microsoft.Win32;
using CCS.Core;
using CCS.Services;
namespace CCS.App
{
    public partial class MainWindow:Window
    {
        public MainWindow()
        {
            InitializeComponent();
            ApplyPermissions();
            LoadFooterStats();
        }
        private void LoadFooterStats()
        {
            var service=ServiceContainer.Get<StatsService>();
            FooterTotal.Text=service.GetTotalCoaches().ToString();
            var dict=service.GetCoachCountsByType();
            var parts=new List<string>();
            foreach(var kvp in dict)
            {
                var name=FileTypes.PersianNames.ContainsKey(kvp.Key)?FileTypes.PersianNames[kvp.Key]:kvp.Key;
                parts.Add($"{name}: {kvp.Value}");
            }
            FooterDetails.Text=string.Join("  |  ",parts);
        }
        private void ApplyPermissions()
        {
            var role=Session.CurrentUser?.Role;
            if(role=="Registrar")
            {
                BtnManageUsers.Visibility=Visibility.Collapsed;
                BtnSearch.Visibility=Visibility.Collapsed;
                BtnPayments.Visibility=Visibility.Collapsed;
                BtnFields.Visibility=Visibility.Collapsed;
                BtnZip.Visibility=Visibility.Collapsed;
                BtnExpPay.Visibility=Visibility.Collapsed;
                BtnExpResume.Visibility=Visibility.Collapsed;
                BtnExpCourse.Visibility=Visibility.Collapsed;
                BtnBackup.Visibility=Visibility.Collapsed;
                BtnRestore.Visibility=Visibility.Collapsed;
                BtnSettings.Visibility=Visibility.Collapsed;
            }
            else if(role=="Manager")
            {
                BtnManageUsers.Visibility=Visibility.Visible;
            }
            else if(role=="Admin")
            {
                BtnManageUsers.Visibility=Visibility.Visible;
            }
        }
        private void TitleBar_MouseLeftButtonDown(object sender,MouseButtonEventArgs e)
        {
            if(e.ClickCount==2)WindowState=WindowState==WindowState.Maximized?WindowState.Normal:WindowState.Maximized;
            else if(e.LeftButton==MouseButtonState.Pressed)DragMove();
        }
        private void MinimizeButton_Click(object sender,RoutedEventArgs e)=>WindowState=WindowState.Minimized;
        private void MaximizeButton_Click(object sender,RoutedEventArgs e)=>WindowState=WindowState==WindowState.Maximized?WindowState.Normal:WindowState.Maximized;
        private void CloseButton_Click(object sender,RoutedEventArgs e)=>Close();
        private void Window_StateChanged(object sender,EventArgs e)=>MaximizeButton.Content=WindowState==WindowState.Maximized?"❐":"□";
        private void ManageUsersButton_Click(object sender,RoutedEventArgs e)=>new UserManagerWindow{Owner=this}.ShowDialog();
        private void RegisterCoachButton_Click(object sender,RoutedEventArgs e){new CoachEditWindow{Owner=this}.ShowDialog();LoadFooterStats();}
        private void SearchCoachButton_Click(object sender,RoutedEventArgs e){new SearchCoachWindow{Owner=this}.ShowDialog();LoadFooterStats();}
        private void PaymentManagerButton_Click(object sender,RoutedEventArgs e)=>new PaymentManagerWindow{Owner=this}.ShowDialog();
        private void ManageFieldsButton_Click(object sender,RoutedEventArgs e)=>new CustomFieldManagerWindow{Owner=this}.ShowDialog();
        private void ExportZipButton_Click(object sender,RoutedEventArgs e)=>new ExportZipWindow{Owner=this}.ShowDialog();
        private void ExportPaymentsButton_Click(object sender,RoutedEventArgs e)=>new ExportPaymentWindow{Owner=this}.ShowDialog();
        private void ExportResumeButton_Click(object sender,RoutedEventArgs e)=>new ExportResumeWindow{Owner=this}.ShowDialog();
        private void ExportCoursesButton_Click(object sender,RoutedEventArgs e)=>new ExportCourseWindow{Owner=this}.ShowDialog();
        private void SettingsButton_Click(object sender,RoutedEventArgs e)=>new SettingsWindow{Owner=this}.ShowDialog();
        private void RestoreButton_Click(object sender,RoutedEventArgs e)=>new RestoreWindow{Owner=this}.ShowDialog();
        private void ChangeCredentialsButton_Click(object sender,RoutedEventArgs e)=>new ChangeCredentialsWindow{Owner=this}.ShowDialog();
        private void BackupButton_Click(object sender,RoutedEventArgs e)
        {
            var dialog=new SaveFileDialog{Filter="Zip Files|*.zip",FileName="backup_"+DateTime.Now.ToString("yyyyMMdd_HHmmss")+".zip"};
            if(dialog.ShowDialog()==true)
            {
                try
                {
                    ServiceContainer.Get<FileService>().BackupAllData(dialog.FileName);
                    MessageBox.Show("پشتیبان‌گیری با موفقیت انجام شد","موفق",MessageBoxButton.OK,MessageBoxImage.Information);
                }
                catch(Exception)
                {
                    MessageBox.Show("خطا در پشتیبان‌گیری","خطا",MessageBoxButton.OK,MessageBoxImage.Error);
                }
            }
        }
        private void LogoutButton_Click(object sender,RoutedEventArgs e)
        {
            Session.CurrentUser=null;
            new LoginWindow().Show();
            Close();
        }
    }
}