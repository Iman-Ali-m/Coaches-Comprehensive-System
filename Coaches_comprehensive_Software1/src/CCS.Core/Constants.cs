using System;
using System.Collections.Generic;
namespace CCS.Core
{
    public static class FileTypes
    {
        public const string Dependent="Dependent";
        public const string Contractual="Contractual";
        public const string OfficialDependent="OfficialDependent";
        public const string Official="Official";
        public static readonly Dictionary<string,string> PersianNames=new Dictionary<string,string>
        {
            {Dependent,"مربی وابسته"},
            {Contractual,"مربی قراردادی"},
            {OfficialDependent,"مربی رسمی وابسته"},
            {Official,"مربی رسمی"}
        };
    }
    public static class DocumentCategories
    {
        public const string NationalCardFrontBack="NationalCardFrontBack";
        public const string BirthCertificateAllPages="BirthCertificateAllPages";
        public const string PersonalPhoto="PersonalPhoto";
        public const string MilitaryServiceCard="MilitaryServiceCard";
        public const string LastDegreeCertificate="LastDegreeCertificate";
        public const string PublicOfficial="PublicOfficial";
        public const string MembershipConversion="MembershipConversion";
        public const string PreliminaryRank="PreliminaryRank";
        public const string AdvancedRank="AdvancedRank";
        public const string LastPositionOrder="LastPositionOrder";
        public const string LastRankDelegation="LastRankDelegation";
        public const string LastRankCategoryDelegation="LastRankCategoryDelegation";
        public const string MedicalExemptionDocuments="MedicalExemptionDocuments";
        public const string Cartex="Cartex";
        public const string CoachFormPhoto="CoachFormPhoto";
        public static readonly Dictionary<string,string> PersianNames=new Dictionary<string,string>
        {
            {NationalCardFrontBack,"کارت ملی پشت و رو"},
            {BirthCertificateAllPages,"تمام صفحات شناسنامه"},
            {PersonalPhoto,"عکس شخصی"},
            {MilitaryServiceCard,"کارت پایان خدمت"},
            {LastDegreeCertificate,"آخرین مدرک تحصیلی"},
            {PublicOfficial,"عمومی رسمی"},
            {MembershipConversion,"تبدیل عضویت"},
            {PreliminaryRank,"مقدماتی رسته‌ای"},
            {AdvancedRank,"عالی رسته‌ای"},
            {LastPositionOrder,"آخرین حکم مسئولیت"},
            {LastRankDelegation,"آخرین ابلاغ درجه"},
            {LastRankCategoryDelegation,"آخرین ابلاغ رسته"},
            {MedicalExemptionDocuments,"اسناد پزشکی معافیت"},
            {Cartex,"کارتکس"},
            {CoachFormPhoto,"عکس های فرم"}
        };
    }
    public static class CollaborationStatus
    {
        public const string Inactive="Inactive";
        public const string Active="Active";
        public const string Transferred="Transferred";
        public static readonly Dictionary<string,string> PersianNames=new Dictionary<string,string>
        {
            {Inactive,"راکد"},
            {Active,"جاری"},
            {Transferred,"انتقال"}
        };
    }
    public static class CourseTypes
    {
        public const string Coaching="Coaching";
        public const string OrganizationalCross="OrganizationalCross";
        public const string NonOrganizational="NonOrganizational";
        public static readonly Dictionary<string,string> PersianNames=new Dictionary<string,string>
        {
            {Coaching,"مربیگری"},
            {OrganizationalCross,"عرضی سازمانی"},
            {NonOrganizational,"غیر سازمانی"}
        };
    }
    public static class CustomFieldTypes
    {
        public const string Text="Text";
        public const string Number="Number";
        public const string Date="Date";
        public const string File="File";
        public const string Checkbox="Checkbox";
        public const string Radio="Radio";
        public const string Select="Select";
        public const string MultiSelect="MultiSelect";
        public const string TemplateRoundtrip="TemplateRoundtrip";
        public static readonly Dictionary<string,string> PersianNames=new Dictionary<string,string>
        {
            {Text,"ورودی تشریحی"},
            {Number,"ورودی عددی"},
            {Date,"ورودی تاریخ"},
            {File,"ورودی فایل"},
            {Checkbox,"چک باکس"},
            {Radio,"دکمه رادیویی"},
            {Select,"منو کشویی (تک انتخابی)"},
            {MultiSelect,"لیست کشویی (چند انتخابی)"},
            {TemplateRoundtrip,"دریافت و بازگرداندن فایل"}
        };
    }
    public static class Clock
    {
        public static string Now()=>DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
    }
}