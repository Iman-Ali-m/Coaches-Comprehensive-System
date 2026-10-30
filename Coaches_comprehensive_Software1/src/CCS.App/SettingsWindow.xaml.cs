using System;
using System.Diagnostics;
using System.Windows;
using CCS.Core;
using CCS.Services;
namespace CCS.App
{
    public partial class SettingsWindow:Window
    {
        public SettingsWindow()
        {
            InitializeComponent();
            var current=ServiceContainer.Get<SettingsService>().GetValue("AutoBackupDays");
            IntervalBox.Text=string.IsNullOrEmpty(current)?"7":current;
        }
        private void OpenLogsButton_Click(object sender,RoutedEventArgs e)
        {
            Process.Start(new ProcessStartInfo(AppPaths.Logs){UseShellExecute=true});
        }
        private void OpenBackupsButton_Click(object sender,RoutedEventArgs e)
        {
            Process.Start(new ProcessStartInfo(AppPaths.Backups){UseShellExecute=true});
        }
        private void TitleBar_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if(e.ClickCount==2)WindowState=WindowState==WindowState.Maximized?WindowState.Normal:WindowState.Maximized;
            else if(e.LeftButton==System.Windows.Input.MouseButtonState.Pressed)DragMove();
        }
        private void Minimize_Click(object sender, System.Windows.RoutedEventArgs e)=>WindowState=WindowState.Minimized;
        private void Maximize_Click(object sender, System.Windows.RoutedEventArgs e)=>WindowState=WindowState==WindowState.Maximized?WindowState.Normal:WindowState.Maximized;
        private void Close_Click(object sender, System.Windows.RoutedEventArgs e)=>Close();
        private void Window_StateChanged(object sender,System.EventArgs e)
        {
            if(MaximizeBtn!=null)MaximizeBtn.Content=WindowState==WindowState.Maximized?"❐":"□";
        }
        private void SaveButton_Click(object sender,RoutedEventArgs e)
        {
            if(!int.TryParse(IntervalBox.Text,out var days)||days<1)
            {
                ErrorText.Text="بازه زمانی باید یک عدد صحیح بزرگتر از صفر باشد";
                return;
            }
            try
            {
                ServiceContainer.Get<SettingsService>().SetValue("AutoBackupDays",days.ToString());
                MessageBox.Show("تنظیمات با موفقیت ذخیره شد","موفق",MessageBoxButton.OK,MessageBoxImage.Information);
                Close();
            }
            catch(Exception)
            {
                ErrorText.Text="خطا در ذخیره تنظیمات";
            }
        }
    }
}