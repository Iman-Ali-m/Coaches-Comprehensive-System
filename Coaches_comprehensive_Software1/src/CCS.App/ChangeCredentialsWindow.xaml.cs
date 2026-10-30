using System;
using System.Windows;
using System.Windows.Input;
using CCS.Core;
using CCS.Services;
namespace CCS.App
{
    public partial class ChangeCredentialsWindow:Window
    {
        public ChangeCredentialsWindow()
        {
            InitializeComponent();
            CurrentUsernameBox.Text=Session.CurrentUser?.Username??"";
        }
        private void TitleBar_MouseLeftButtonDown(object sender,MouseButtonEventArgs e)
        {
            if(e.LeftButton==MouseButtonState.Pressed)DragMove();
        }
        private void Close_Click(object sender,RoutedEventArgs e)=>Close();
        private void Window_StateChanged(object sender,EventArgs e){}

        private void CancelButton_Click(object sender,RoutedEventArgs e)
        {
            Close();
        }
        private void Minimize_Click(object sender,RoutedEventArgs e)=>WindowState=WindowState.Minimized;
        private void Maximize_Click(object sender,RoutedEventArgs e)=>WindowState=WindowState==WindowState.Maximized?WindowState.Normal:WindowState.Maximized;

        private void ConfirmButton_Click(object sender,RoutedEventArgs e)
        {
            if(string.IsNullOrWhiteSpace(CurrentUsernameBox.Text))
            {
                ErrorText.Text="نام کاربری فعلی الزامی است";
                return;
            }
            if(string.IsNullOrEmpty(CurrentPasswordBox.Password))
            {
                ErrorText.Text="رمز عبور فعلی الزامی است";
                return;
            }
            if(string.IsNullOrWhiteSpace(NewUsernameBox.Text)&&string.IsNullOrEmpty(NewPasswordBox.Password))
            {
                ErrorText.Text="حداقل یکی از موارد جدید را وارد کنید";
                return;
            }
            if(!string.IsNullOrEmpty(NewPasswordBox.Password)&&NewPasswordBox.Password!=ConfirmNewPasswordBox.Password)
            {
                ErrorText.Text="رمز عبور جدید و تکرار آن یکسان نیست";
                return;
            }
            try
            {
                var userService=ServiceContainer.Get<UserService>();
                var user=userService.VerifyCredentials(CurrentUsernameBox.Text.Trim(),CurrentPasswordBox.Password);
                if(user==null)
                {
                    ErrorText.Text="نام کاربری یا رمز عبور فعلی صحیح نیست";
                    return;
                }
                var finalUsername=string.IsNullOrWhiteSpace(NewUsernameBox.Text)?user.Username:NewUsernameBox.Text.Trim();
                userService.UpdateUser(user.Id,finalUsername,NewPasswordBox.Password,user.Role);
                Session.CurrentUser=userService.VerifyCredentials(finalUsername,string.IsNullOrEmpty(NewPasswordBox.Password)?CurrentPasswordBox.Password:NewPasswordBox.Password);
                MessageBox.Show("تغییرات ذخیره شد","موفق",MessageBoxButton.OK,MessageBoxImage.Information);
                Close();
            }
            catch(Exception)
            {
                ErrorText.Text="خطا در تغییر مشخصات، دوباره تلاش کنید";
            }
        }
    }
}