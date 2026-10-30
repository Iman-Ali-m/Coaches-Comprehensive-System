using System;
using System.Windows;
using System.Windows.Input;
using Microsoft.Win32;
using CCS.Core;
using CCS.Services;
namespace CCS.App
{
    public partial class RestoreWindow:Window
    {
        public RestoreWindow()
        {
            InitializeComponent();
        }
        private void BrowseButton_Click(object sender,RoutedEventArgs e)
        {
            var dialog=new OpenFileDialog{Filter="Zip Files|*.zip"};
            if(dialog.ShowDialog()==true)
            {
                PathBox.Text=dialog.FileName;
            }
        }
        private void RestoreButton_Click(object sender,RoutedEventArgs e)
        {
            if(string.IsNullOrWhiteSpace(PathBox.Text))
            {
                ErrorText.Text="ابتدا فایل پشتیبان را انتخاب کنید";
                return;
            }
            if(MessageBox.Show("آیا از بازگردانی اطلاعات مطمئن هستید؟ اطلاعات فعلی حذف می‌شوند.","تایید",MessageBoxButton.YesNo,MessageBoxImage.Warning)!=MessageBoxResult.Yes)return;
            try
            {
                ServiceContainer.Get<RestoreService>().Restore(PathBox.Text);
                MessageBox.Show("اطلاعات با موفقیت بازگردانی شد. برنامه مجددا اجرا می‌شود.","موفق",MessageBoxButton.OK,MessageBoxImage.Information);
                System.Diagnostics.Process.Start(Application.ResourceAssembly.Location);
                Application.Current.Shutdown();
            }
            catch(Exception)
            {
                ErrorText.Text="خطا در بازگردانی اطلاعات، فایل ممکن است معتبر نباشد";
            }
        }
        private void CancelButton_Click(object sender,RoutedEventArgs e)
        {
            Close();
        }
        private void TitleBar_MouseLeftButtonDown(object sender,MouseButtonEventArgs e)
        {
            if(e.ClickCount==2)WindowState=WindowState==WindowState.Maximized?WindowState.Normal:WindowState.Maximized;
            else if(e.LeftButton==MouseButtonState.Pressed)DragMove();
        }
        private void Minimize_Click(object sender,RoutedEventArgs e)=>WindowState=WindowState.Minimized;
        private void Maximize_Click(object sender,RoutedEventArgs e)=>WindowState=WindowState==WindowState.Maximized?WindowState.Normal:WindowState.Maximized;
        private void Close_Click(object sender,RoutedEventArgs e)=>Close();
        private void Window_StateChanged(object sender,System.EventArgs e)
        {
            if(MaximizeBtn!=null)MaximizeBtn.Content=WindowState==WindowState.Maximized?"❐":"□";
        }
    }
}