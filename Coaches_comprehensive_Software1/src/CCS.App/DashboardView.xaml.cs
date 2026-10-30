using System.Windows.Controls;
using CCS.Core;
using CCS.Services;
namespace CCS.App
{
    public partial class DashboardView:UserControl
    {
        public DashboardView()
        {
            InitializeComponent();
            Refresh();
        }
        public void Refresh()
        {
            var service=ServiceContainer.Get<StatsService>();
            TotalCoachesText.Text=service.GetTotalCoaches().ToString();
            TotalCoursesText.Text=service.GetTotalCourses().ToString();
            TotalPaymentsText.Text=service.GetTotalPayments().ToString();
        }
    }
}