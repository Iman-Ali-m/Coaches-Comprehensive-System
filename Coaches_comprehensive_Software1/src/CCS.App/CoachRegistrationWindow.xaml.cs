using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Win32;
using CCS.Core;
using CCS.Services;
namespace CCS.App
{
    public partial class CoachRegistrationWindow:Window
    {
        private readonly Dictionary<string,string> selectedFiles=new Dictionary<string,string>();
        public CoachRegistrationWindow()
        {
            InitializeComponent();
            foreach(var item in FileTypes.PersianNames)
            {
                FileTypeCombo.Items.Add(new ComboBoxItem{Content=item.Value,Tag=item.Key});
            }
        }
        private void PickFile(string category,TextBlock target)
        {
            var dialog=new OpenFileDialog{Filter="All Files|*.*"};
            if(dialog.ShowDialog()==true)
            {
                selectedFiles[category]=dialog.FileName;
                target.Text=Path.GetFileName(dialog.FileName);
            }
        }
        private void UploadNationalCardBtn_Click(object sender,RoutedEventArgs e)=>PickFile(DocumentCategories.NationalCardFrontBack,NationalCardText);
        private void UploadBirthCertBtn_Click(object sender,RoutedEventArgs e)=>PickFile(DocumentCategories.BirthCertificateAllPages,BirthCertText);
        private void UploadPhotoBtn_Click(object sender,RoutedEventArgs e)=>PickFile(DocumentCategories.PersonalPhoto,PhotoText);
        private void UploadMilitaryBtn_Click(object sender,RoutedEventArgs e)=>PickFile(DocumentCategories.MilitaryServiceCard,MilitaryText);
        private void UploadDegreeBtn_Click(object sender,RoutedEventArgs e)=>PickFile(DocumentCategories.LastDegreeCertificate,DegreeText);
        private void SaveButton_Click(object sender,RoutedEventArgs e)
        {
            try
            {
                var coachService=ServiceContainer.Get<CoachService>();
                var fileService=ServiceContainer.Get<FileService>();
                var coach=new Coach
                {
                    FirstName=FirstNameBox.Text.Trim(),
                    LastName=LastNameBox.Text.Trim(),
                    FatherName=FatherNameBox.Text.Trim(),
                    NationalCode=NationalCodeBox.Text.Trim(),
                    BirthCertificateNumber=BirthCertificateNumberBox.Text.Trim(),
                    PersonnelCode=PersonnelCodeBox.Text.Trim(),
                    BirthDate=BirthDatePicker.SelectedDate?.ToString("yyyy-MM-dd"),
                    Gender=(GenderCombo.SelectedItem as ComboBoxItem)?.Content?.ToString(),
                    MaritalStatus=(MaritalStatusCombo.SelectedItem as ComboBoxItem)?.Content?.ToString(),
                    ChildrenCount=int.TryParse(ChildrenCountBox.Text,out var cc)?cc:null,
                    FieldOfStudy=FieldOfStudyBox.Text.Trim(),
                    Education=EducationBox.Text.Trim(),
                    MembershipType=MembershipTypeBox.Text.Trim(),
                    EntryDate=EntryDatePicker.SelectedDate?.ToString("yyyy-MM-dd"),
                    LastPosition=LastPositionBox.Text.Trim(),
                    FileType=(FileTypeCombo.SelectedItem as ComboBoxItem)?.Tag?.ToString(),
                    DistrictName=DistrictNameBox.Text.Trim(),
                    MilitaryStatus=MilitaryStatusBox.Text.Trim(),
                    PhysicalStatus=PhysicalStatusBox.Text.Trim(),
                    Weight=double.TryParse(WeightBox.Text,out var w)?w:null,
                    Height=double.TryParse(HeightBox.Text,out var h)?h:null,
                    BloodType=BloodTypeBox.Text.Trim(),
                    SacrificeStatus=SacrificeStatusBox.Text.Trim(),
                    Job=JobBox.Text.Trim(),
                    WorkAddress=WorkAddressBox.Text.Trim(),
                    HomeAddress=HomeAddressBox.Text.Trim(),
                    Phone1=Phone1Box.Text.Trim(),
                    Phone2=Phone2Box.Text.Trim()
                };
                var coachId=coachService.Create(coach);
                foreach(var file in selectedFiles)
                {
                    var stored=fileService.SaveToDisk(file.Value);
                    fileService.RegisterDocument(coachId,file.Key,Path.GetFileName(file.Value),stored,new FileInfo(file.Value).Length);
                }
                MessageBox.Show("اطلاعات مربی با موفقیت ذخیره شد","موفق",MessageBoxButton.OK,MessageBoxImage.Information);
                var result=MessageBox.Show("اطلاعات ثبت شد. آیا مایل به تکمیل مدارک سازمانی هستید؟","سوال",MessageBoxButton.YesNo,MessageBoxImage.Question);
                if(result==MessageBoxResult.Yes)
                {
                    new CoachFormWindow(coachId){Owner=this}.ShowDialog();
                }
                DialogResult=true;
                Close();
            }
            catch(Exception)
            {
                ErrorText.Text="خطا در ذخیره اطلاعات، دوباره تلاش کنید";
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

