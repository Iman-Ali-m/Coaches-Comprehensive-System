using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CCS.Core;
using CCS.Services;
namespace CCS.App
{
    public partial class UserManagerWindow:Window
    {
        public UserManagerWindow()
        {
            InitializeComponent();
            LoadData();
        }
        private void TitleBar_MouseLeftButtonDown(object sender,MouseButtonEventArgs e)
        {
            if(e.ClickCount==2)WindowState=WindowState==WindowState.Maximized?WindowState.Normal:WindowState.Maximized;
            else if(e.LeftButton==MouseButtonState.Pressed)DragMove();
        }
        private void Minimize_Click(object sender,RoutedEventArgs e)=>WindowState=WindowState.Minimized;
        private void Maximize_Click(object sender,RoutedEventArgs e)=>WindowState=WindowState==WindowState.Maximized?WindowState.Normal:WindowState.Maximized;
        private void Close_Click(object sender,RoutedEventArgs e)=>Close();
        private void Window_StateChanged(object sender,EventArgs e)
        {
            if(MaximizeBtn!=null)MaximizeBtn.Content=WindowState==WindowState.Maximized?"❐":"□";
        }
        private void LoadData()
        {
            UsersGrid.ItemsSource=ServiceContainer.Get<UserService>().GetAllUsers();
        }
        private void AddUser_Click(object sender,RoutedEventArgs e)
        {
            if(string.IsNullOrWhiteSpace(UsernameBox.Text)||string.IsNullOrEmpty(PasswordBox.Password))return;
            var role=(RoleCombo.SelectedItem as ComboBoxItem)?.Tag?.ToString();
            try
            {
                ServiceContainer.Get<UserService>().CreateUser(UsernameBox.Text.Trim(),PasswordBox.Password,role);
                UsernameBox.Text="";
                PasswordBox.Password="";
                LoadData();
            }
            catch(Exception)
            {
                MessageBox.Show("خطا در ایجاد کاربر. احتمالا نام کاربری تکراری است.","خطا",MessageBoxButton.OK,MessageBoxImage.Error);
            }
        }
        private void DeleteUser_Click(object sender,RoutedEventArgs e)
        {
            var user=(User)((FrameworkElement)sender).DataContext;
            if(Session.CurrentUser.Role!="Admin"&&user.Role=="Admin")
            {
                MessageBox.Show("شما اجازه حذف ادمین را ندارید.","عدم دسترسی",MessageBoxButton.OK,MessageBoxImage.Warning);
                return;
            }
            if(user.Id==Session.CurrentUser.Id)
            {
                MessageBox.Show("نمی‌توانید خودتان را حذف کنید.","خطا",MessageBoxButton.OK,MessageBoxImage.Warning);
                return;
            }
            if(MessageBox.Show("آیا مطمئن هستید؟","تایید",MessageBoxButton.YesNo,MessageBoxImage.Warning)==MessageBoxResult.Yes)
            {
                try
                {
                    ServiceContainer.Get<UserService>().DeleteUser(user.Id);
                    LoadData();
                }
                catch(Exception)
                {
                    MessageBox.Show("خطا در حذف کاربر.","خطا",MessageBoxButton.OK,MessageBoxImage.Error);
                }
            }
        }
    }
}