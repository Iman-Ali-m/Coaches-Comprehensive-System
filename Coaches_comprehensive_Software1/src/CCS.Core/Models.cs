namespace CCS.Core
{
    public class User
    {
        public long Id{get;set;}
        public string Username{get;set;}
        public string PasswordHash{get;set;}
        public string PasswordSalt{get;set;}
        public string Role{get;set;}
        public string CreatedAt{get;set;}
    }
    public class Coach
    {
        public long Id{get;set;}
        public string FirstName{get;set;}
        public string LastName{get;set;}
        public string FatherName{get;set;}
        public string NationalCode{get;set;}
        public string BirthCertificateNumber{get;set;}
        public string PersonnelCode{get;set;}
        public string BirthDate{get;set;}
        public string Gender{get;set;}
        public string MaritalStatus{get;set;}
        public int? ChildrenCount{get;set;}
        public string FieldOfStudy{get;set;}
        public string Education{get;set;}
        public string MembershipType{get;set;}
        public string EntryDate{get;set;}
        public string LastPosition{get;set;}
        public string FileType{get;set;}
        public string DistrictName{get;set;}
        public string MilitaryStatus{get;set;}
        public string PhysicalStatus{get;set;}
        public double? Weight{get;set;}
        public double? Height{get;set;}
        public string BloodType{get;set;}
        public string SacrificeStatus{get;set;}
        public string Job{get;set;}
        public string WorkAddress{get;set;}
        public string HomeAddress{get;set;}
        public string Phone1{get;set;}
        public string Phone2{get;set;}
        public string CreatedAt{get;set;}
        public string UpdatedAt{get;set;}
    }
    public class CoachDocument
    {
        public long Id{get;set;}
        public long CoachId{get;set;}
        public string Category{get;set;}
        public string OriginalFileName{get;set;}
        public string StoredFileName{get;set;}
        public long FileSize{get;set;}
        public string UploadedAt{get;set;}
    }
    public class CoachForm
    {
        public long Id{get;set;}
        public long CoachId{get;set;}
        public int? CoachLevel{get;set;}
        public int? CoachBase{get;set;}
        public int? PhysicalFileNumber{get;set;}
        public string CollaborationStatus{get;set;}
    }
    public class CoachingCourse
    {
        public long Id{get;set;}
        public long CoachId{get;set;}
        public string CourseType{get;set;}
        public string CourseName{get;set;}
        public string StartDate{get;set;}
        public string EndDate{get;set;}
        public int? DaysCount{get;set;}
        public string Description{get;set;}
        public string CreatedAt{get;set;}
    }
    public class CourseFile
    {
        public long Id{get;set;}
        public long CourseId{get;set;}
        public string OriginalFileName{get;set;}
        public string StoredFileName{get;set;}
        public long FileSize{get;set;}
        public string UploadedAt{get;set;}
    }
    public class BankAccount
    {
        public long Id{get;set;}
        public long CoachId{get;set;}
        public string BankName{get;set;}
        public string AccountNumber{get;set;}
        public string IBAN{get;set;}
        public string CardNumber{get;set;}
        public string CreatedAt{get;set;}
    }
    public class ChessTrainingRecord
    {
        public long Id{get;set;}
        public long CoachId{get;set;}
        public int? Year{get;set;}
        public string ChessType{get;set;}
        public string PlayDate{get;set;}
        public double? Score{get;set;}
        public string Description{get;set;}
        public string CreatedAt{get;set;}
    }
    public class ChessTrainingFile
    {
        public long Id{get;set;}
        public long RecordId{get;set;}
        public string OriginalFileName{get;set;}
        public string StoredFileName{get;set;}
        public long FileSize{get;set;}
        public string UploadedAt{get;set;}
    }
    public class Payment
    {
        public long Id{get;set;}
        public long CoachId{get;set;}
        public string Subject{get;set;}
        public string PaymentDate{get;set;}
        public string StartTime{get;set;}
        public string EndTime{get;set;}
        public string Location{get;set;}
        public int? DaysCount{get;set;}
        public int? ClassesCount{get;set;}
        public long? PaidAmount{get;set;}
        public long? ReturnedAmount{get;set;}
        public string PaidDate{get;set;}
        public string CreatedAt{get;set;}
    }
    public class PaymentFile
    {
        public long Id{get;set;}
        public long PaymentId{get;set;}
        public string OriginalFileName{get;set;}
        public string StoredFileName{get;set;}
        public long FileSize{get;set;}
        public string UploadedAt{get;set;}
    }
    public class CustomField
    {
        public long Id{get;set;}
        public string Title{get;set;}
        public string FieldType{get;set;}
        public string MinValue{get;set;}
        public string MaxValue{get;set;}
        public string Options{get;set;}
        public int DisplayOrder{get;set;}
        public string TemplateOriginalFileName{get;set;}
        public string TemplateStoredFileName{get;set;}
        public long? TemplateFileSize{get;set;}
        public string CreatedAt{get;set;}
    }

    public class CustomFieldFile
    {
        public long Id{get;set;}
        public long ValueId{get;set;}
        public string OriginalFileName{get;set;}
        public string StoredFileName{get;set;}
        public long FileSize{get;set;}
        public string UploadedAt{get;set;}
    }

    public class CustomFieldValue
    {
        public long Id{get;set;}
        public long FieldId{get;set;}
        public long CoachId{get;set;}
        public string Value{get;set;}
    }
    public class AppSetting
    {
        public long Id{get;set;}
        public string Key{get;set;}
        public string Value{get;set;}
    }
}