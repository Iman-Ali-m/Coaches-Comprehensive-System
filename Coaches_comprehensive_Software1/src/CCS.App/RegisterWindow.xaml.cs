using System;
using System.Windows;
using System.Windows.Input;
using CCS.Core;
using CCS.Services;
namespace CCS.App
{
    public partial class RegisterWindow:Window
    {
        public RegisterWindow()
        {
            InitializeComponent();
        }
        private void RegisterButton_Click(object sender,RoutedEventArgs e)
        {
            if(string.IsNullOrWhiteSpace(UsernameBox.Text))
            {
                ErrorText.Text="نام کاربری الزامی است";
                return;
            }
            if(string.IsNullOrEmpty(PasswordBox.Password))
            {
                ErrorText.Text="رمز عبور الزامی است";
                return;
            }
            if(PasswordBox.Password!=ConfirmPasswordBox.Password)
            {
                ErrorText.Text="رمز عبور و تکرار آن یکسان نیست";
                return;
            }
            try
            {
                ServiceContainer.Get<UserService>().CreateUser(UsernameBox.Text.Trim(),PasswordBox.Password,"Manager");
                new MainWindow().Show();
                Close();
            }
            catch(Exception)
            {
                ErrorText.Text="خطا در ثبت نام، دوباره تلاش کنید";
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