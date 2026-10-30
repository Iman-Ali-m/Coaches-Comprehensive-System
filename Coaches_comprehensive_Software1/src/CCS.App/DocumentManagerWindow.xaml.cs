using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using CCS.Core;
using CCS.Services;
namespace CCS.App
{
    public partial class DocumentManagerWindow:Window
    {
        private readonly long currentCoachId;
        public class DocumentItem
        {
            public long Id{get;set;}
            public string CategoryPersian{get;set;}
            public string OriginalFileName{get;set;}
            public string StoredFileName{get;set;}
            public string UploadedAt{get;set;}
        }
        public DocumentManagerWindow(long coachId)
        {
            InitializeComponent();
            currentCoachId=coachId;
            LoadData();
        }
        private void LoadData()
        {
            var docs=ServiceContainer.Get<FileService>().GetDocumentsByCoachId(currentCoachId);
            var items=new List<DocumentItem>();
            foreach(var doc in docs)
            {
                var persianName=DocumentCategories.PersianNames.ContainsKey(doc.Category)?DocumentCategories.PersianNames[doc.Category]:doc.Category;
                items.Add(new DocumentItem{Id=doc.Id,CategoryPersian=persianName,OriginalFileName=doc.OriginalFileName,StoredFileName=doc.StoredFileName,UploadedAt=doc.UploadedAt});
            }
            DocumentsGrid.ItemsSource=items;
        }
        private void OpenButton_Click(object sender,RoutedEventArgs e)
        {
            var item=(DocumentItem)((FrameworkElement)sender).DataContext;
            try
            {
                ServiceContainer.Get<FileService>().OpenDocument(item.StoredFileName);
            }
            catch(Exception)
            {
                MessageBox.Show("خطا در باز کردن فایل","خطا",MessageBoxButton.OK,MessageBoxImage.Error);
            }
        }
        private void DeleteButton_Click(object sender,RoutedEventArgs e)
        {
            var item=(DocumentItem)((FrameworkElement)sender).DataContext;
            if(MessageBox.Show("آیا از حذف این فایل مطمئن هستید؟","تایید",MessageBoxButton.YesNo,MessageBoxImage.Warning)==MessageBoxResult.Yes)
            {
                ServiceContainer.Get<FileService>().DeleteDocument(item.Id,item.StoredFileName);
                LoadData();
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