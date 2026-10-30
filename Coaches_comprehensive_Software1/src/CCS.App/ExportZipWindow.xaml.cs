using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Win32;
using CCS.Core;
using CCS.Services;
namespace CCS.App
{
    public partial class ExportZipWindow:Window
    {
        public class ExportItem
        {
            public Coach Coach{get;set;}
            public string FileTypePersian{get;set;}
            public bool IsSelected{get;set;}
        }
        private List<ExportItem> currentItems=new List<ExportItem>();
        public ExportZipWindow()
        {
            InitializeComponent();
            foreach(var item in FileTypes.PersianNames)FileTypeCombo.Items.Add(new ComboBoxItem{Content=item.Value,Tag=item.Key});
            SingleModeRadio_Checked(null,null);
        }
        private void ModeRadio_Checked(object sender,RoutedEventArgs e)
        {
            if(SinglePanel==null)return;
            SinglePanel.Visibility=SingleModeRadio.IsChecked==true?Visibility.Visible:Visibility.Collapsed;
            GroupPanel.Visibility=GroupModeRadio.IsChecked==true?Visibility.Visible:Visibility.Collapsed;
            if(AllModeRadio.IsChecked==true)
            {
                var service=ServiceContainer.Get<CoachService>();
                FillItems(service.Search(null,null,null,null));
            }
        }
        private void SingleModeRadio_Checked(object sender,RoutedEventArgs e)
        {
            ModeRadio_Checked(sender,e);
        }
        private void SearchButton_Click(object sender,RoutedEventArgs e)
        {
            var service=ServiceContainer.Get<CoachService>();
            FillItems(service.Search(FirstNameBox.Text.Trim(),LastNameBox.Text.Trim(),FatherNameBox.Text.Trim(),NationalCodeBox.Text.Trim()));
        }
        private void FillItems(IEnumerable<Coach> coaches)
        {
            currentItems=new List<ExportItem>();
            foreach(var c in coaches)
            {
                var persian=string.IsNullOrEmpty(c.FileType)?"":(FileTypes.PersianNames.ContainsKey(c.FileType)?FileTypes.PersianNames[c.FileType]:"");
                currentItems.Add(new ExportItem{Coach=c,FileTypePersian=persian,IsSelected=true});
            }
            ResultsGrid.ItemsSource=null;
            ResultsGrid.ItemsSource=currentItems;
        }
        private void ExportButton_Click(object sender,RoutedEventArgs e)
        {
            try
            {
                var service=ServiceContainer.Get<ExportService>();
                List<long> ids=new List<long>();
                if(SingleModeRadio.IsChecked==true)
                {
                    foreach(var item in currentItems)if(item.IsSelected)ids.Add(item.Coach.Id);
                }
                else if(GroupModeRadio.IsChecked==true)
                {
                    var fileType=(FileTypeCombo.SelectedItem as ComboBoxItem)?.Tag?.ToString();
                    if(string.IsNullOrEmpty(fileType))
                    {
                        ErrorText.Text="نوع پرونده را انتخاب کنید";
                        return;
                    }
                    ids=service.GetCoachIdsByFileType(fileType);
                }
                else
                {
                    ids=service.GetCoachIdsByFileType(null);
                }
                if(ids.Count==0)
                {
                    ErrorText.Text="هیچ مربی برای خروجی یافت نشد";
                    return;
                }
                var dialog=new SaveFileDialog{Filter="Zip Files|*.zip",FileName="coaches_export_"+DateTime.Now.ToString("yyyyMMdd_HHmmss")+".zip"};
                if(dialog.ShowDialog()==true)
                {
                    service.ExportZip(ids,dialog.FileName);
                    MessageBox.Show("خروجی ZIP با موفقیت ساخته شد","موفق",MessageBoxButton.OK,MessageBoxImage.Information);
                    Close();
                }
            }
            catch(Exception)
            {
                ErrorText.Text="خطا در ایجاد خروجی";
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