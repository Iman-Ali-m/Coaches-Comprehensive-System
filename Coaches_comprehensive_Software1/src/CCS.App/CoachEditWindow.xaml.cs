using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Input;
using Microsoft.Win32;
using CCS.Core;
using CCS.Services;
namespace CCS.App
{
    public partial class CoachEditWindow:Window
    {
        private long? currentCoachId;
        private readonly Dictionary<string,string> selectedFiles=new Dictionary<string,string>();
        private readonly List<string> selectedPhotos=new List<string>();
        private readonly List<CourseViewItem> pendingCourses=new List<CourseViewItem>();
        private readonly List<BankAccount> pendingBanks=new List<BankAccount>();
        private readonly List<ChessViewItem> pendingChess=new List<ChessViewItem>();
        private readonly List<PaymentViewItem> pendingPayments=new List<PaymentViewItem>();
        private string currentPaymentFilePath;
        private readonly Dictionary<long,string> dynamicFilePaths=new Dictionary<long,string>();
        private string currentCourseFilePath;
        private string currentChessFilePath;

        public class CourseViewItem
        {
            public CoachingCourse Course{get;set;}
            public string FilePath{get;set;}
            public bool HasFile=>!string.IsNullOrEmpty(FilePath);
        }
        public class ChessViewItem
        {
            public ChessTrainingRecord Record{get;set;}
            public string FilePath{get;set;}
            public bool HasFile=>!string.IsNullOrEmpty(FilePath);
        }

        public CoachEditWindow(long? coachId=null,int initialTabIndex=0)
        {
            InitializeComponent();
            currentCoachId=coachId;
            foreach(var item in FileTypes.PersianNames)FileTypeCombo.Items.Add(new ComboBoxItem{Content=item.Value,Tag=item.Key});
            foreach(var item in CollaborationStatus.PersianNames)CollaborationStatusCombo.Items.Add(new ComboBoxItem{Content=item.Value,Tag=item.Key});
            foreach(var item in CourseTypes.PersianNames)CourseTypeCombo.Items.Add(new ComboBoxItem{Content=item.Value,Tag=item.Key});
            
            if(currentCoachId.HasValue)LoadData();
            if(initialTabIndex>0)
            {
                foreach(var child in ((Grid)Content).Children)
                {
                    if(child is TabControl tc)tc.SelectedIndex=initialTabIndex;
                }
            }
        }

        private void LoadData()
        {
            var coach=ServiceContainer.Get<CoachService>().GetById(currentCoachId.Value);
            if(coach==null)return;
            
            FirstNameBox.Text=coach.FirstName;
            LastNameBox.Text=coach.LastName;
            FatherNameBox.Text=coach.FatherName;
            NationalCodeBox.Text=coach.NationalCode;
            BirthCertificateNumberBox.Text=coach.BirthCertificateNumber;
            PersonnelCodeBox.Text=coach.PersonnelCode;
            if(!string.IsNullOrEmpty(coach.BirthDate))BirthDatePicker.SelectedDate=DateTime.Parse(coach.BirthDate);
            foreach(ComboBoxItem item in GenderCombo.Items)if(item.Content.ToString()==coach.Gender)GenderCombo.SelectedItem=item;
            foreach(ComboBoxItem item in MaritalStatusCombo.Items)if(item.Content.ToString()==coach.MaritalStatus)MaritalStatusCombo.SelectedItem=item;
            ChildrenCountBox.Text=coach.ChildrenCount?.ToString();
            FieldOfStudyBox.Text=coach.FieldOfStudy;
            EducationBox.Text=coach.Education;
            MembershipTypeBox.Text=coach.MembershipType;
            if(!string.IsNullOrEmpty(coach.EntryDate))EntryDatePicker.SelectedDate=DateTime.Parse(coach.EntryDate);
            LastPositionBox.Text=coach.LastPosition;
            foreach(ComboBoxItem item in FileTypeCombo.Items)if(item.Tag?.ToString()==coach.FileType)FileTypeCombo.SelectedItem=item;
            DistrictNameBox.Text=coach.DistrictName;
            MilitaryStatusBox.Text=coach.MilitaryStatus;
            PhysicalStatusBox.Text=coach.PhysicalStatus;
            WeightBox.Text=coach.Weight?.ToString();
            HeightBox.Text=coach.Height?.ToString();
            BloodTypeBox.Text=coach.BloodType;
            SacrificeStatusBox.Text=coach.SacrificeStatus;
            JobBox.Text=coach.Job;
            WorkAddressBox.Text=coach.WorkAddress;
            HomeAddressBox.Text=coach.HomeAddress;
            Phone1Box.Text=coach.Phone1;
            Phone2Box.Text=coach.Phone2;
            
            var form=ServiceContainer.Get<CoachFormService>().GetByCoachId(currentCoachId.Value);
            if(form!=null)
            {
                CoachLevelBox.Text=form.CoachLevel?.ToString();
                CoachBaseBox.Text=form.CoachBase?.ToString();
                PhysicalFileNumberBox.Text=form.PhysicalFileNumber?.ToString();
                foreach(ComboBoxItem item in CollaborationStatusCombo.Items)if(item.Tag?.ToString()==form.CollaborationStatus)CollaborationStatusCombo.SelectedItem=item;
            }

            foreach(var course in ServiceContainer.Get<CoachingCourseService>().GetByCoachId(currentCoachId.Value))
            {
                var file=ServiceContainer.Get<CoachingCourseService>().GetFirstFileByCourseId(course.Id);
                pendingCourses.Add(new CourseViewItem{Course=course,FilePath=file?.StoredFileName});
            }
            CoursesGrid.ItemsSource=null;
            CoursesGrid.ItemsSource=pendingCourses;
            
            pendingBanks.AddRange(ServiceContainer.Get<BankAccountService>().GetByCoachId(currentCoachId.Value));
            BankGrid.ItemsSource=null;
            BankGrid.ItemsSource=pendingBanks;
            
            foreach(var chess in ServiceContainer.Get<ChessTrainingService>().GetByCoachId(currentCoachId.Value))
            {
                var file=ServiceContainer.Get<ChessTrainingService>().GetFirstFileByRecordId(chess.Id);
                pendingChess.Add(new ChessViewItem{Record=chess,FilePath=file?.StoredFileName});
            }
            ChessGrid.ItemsSource=null;
            ChessGrid.ItemsSource=pendingChess;
            
            foreach(var pay in ServiceContainer.Get<PaymentService>().GetByCoachId(currentCoachId.Value))
            {
                var file=ServiceContainer.Get<PaymentService>().GetFirstFileByPaymentId(pay.Id);
                pendingPayments.Add(new PaymentViewItem{Payment=pay,FilePath=file?.StoredFileName});
            }
            PaymentsGrid.ItemsSource=null;
            PaymentsGrid.ItemsSource=pendingPayments;

            LoadDynamicFields();
        }

        private void PickCourseFileBtn_Click(object sender,RoutedEventArgs e)
        {
            var dialog=new OpenFileDialog{Filter="All Files|*.*"};
            if(dialog.ShowDialog()==true)
            {
                currentCourseFilePath=dialog.FileName;
                CourseFileText.Text=Path.GetFileName(dialog.FileName);
            }
        }
        private void PickChessFileBtn_Click(object sender,RoutedEventArgs e)
        {
            var dialog=new OpenFileDialog{Filter="All Files|*.*"};
            if(dialog.ShowDialog()==true)
            {
                currentChessFilePath=dialog.FileName;
                ChessFileText.Text=Path.GetFileName(dialog.FileName);
            }
        }
        private void ViewCourseFileBtn_Click(object sender,RoutedEventArgs e)
        {
            var item=(CourseViewItem)((FrameworkElement)sender).DataContext;
            if(item.HasFile)ServiceContainer.Get<FileService>().OpenDocument(item.FilePath);
        }
        private void ViewChessFileBtn_Click(object sender,RoutedEventArgs e)
        {
            var item=(ChessViewItem)((FrameworkElement)sender).DataContext;
            if(item.HasFile)ServiceContainer.Get<FileService>().OpenDocument(item.FilePath);
        }

        private void PickSingleFile(string category,TextBlock target,string filter)
        {
            var dialog=new OpenFileDialog{Filter=filter};
            if(dialog.ShowDialog()==true)
            {
                selectedFiles[category]=dialog.FileName;
                target.Text=Path.GetFileName(dialog.FileName);
            }
        }
        private void UploadNationalCardBtn_Click(object sender,RoutedEventArgs e)=>PickSingleFile(DocumentCategories.NationalCardFrontBack,NationalCardText,"All Files|*.*");
        private void UploadBirthCertBtn_Click(object sender,RoutedEventArgs e)=>PickSingleFile(DocumentCategories.BirthCertificateAllPages,BirthCertText,"All Files|*.*");
        private void UploadPhotoBtn_Click(object sender,RoutedEventArgs e)=>PickSingleFile(DocumentCategories.PersonalPhoto,PhotoText,"All Files|*.*");
        private void UploadMilitaryBtn_Click(object sender,RoutedEventArgs e)=>PickSingleFile(DocumentCategories.MilitaryServiceCard,MilitaryText,"All Files|*.*");
        private void UploadDegreeBtn_Click(object sender,RoutedEventArgs e)=>PickSingleFile(DocumentCategories.LastDegreeCertificate,DegreeText,"All Files|*.*");
        private void UploadPublicOfficialBtn_Click(object sender,RoutedEventArgs e)=>PickSingleFile(DocumentCategories.PublicOfficial,PublicOfficialText,"All Files|*.*");
        private void UploadMembershipConversionBtn_Click(object sender,RoutedEventArgs e)=>PickSingleFile(DocumentCategories.MembershipConversion,MembershipConversionText,"All Files|*.*");
        private void UploadPreliminaryRankBtn_Click(object sender,RoutedEventArgs e)=>PickSingleFile(DocumentCategories.PreliminaryRank,PreliminaryRankText,"All Files|*.*");
        private void UploadAdvancedRankBtn_Click(object sender,RoutedEventArgs e)=>PickSingleFile(DocumentCategories.AdvancedRank,AdvancedRankText,"All Files|*.*");
        private void UploadLastPositionOrderBtn_Click(object sender,RoutedEventArgs e)=>PickSingleFile(DocumentCategories.LastPositionOrder,LastPositionOrderText,"All Files|*.*");
        private void UploadLastRankDelegationBtn_Click(object sender,RoutedEventArgs e)=>PickSingleFile(DocumentCategories.LastRankDelegation,LastRankDelegationText,"All Files|*.*");
        private void UploadLastRankCategoryDelegationBtn_Click(object sender,RoutedEventArgs e)=>PickSingleFile(DocumentCategories.LastRankCategoryDelegation,LastRankCategoryDelegationText,"All Files|*.*");
        private void UploadMedicalExemptionDocumentsBtn_Click(object sender,RoutedEventArgs e)=>PickSingleFile(DocumentCategories.MedicalExemptionDocuments,MedicalExemptionDocumentsText,"All Files|*.*");
        private void UploadCartexBtn_Click(object sender,RoutedEventArgs e)=>PickSingleFile(DocumentCategories.Cartex,CartexText,"Word Files|*.doc;*.docx");
        
        private void UploadCoachFormPhotoBtn_Click(object sender,RoutedEventArgs e)
        {
            var dialog=new OpenFileDialog{Filter="Image Files|*.jpg;*.jpeg;*.png;*.bmp",Multiselect=true};
            if(dialog.ShowDialog()==true)
            {
                foreach(var file in dialog.FileNames)
                {
                    selectedPhotos.Add(file);
                    CoachFormPhotosList.Items.Add(Path.GetFileName(file));
                }
            }
        }

        private void AddCourseBtn_Click(object sender,RoutedEventArgs e)
        {
            var course=new CoachingCourse
            {
                CourseType=(CourseTypeCombo.SelectedItem as ComboBoxItem)?.Tag?.ToString(),
                CourseName=CourseNameBox.Text.Trim(),
                StartDate=CourseStartPicker.SelectedDate?.ToString("yyyy-MM-dd"),
                EndDate=CourseEndPicker.SelectedDate?.ToString("yyyy-MM-dd"),
                DaysCount=int.TryParse(CourseDaysBox.Text,out var cd)?cd:null,
                Description=CourseDescBox.Text.Trim()
            };
            pendingCourses.Add(new CourseViewItem{Course=course,FilePath=currentCourseFilePath});
            CoursesGrid.ItemsSource=null;
            CoursesGrid.ItemsSource=pendingCourses;
            CourseNameBox.Text="";
            CourseStartPicker.SelectedDate=null;
            CourseEndPicker.SelectedDate=null;
            CourseDaysBox.Text="";
            CourseDescBox.Text="";
            currentCourseFilePath=null;
            CourseFileText.Text="";
        }
        private void DeleteCourseBtn_Click(object sender,RoutedEventArgs e)
        {
            var item=(CourseViewItem)((FrameworkElement)sender).DataContext;
            if(item.Course.Id>0)ServiceContainer.Get<CoachingCourseService>().Delete(item.Course.Id);
            pendingCourses.Remove(item);
            CoursesGrid.ItemsSource=null;
            CoursesGrid.ItemsSource=pendingCourses;
        }

        private void AddBankBtn_Click(object sender,RoutedEventArgs e)
        {
            var bank=new BankAccount
            {
                BankName=BankNameBox.Text.Trim(),
                AccountNumber=AccountNumberBox.Text.Trim(),
                IBAN=IBANBox.Text.Trim(),
                CardNumber=CardNumberBox.Text.Trim()
            };
            pendingBanks.Add(bank);
            BankGrid.ItemsSource=null;
            BankGrid.ItemsSource=pendingBanks;
            BankNameBox.Text="";
            AccountNumberBox.Text="";
            IBANBox.Text="";
            CardNumberBox.Text="";
        }
        private void DeleteBankBtn_Click(object sender,RoutedEventArgs e)
        {
            var bank=(BankAccount)((FrameworkElement)sender).DataContext;
            if(bank.Id>0)ServiceContainer.Get<BankAccountService>().Delete(bank.Id);
            pendingBanks.Remove(bank);
            BankGrid.ItemsSource=null;
            BankGrid.ItemsSource=pendingBanks;
        }

        private void AddChessBtn_Click(object sender,RoutedEventArgs e)
        {
            var chess=new ChessTrainingRecord
            {
                Year=int.TryParse(ChessYearBox.Text,out var cy)?cy:null,
                ChessType=ChessTypeBox.Text.Trim(),
                PlayDate=ChessPlayDatePicker.SelectedDate?.ToString("yyyy-MM-dd"),
                Score=double.TryParse(ChessScoreBox.Text,out var cs)?cs:null,
                Description=ChessDescBox.Text.Trim()
            };
            pendingChess.Add(new ChessViewItem{Record=chess,FilePath=currentChessFilePath});
            ChessGrid.ItemsSource=null;
            ChessGrid.ItemsSource=pendingChess;
            ChessYearBox.Text="";
            ChessTypeBox.Text="";
            ChessPlayDatePicker.SelectedDate=null;
            ChessScoreBox.Text="";
            ChessDescBox.Text="";
            currentChessFilePath=null;
            ChessFileText.Text="";
        }
        private void DeleteChessBtn_Click(object sender,RoutedEventArgs e)
        {
            var item=(ChessViewItem)((FrameworkElement)sender).DataContext;
            if(item.Record.Id>0)ServiceContainer.Get<ChessTrainingService>().Delete(item.Record.Id);
            pendingChess.Remove(item);
            ChessGrid.ItemsSource=null;
            ChessGrid.ItemsSource=pendingChess;
        }

        private void AddPaymentBtn_Click(object sender,RoutedEventArgs e)
        {
            var payment=new Payment
            {
                Subject=PaySubjectBox.Text.Trim(),
                PaymentDate=PayDatePicker.SelectedDate?.ToString("yyyy-MM-dd"),
                StartTime=PayStartBox.Text.Trim(),
                EndTime=PayEndBox.Text.Trim(),
                Location=PayLocationBox.Text.Trim(),
                DaysCount=int.TryParse(PayDaysBox.Text,out var pd)?pd:null,
                ClassesCount=int.TryParse(PayClassesBox.Text,out var pc)?pc:null,
                PaidAmount=long.TryParse(PayPaidBox.Text,out var pp)?pp:null,
                ReturnedAmount=long.TryParse(PayReturnedBox.Text,out var pr)?pr:null,
                PaidDate=PayPaidDatePicker.SelectedDate?.ToString("yyyy-MM-dd")
            };
            pendingPayments.Add(new PaymentViewItem{Payment=payment,FilePath=currentPaymentFilePath});
            PaymentsGrid.ItemsSource=null;
            PaymentsGrid.ItemsSource=pendingPayments;
            PaySubjectBox.Text="";
            PayDatePicker.SelectedDate=null;
            PayStartBox.Text="";
            PayEndBox.Text="";
            PayLocationBox.Text="";
            PayDaysBox.Text="";
            PayClassesBox.Text="";
            PayPaidBox.Text="";
            PayReturnedBox.Text="";
            PayPaidDatePicker.SelectedDate=null;
            currentPaymentFilePath=null;
            PaymentFileText.Text="";
        }
        private void DeletePaymentBtn_Click(object sender,RoutedEventArgs e)
        {
            var item=(PaymentViewItem)((FrameworkElement)sender).DataContext;
            if(item.Payment.Id>0)ServiceContainer.Get<PaymentService>().Delete(item.Payment.Id);
            pendingPayments.Remove(item);
            PaymentsGrid.ItemsSource=null;
            PaymentsGrid.ItemsSource=pendingPayments;
        }

        private void SaveButton_Click(object sender,RoutedEventArgs e)
        {
            try
            {
                var coachService=ServiceContainer.Get<CoachService>();
                var fileService=ServiceContainer.Get<FileService>();
                
                var coach=new Coach
                {
                    Id=currentCoachId??0,
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

                if(currentCoachId.HasValue)coachService.Update(coach);
                else currentCoachId=coachService.Create(coach);

                var formService=ServiceContainer.Get<CoachFormService>();
                formService.Save(currentCoachId.Value,new CoachForm{
                    CoachLevel=int.TryParse(CoachLevelBox.Text,out var cl)?cl:null,
                    CoachBase=int.TryParse(CoachBaseBox.Text,out var cb)?cb:null,
                    PhysicalFileNumber=int.TryParse(PhysicalFileNumberBox.Text,out var pfn)?pfn:null,
                    CollaborationStatus=(CollaborationStatusCombo.SelectedItem as ComboBoxItem)?.Tag?.ToString()
                });

                foreach(var file in selectedFiles)
                {
                    var stored=fileService.SaveToDisk(file.Value);
                    fileService.RegisterDocument(currentCoachId.Value,file.Key,Path.GetFileName(file.Value),stored,new FileInfo(file.Value).Length);
                }
                foreach(var photo in selectedPhotos)
                {
                    var stored=fileService.SaveToDisk(photo);
                    fileService.RegisterDocument(currentCoachId.Value,DocumentCategories.CoachFormPhoto,Path.GetFileName(photo),stored,new FileInfo(photo).Length);
                }

                var courseService=ServiceContainer.Get<CoachingCourseService>();
                foreach(var item in pendingCourses)
                {
                    if(item.Course.Id==0)
                    {
                        item.Course.CoachId=currentCoachId.Value;
                        item.Course.Id=courseService.Create(item.Course);
                        if(!string.IsNullOrEmpty(item.FilePath))
                        {
                            var stored=fileService.SaveToDisk(item.FilePath);
                            courseService.AddFile(item.Course.Id,Path.GetFileName(item.FilePath),stored,new FileInfo(item.FilePath).Length);
                        }
                    }
                }
                
                foreach(var bank in pendingBanks)if(bank.Id==0){bank.CoachId=currentCoachId.Value;ServiceContainer.Get<BankAccountService>().Create(bank);}
                
                var chessService=ServiceContainer.Get<ChessTrainingService>();
                foreach(var item in pendingChess)
                {
                    if(item.Record.Id==0)
                    {
                        item.Record.CoachId=currentCoachId.Value;
                        item.Record.Id=chessService.Create(item.Record);
                        if(!string.IsNullOrEmpty(item.FilePath))
                        {
                            var stored=fileService.SaveToDisk(item.FilePath);
                            chessService.AddFile(item.Record.Id,Path.GetFileName(item.FilePath),stored,new FileInfo(item.FilePath).Length);
                        }
                    }
                }
                var payService=ServiceContainer.Get<PaymentService>();
                foreach(var item in pendingPayments)
                {
                    if(item.Payment.Id==0)
                    {
                        item.Payment.CoachId=currentCoachId.Value;
                        item.Payment.Id=payService.Create(item.Payment);
                        if(!string.IsNullOrEmpty(item.FilePath))
                        {
                            var stored=fileService.SaveToDisk(item.FilePath);
                            payService.AddFile(item.Payment.Id,Path.GetFileName(item.FilePath),stored,new FileInfo(item.FilePath).Length);
                        }
                    }
                }
                SaveDynamicFields();

                MessageBox.Show("اطلاعات با موفقیت ذخیره شد","موفق",MessageBoxButton.OK,MessageBoxImage.Information);
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
            DialogResult=false;
            Close();
        }
        private void ManageFilesButton_Click(object sender,RoutedEventArgs e)
        {
            if(currentCoachId.HasValue)
            {
                new DocumentManagerWindow(currentCoachId.Value){Owner=this}.ShowDialog();
            }
            else
            {
                MessageBox.Show("ابتدا اطلاعات مربی را ذخیره کنید","توجه",MessageBoxButton.OK,MessageBoxImage.Warning);
            }
        }

        private void LoadDynamicFields()
        {
            DynamicFieldsPanel.Children.Clear();
            if(!currentCoachId.HasValue)return;
            var fields=ServiceContainer.Get<CustomFieldService>().GetAll();
            var values=ServiceContainer.Get<CustomFieldService>().GetValuesByCoachId(currentCoachId.Value).ToDictionary(x=>x.FieldId);
            foreach(var field in fields)
            {
                var stack=new StackPanel{Margin=new Thickness(4),Width=240};
                stack.Children.Add(new TextBlock{Text=field.Title,FontSize=12,Foreground=new SolidColorBrush(Color.FromRgb(44,62,80))});
                if(field.FieldType==CustomFieldTypes.Text||field.FieldType==CustomFieldTypes.Number)
                {
                    var tb=new TextBox{Height=30,Margin=new Thickness(0,4,0,0),Tag=field};
                    if(values.ContainsKey(field.Id))tb.Text=values[field.Id].Value;
                    stack.Children.Add(tb);
                }
                else if(field.FieldType==CustomFieldTypes.Date)
                {
                    var dp=new DatePicker{Height=30,Margin=new Thickness(0,4,0,0),Tag=field};
                    if(values.ContainsKey(field.Id)&&!string.IsNullOrEmpty(values[field.Id].Value))dp.SelectedDate=DateTime.Parse(values[field.Id].Value);
                    stack.Children.Add(dp);
                }
                else if(field.FieldType==CustomFieldTypes.Select||field.FieldType==CustomFieldTypes.Radio)
                {
                    var cb=new ComboBox{Height=30,Margin=new Thickness(0,4,0,0),Tag=field};
                    if(!string.IsNullOrEmpty(field.Options))foreach(var opt in field.Options.Split(','))cb.Items.Add(opt.Trim());
                    if(values.ContainsKey(field.Id))cb.SelectedItem=values[field.Id].Value;
                    stack.Children.Add(cb);
                }
                else if(field.FieldType==CustomFieldTypes.Checkbox)
                {
                    var cb=new CheckBox{Height=30,Margin=new Thickness(0,4,0,0),Tag=field,Content="بله"};
                    if(values.ContainsKey(field.Id))cb.IsChecked=values[field.Id].Value=="True";
                    stack.Children.Add(cb);
                }
                else if(field.FieldType==CustomFieldTypes.MultiSelect)
                {
                    var lb=new ListBox{Height=80,Margin=new Thickness(0,4,0,0),Tag=field};
                    lb.SelectionMode=SelectionMode.Multiple;
                    if(!string.IsNullOrEmpty(field.Options))foreach(var opt in field.Options.Split(','))lb.Items.Add(opt.Trim());
                    stack.Children.Add(lb);
                }
                else if(field.FieldType==CustomFieldTypes.File||field.FieldType==CustomFieldTypes.TemplateRoundtrip)
                {
                    var sp=new StackPanel{Orientation=Orientation.Horizontal,Margin=new Thickness(0,4,0,0)};
                    var btn=new Button{Content="انتخاب فایل",Height=30,Width=100,Tag=field};
                    btn.Click+=DynamicFileUpload_Click;
                    var lbl=new TextBlock{VerticalAlignment=VerticalAlignment.Center,Margin=new Thickness(10,0,0,0),Text="بدون فایل"};
                    if(values.ContainsKey(field.Id)&&!string.IsNullOrEmpty(values[field.Id].Value))lbl.Text=values[field.Id].Value;
                    sp.Children.Add(btn);
                    sp.Children.Add(lbl);
                    stack.Children.Add(sp);
                    if(field.FieldType==CustomFieldTypes.TemplateRoundtrip&&!string.IsNullOrEmpty(field.TemplateStoredFileName))
                    {
                        var dlBtn=new Button{Content="دانلود قالب",Height=24,Margin=new Thickness(0,4,0,0),Tag=field.TemplateStoredFileName,Background=new SolidColorBrush(Color.FromRgb(31,111,235)),Foreground=Brushes.White,BorderThickness=new Thickness(0)};
                        dlBtn.Click+=DownloadTemplate_Click;
                        stack.Children.Add(dlBtn);
                    }
                }
                DynamicFieldsPanel.Children.Add(stack);
            }
        }

        private void DynamicFileUpload_Click(object sender,RoutedEventArgs e)
        {
            var btn=sender as Button;
            var field=btn.Tag as CustomField;
            var dialog=new OpenFileDialog{Filter="All Files|*.*"};
            if(dialog.ShowDialog()==true)
            {
                dynamicFilePaths[field.Id]=dialog.FileName;
                var sp=btn.Parent as StackPanel;
                var lbl=sp.Children[1] as TextBlock;
                lbl.Text=Path.GetFileName(dialog.FileName);
            }
        }

        private void DownloadTemplate_Click(object sender,RoutedEventArgs e)
        {
            var btn=sender as Button;
            var storedName=btn.Tag.ToString();
            try
            {
                ServiceContainer.Get<FileService>().OpenDocument(storedName);
            }
            catch(Exception)
            {
                MessageBox.Show("خطا در باز کردن فایل قالب","خطا",MessageBoxButton.OK,MessageBoxImage.Error);
            }
        }

        private void SaveDynamicFields()
        {
            var service=ServiceContainer.Get<CustomFieldService>();
            foreach(var child in DynamicFieldsPanel.Children)
            {
                var stack=child as StackPanel;
                if(stack==null)continue;
                foreach(var element in stack.Children)
                {
                    if(element is TextBox tb)
                    {
                        var field=(CustomField)tb.Tag;
                        service.SaveValue(currentCoachId.Value,field.Id,tb.Text.Trim());
                    }
                    else if(element is DatePicker dp)
                    {
                        var field=(CustomField)dp.Tag;
                        service.SaveValue(currentCoachId.Value,field.Id,dp.SelectedDate?.ToString("yyyy-MM-dd"));
                    }
                    else if(element is ComboBox cb)
                    {
                        var field=(CustomField)cb.Tag;
                        service.SaveValue(currentCoachId.Value,field.Id,cb.SelectedItem?.ToString());
                    }
                    else if(element is CheckBox chk)
                    {
                        var field=(CustomField)chk.Tag;
                        service.SaveValue(currentCoachId.Value,field.Id,chk.IsChecked==true?"True":"False");
                    }
                    else if(element is ListBox lb)
                    {
                        var field=(CustomField)lb.Tag;
                        var selected=new List<string>();
                        foreach(var item in lb.SelectedItems)selected.Add(item.ToString());
                        service.SaveValue(currentCoachId.Value,field.Id,string.Join(",",selected));
                    }
                    else if(element is StackPanel sp && sp.Children.Count==2 && sp.Children[0] is Button btn && btn.Tag is CustomField f && (f.FieldType==CustomFieldTypes.File||f.FieldType==CustomFieldTypes.TemplateRoundtrip))
                    {
                        var lbl=sp.Children[1] as TextBlock;
                        service.SaveValue(currentCoachId.Value,f.Id,lbl.Text);
                    }
                }
            }
            foreach(var kvp in dynamicFilePaths)
            {
                var stored=ServiceContainer.Get<FileService>().SaveToDisk(kvp.Value);
                service.SaveValue(currentCoachId.Value,kvp.Key,Path.GetFileName(kvp.Value));
                var valId=service.GetValueId(currentCoachId.Value,kvp.Key);
                service.SaveFile(valId,Path.GetFileName(kvp.Value),stored,new FileInfo(kvp.Value).Length);
            }
            dynamicFilePaths.Clear();
        }
        private void PickPaymentFileBtn_Click(object sender,RoutedEventArgs e)
        {
            var dialog=new OpenFileDialog{Filter="Image Files|*.jpg;*.jpeg;*.png;*.bmp"};
            if(dialog.ShowDialog()==true)
            {
                currentPaymentFilePath=dialog.FileName;
                PaymentFileText.Text=Path.GetFileName(dialog.FileName);
            }
        }
        private void ViewPaymentFileBtn_Click(object sender,RoutedEventArgs e)
        {
            var item=(PaymentViewItem)((FrameworkElement)sender).DataContext;
            if(item.HasFile)ServiceContainer.Get<FileService>().OpenDocument(item.FilePath);
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
    public class PaymentViewItem
    {
        public Payment Payment{get;set;}
        public string FilePath{get;set;}
        public bool HasFile=>!string.IsNullOrEmpty(FilePath);
    }
}