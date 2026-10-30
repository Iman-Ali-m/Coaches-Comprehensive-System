using System;
using System.Windows;
using System.Windows.Input;
using CCS.Core;
using CCS.Services;
namespace CCS.App
{
    public partial class LoginWindow:Window
    {
        public LoginWindow()
        {
            InitializeComponent();
        }
        private void TitleBar_MouseLeftButtonDown(object sender,MouseButtonEventArgs e)
        {
            if(e.LeftButton==MouseButtonState.Pressed)DragMove();
        }
        private void Close_Click(object sender,RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
        private void Window_StateChanged(object sender,EventArgs e){}
        private void Minimize_Click(object sender,RoutedEventArgs e)=>WindowState=WindowState.Minimized;
        private void Maximize_Click(object sender,RoutedEventArgs e)=>WindowState=WindowState==WindowState.Maximized?WindowState.Normal:WindowState.Maximized;
        private void LoginButton_Click(object sender,RoutedEventArgs e)
        {
            if(string.IsNullOrWhiteSpace(UsernameBox.Text)||string.IsNullOrEmpty(PasswordBox.Password))
            {
                ErrorText.Text="نام کاربری و رمز عبور الزامی است";
                return;
            }
            try
            {
                var user=ServiceContainer.Get<UserService>().VerifyCredentials(UsernameBox.Text.Trim(),PasswordBox.Password);
                if(user!=null)
                {
                    Session.CurrentUser=user;
                    new MainWindow().Show();
                    Close();
                    return;
                }
                ErrorText.Text="نام کاربری یا رمز عبور صحیح نیست";
            }
            catch(Exception ex)
            {
                ErrorText.Text="خطای سیستمی: " + ex.Message;
            }
        }
        private void ChangeButton_Click(object sender,RoutedEventArgs e)
        {
            new ChangeCredentialsWindow{Owner=this}.ShowDialog();
        }
    }
}