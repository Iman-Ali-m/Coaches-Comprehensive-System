using System.Windows;
using CCS.Core;
using CCS.Services;
using System.Windows.Input;
namespace CCS.App
{
    public partial class PaymentManagerWindow:Window
    {
        public PaymentManagerWindow()
        {
            InitializeComponent();
        }
        private void SearchButton_Click(object sender,RoutedEventArgs e)
        {
            var service=ServiceContainer.Get<CoachService>();
            ResultsGrid.ItemsSource=service.Search(FirstNameBox.Text.Trim(),LastNameBox.Text.Trim(),FatherNameBox.Text.Trim(),NationalCodeBox.Text.Trim());
        }
        private void ResultsGrid_MouseDoubleClick(object sender,System.Windows.Input.MouseButtonEventArgs e)
        {
            if(ResultsGrid.SelectedItem is Coach coach)
            {
                OpenPayments(coach);
            }
        }
        private void ManagePaymentsButton_Click(object sender,RoutedEventArgs e)
        {
            var coach=(Coach)((FrameworkElement)sender).DataContext;
            OpenPayments(coach);
        }
        private void OpenPayments(Coach coach)
        {
            var window=new CoachEditWindow(coach.Id,6){Owner=this};
            window.ShowDialog();
            SearchButton_Click(null,null);
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