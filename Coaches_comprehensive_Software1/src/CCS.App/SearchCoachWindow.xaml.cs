using System.Windows;
using System.Windows.Input;
using CCS.Core;
using CCS.Services;
namespace CCS.App
{
    public partial class SearchCoachWindow:Window
    {
        public SearchCoachWindow()
        {
            InitializeComponent();
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

        private void SearchButton_Click(object sender,RoutedEventArgs e)
        {
            var service=ServiceContainer.Get<CoachService>();
            ResultsGrid.ItemsSource=service.Search(FirstNameBox.Text.Trim(),LastNameBox.Text.Trim(),FatherNameBox.Text.Trim(),NationalCodeBox.Text.Trim());
        }
        private void EditButton_Click(object sender,RoutedEventArgs e)
        {
            var coach=(Coach)((FrameworkElement)sender).DataContext;
            var window=new CoachEditWindow(coach.Id){Owner=this};
            if(window.ShowDialog()==true)
            {
                SearchButton_Click(null,null);
            }
        }
        private void DeleteButton_Click(object sender,RoutedEventArgs e)
        {
            var coach=(Coach)((FrameworkElement)sender).DataContext;
            if(MessageBox.Show("آیا از حذف این مربی و تمام اطلاعات مرتبط مطمئن هستید؟","تایید",MessageBoxButton.YesNo,MessageBoxImage.Warning)==MessageBoxResult.Yes)
            {
                ServiceContainer.Get<CoachService>().Delete(coach.Id);
                SearchButton_Click(null,null);
            }
        }
    }
}