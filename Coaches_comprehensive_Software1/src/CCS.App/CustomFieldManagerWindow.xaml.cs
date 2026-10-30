using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using CCS.Core;
using CCS.Services;
namespace CCS.App
{
    public partial class CustomFieldManagerWindow:Window
    {
        public class FieldItem
        {
            public long Id{get;set;}
            public string Title{get;set;}
            public string TypePersian{get;set;}
            public int DisplayOrder{get;set;}
            public CustomField Original{get;set;}
        }
        private string selectedTemplatePath;
        public CustomFieldManagerWindow()
        {
            InitializeComponent();
            foreach(var item in CustomFieldTypes.PersianNames)
            {
                TypeCombo.Items.Add(new ComboBoxItem{Content=item.Value,Tag=item.Key});
            }
            LoadData();
        }
        private void LoadData()
        {
            var items=new List<FieldItem>();
            foreach(var field in ServiceContainer.Get<CustomFieldService>().GetAll())
            {
                var persian=CustomFieldTypes.PersianNames.ContainsKey(field.FieldType)?CustomFieldTypes.PersianNames[field.FieldType]:field.FieldType;
                items.Add(new FieldItem{Id=field.Id,Title=field.Title,TypePersian=persian,DisplayOrder=field.DisplayOrder,Original=field});
            }
            FieldsGrid.ItemsSource=items;
        }
        private void PickTemplateBtn_Click(object sender,RoutedEventArgs e)
        {
            var dialog=new OpenFileDialog{Filter="All Files|*.*"};
            if(dialog.ShowDialog()==true)
            {
                selectedTemplatePath=dialog.FileName;
                TemplateText.Text=Path.GetFileName(dialog.FileName);
            }
        }
        private void AddFieldBtn_Click(object sender,RoutedEventArgs e)
        {
            if(string.IsNullOrWhiteSpace(TitleBox.Text))return;
            if(TypeCombo.SelectedItem==null)return;
            var field=new CustomField
            {
                Title=TitleBox.Text.Trim(),
                FieldType=(TypeCombo.SelectedItem as ComboBoxItem).Tag.ToString(),
                MinValue=MinBox.Text.Trim(),
                MaxValue=MaxBox.Text.Trim(),
                Options=OptionsBox.Text.Trim(),
                DisplayOrder=int.TryParse(OrderBox.Text,out var o)?o:0
            };
            if(!string.IsNullOrEmpty(selectedTemplatePath))
            {
                var stored=ServiceContainer.Get<FileService>().SaveToDisk(selectedTemplatePath);
                field.TemplateOriginalFileName=Path.GetFileName(selectedTemplatePath);
                field.TemplateStoredFileName=stored;
                field.TemplateFileSize=new FileInfo(selectedTemplatePath).Length;
            }
            ServiceContainer.Get<CustomFieldService>().Create(field);
            TitleBox.Text="";
            MinBox.Text="";
            MaxBox.Text="";
            OptionsBox.Text="";
            OrderBox.Text="";
            selectedTemplatePath=null;
            TemplateText.Text="";
            LoadData();
        }
        private void DeleteFieldBtn_Click(object sender,RoutedEventArgs e)
        {
            var item=(FieldItem)((FrameworkElement)sender).DataContext;
            if(MessageBox.Show("با حذف این ویژگی، مقادیر تمام مربیان برای آن نیز حذف می‌شود. مطمئن هستید؟","تایید",MessageBoxButton.YesNo,MessageBoxImage.Warning)==MessageBoxResult.Yes)
            {
                ServiceContainer.Get<CustomFieldService>().Delete(item.Id);
                LoadData();
            }
        }
    }
}