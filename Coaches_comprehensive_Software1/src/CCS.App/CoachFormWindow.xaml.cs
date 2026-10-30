using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using CCS.Core;
using CCS.Services;
namespace CCS.App
{
    public partial class CoachFormWindow:Window
    {
        private readonly long currentCoachId;
        private readonly Dictionary<string,string> selectedFiles=new Dictionary<string,string>();
        private readonly List<string> selectedPhotos=new List<string>();
        public CoachFormWindow(long coachId)
        {
            InitializeComponent();
            currentCoachId=coachId;
            foreach(var item in CollaborationStatus.PersianNames)
            {
                CollaborationStatusCombo.Items.Add(new ComboBoxItem{Content=item.Value,Tag=item.Key});
            }
            LoadForm();
        }
        private void LoadForm()
        {
            var form=ServiceContainer.Get<CoachFormService>().GetByCoachId(currentCoachId);
            if(form==null)return;
            CoachLevelBox.Text=form.CoachLevel?.ToString();
            CoachBaseBox.Text=form.CoachBase?.ToString();
            PhysicalFileNumberBox.Text=form.PhysicalFileNumber?.ToString();
            foreach(ComboBoxItem item in CollaborationStatusCombo.Items)
            {
                if(item.Tag?.ToString()==form.CollaborationStatus)
                {
                    CollaborationStatusCombo.SelectedItem=item;
                    break;
                }
            }
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
        private void SaveButton_Click(object sender,RoutedEventArgs e)
        {
            try
            {
                var formService=ServiceContainer.Get<CoachFormService>();
                var fileService=ServiceContainer.Get<FileService>();
                var form=new CoachForm
                {
                    CoachLevel=int.TryParse(CoachLevelBox.Text,out var cl)?cl:null,
                    CoachBase=int.TryParse(CoachBaseBox.Text,out var cb)?cb:null,
                    PhysicalFileNumber=int.TryParse(PhysicalFileNumberBox.Text,out var pfn)?pfn:null,
                    CollaborationStatus=(CollaborationStatusCombo.SelectedItem as ComboBoxItem)?.Tag?.ToString()
                };
                formService.Save(currentCoachId,form);
                foreach(var file in selectedFiles)
                {
                    var stored=fileService.SaveToDisk(file.Value);
                    fileService.RegisterDocument(currentCoachId,file.Key,Path.GetFileName(file.Value),stored,new FileInfo(file.Value).Length);
                }
                foreach(var photo in selectedPhotos)
                {
                    var stored=fileService.SaveToDisk(photo);
                    fileService.RegisterDocument(currentCoachId,DocumentCategories.CoachFormPhoto,Path.GetFileName(photo),stored,new FileInfo(photo).Length);
                }
                MessageBox.Show("اطلاعات فرم سازمانی ذخیره شد","موفق",MessageBoxButton.OK,MessageBoxImage.Information);
                DialogResult=true;
                Close();
            }
            catch(Exception)
            {
                ErrorText.Text="خطا در ذخیره اطلاعات";
            }
        }
        private void CancelButton_Click(object sender,RoutedEventArgs e)
        {
            DialogResult=false;
            Close();
        }
    }
}