using System.Data;
using Dapper;
namespace CCS.Data
{
    public static class DatabaseInitializer
    {
        private static readonly object _lock=new object();

        public static void Initialize()
        {
            lock(_lock)
            {
                using(var connection=Database.Open())
                {
                    connection.Execute("CREATE TABLE IF NOT EXISTS SchemaVersion(Version INTEGER NOT NULL)");
                    long version=connection.ExecuteScalar<long>("SELECT COALESCE(MAX(Version),0) FROM SchemaVersion");
                    if(version<1)
                    {
                        ApplyV1(connection);
                        connection.Execute("INSERT INTO SchemaVersion(Version) VALUES(1)");
                        version=1;
                    }
                    if(version<2)
                    {
                        ApplyV2(connection);
                        connection.Execute("INSERT INTO SchemaVersion(Version) VALUES(2)");
                        version=2;
                    }
                    if(version<3)
                    {
                        ApplyV3(connection);
                        connection.Execute("INSERT INTO SchemaVersion(Version) VALUES(3)");
                        version=3;
                    }
                    if(version<4)
                    {
                        ApplyV4(connection);
                        connection.Execute("INSERT INTO SchemaVersion(Version) VALUES(4)");
                    }
                }
            }
        }
        static void ApplyV1(IDbConnection connection)
        {
            connection.Execute(
"CREATE TABLE IF NOT EXISTS Users(Id INTEGER PRIMARY KEY AUTOINCREMENT,Username TEXT NOT NULL UNIQUE,PasswordHash TEXT NOT NULL,PasswordSalt TEXT NOT NULL,CreatedAt TEXT NOT NULL);"+
"CREATE TABLE IF NOT EXISTS Coaches(Id INTEGER PRIMARY KEY AUTOINCREMENT,FirstName TEXT,LastName TEXT,FatherName TEXT,NationalCode TEXT,BirthCertificateNumber TEXT,PersonnelCode TEXT,BirthDate TEXT,Gender TEXT,MaritalStatus TEXT,ChildrenCount INTEGER,FieldOfStudy TEXT,Education TEXT,MembershipType TEXT,EntryDate TEXT,LastPosition TEXT,FileType TEXT,DistrictName TEXT,MilitaryStatus TEXT,PhysicalStatus TEXT,Weight REAL,Height REAL,BloodType TEXT,SacrificeStatus TEXT,Job TEXT,WorkAddress TEXT,HomeAddress TEXT,Phone1 TEXT,Phone2 TEXT,CreatedAt TEXT NOT NULL,UpdatedAt TEXT);"+
"CREATE INDEX IF NOT EXISTS IX_Coaches_FirstName ON Coaches(FirstName);"+
"CREATE INDEX IF NOT EXISTS IX_Coaches_LastName ON Coaches(LastName);"+
"CREATE INDEX IF NOT EXISTS IX_Coaches_FatherName ON Coaches(FatherName);"+
"CREATE INDEX IF NOT EXISTS IX_Coaches_NationalCode ON Coaches(NationalCode);"+
"CREATE INDEX IF NOT EXISTS IX_Coaches_FileType ON Coaches(FileType);"+
"CREATE TABLE IF NOT EXISTS CoachDocuments(Id INTEGER PRIMARY KEY AUTOINCREMENT,CoachId INTEGER NOT NULL,Category TEXT NOT NULL,OriginalFileName TEXT NOT NULL,StoredFileName TEXT NOT NULL,FileSize INTEGER NOT NULL,UploadedAt TEXT NOT NULL);"+
"CREATE INDEX IF NOT EXISTS IX_CoachDocuments_CoachId ON CoachDocuments(CoachId);"+
"CREATE INDEX IF NOT EXISTS IX_CoachDocuments_Category ON CoachDocuments(Category);"+
"CREATE TABLE IF NOT EXISTS CoachForms(Id INTEGER PRIMARY KEY AUTOINCREMENT,CoachId INTEGER NOT NULL UNIQUE,CoachLevel INTEGER,CoachBase INTEGER,PhysicalFileNumber INTEGER,CollaborationStatus TEXT);"+
"CREATE TABLE IF NOT EXISTS CoachingCourses(Id INTEGER PRIMARY KEY AUTOINCREMENT,CoachId INTEGER NOT NULL,CourseType TEXT,CourseName TEXT,StartDate TEXT,EndDate TEXT,DaysCount INTEGER,Description TEXT,CreatedAt TEXT NOT NULL);"+
"CREATE INDEX IF NOT EXISTS IX_CoachingCourses_CoachId ON CoachingCourses(CoachId);"+
"CREATE INDEX IF NOT EXISTS IX_CoachingCourses_CourseName ON CoachingCourses(CourseName);"+
"CREATE TABLE IF NOT EXISTS CourseFiles(Id INTEGER PRIMARY KEY AUTOINCREMENT,CourseId INTEGER NOT NULL,OriginalFileName TEXT NOT NULL,StoredFileName TEXT NOT NULL,FileSize INTEGER NOT NULL,UploadedAt TEXT NOT NULL);"+
"CREATE INDEX IF NOT EXISTS IX_CourseFiles_CourseId ON CourseFiles(CourseId);"+
"CREATE TABLE IF NOT EXISTS BankAccounts(Id INTEGER PRIMARY KEY AUTOINCREMENT,CoachId INTEGER NOT NULL,BankName TEXT,AccountNumber TEXT,IBAN TEXT,CardNumber TEXT,CreatedAt TEXT NOT NULL);"+
"CREATE INDEX IF NOT EXISTS IX_BankAccounts_CoachId ON BankAccounts(CoachId);"+
"CREATE TABLE IF NOT EXISTS ChessTrainingRecords(Id INTEGER PRIMARY KEY AUTOINCREMENT,CoachId INTEGER NOT NULL,Year INTEGER,ChessType TEXT,PlayDate TEXT,Score REAL,Description TEXT,CreatedAt TEXT NOT NULL);"+
"CREATE INDEX IF NOT EXISTS IX_ChessTrainingRecords_CoachId ON ChessTrainingRecords(CoachId);"+
"CREATE TABLE IF NOT EXISTS ChessTrainingFiles(Id INTEGER PRIMARY KEY AUTOINCREMENT,RecordId INTEGER NOT NULL,OriginalFileName TEXT NOT NULL,StoredFileName TEXT NOT NULL,FileSize INTEGER NOT NULL,UploadedAt TEXT NOT NULL);"+
"CREATE INDEX IF NOT EXISTS IX_ChessTrainingFiles_RecordId ON ChessTrainingFiles(RecordId);"+
"CREATE TABLE IF NOT EXISTS Payments(Id INTEGER PRIMARY KEY AUTOINCREMENT,CoachId INTEGER NOT NULL,Subject TEXT,PaymentDate TEXT,StartTime TEXT,EndTime TEXT,Location TEXT,DaysCount INTEGER,ClassesCount INTEGER,PaidAmount INTEGER,ReturnedAmount INTEGER,PaidDate TEXT,CreatedAt TEXT NOT NULL);"+
"CREATE INDEX IF NOT EXISTS IX_Payments_CoachId ON Payments(CoachId);"+
"CREATE TABLE IF NOT EXISTS PaymentFiles(Id INTEGER PRIMARY KEY AUTOINCREMENT,PaymentId INTEGER NOT NULL,OriginalFileName TEXT NOT NULL,StoredFileName TEXT NOT NULL,FileSize INTEGER NOT NULL,UploadedAt TEXT NOT NULL);"+
"CREATE INDEX IF NOT EXISTS IX_PaymentFiles_PaymentId ON PaymentFiles(PaymentId);"+
"CREATE TABLE IF NOT EXISTS CustomFields(Id INTEGER PRIMARY KEY AUTOINCREMENT,Title TEXT NOT NULL,FieldType TEXT NOT NULL,MinValue TEXT,MaxValue TEXT,Options TEXT,DisplayOrder INTEGER NOT NULL,CreatedAt TEXT NOT NULL);"+
"CREATE TABLE IF NOT EXISTS CustomFieldValues(Id INTEGER PRIMARY KEY AUTOINCREMENT,FieldId INTEGER NOT NULL,CoachId INTEGER NOT NULL,Value TEXT,UNIQUE(FieldId,CoachId));"+
"CREATE INDEX IF NOT EXISTS IX_CustomFieldValues_CoachId ON CustomFieldValues(CoachId)");
        }
        static void ApplyV2(IDbConnection connection)
        {
            var cols=connection.Query<string>("SELECT name FROM pragma_table_info('CustomFields')");
            if(!new System.Collections.Generic.List<string>(cols).Contains("TemplateOriginalFileName"))
            {
                connection.Execute("ALTER TABLE CustomFields ADD COLUMN TemplateOriginalFileName TEXT");
                connection.Execute("ALTER TABLE CustomFields ADD COLUMN TemplateStoredFileName TEXT");
                connection.Execute("ALTER TABLE CustomFields ADD COLUMN TemplateFileSize INTEGER");
            }
            connection.Execute("CREATE TABLE IF NOT EXISTS CustomFieldFiles(Id INTEGER PRIMARY KEY AUTOINCREMENT,ValueId INTEGER NOT NULL,OriginalFileName TEXT NOT NULL,StoredFileName TEXT NOT NULL,FileSize INTEGER NOT NULL,UploadedAt TEXT NOT NULL)");
            connection.Execute("CREATE INDEX IF NOT EXISTS IX_CustomFieldFiles_ValueId ON CustomFieldFiles(ValueId)");
        }
        static void ApplyV3(IDbConnection connection)
        {
            connection.Execute("CREATE TABLE IF NOT EXISTS AppSettings(Id INTEGER PRIMARY KEY AUTOINCREMENT,Key TEXT NOT NULL UNIQUE,Value TEXT)");
        }
        static void ApplyV4(IDbConnection connection)
        {
            var cols=connection.Query<string>("SELECT name FROM pragma_table_info('Users')");
            if(!new System.Collections.Generic.List<string>(cols).Contains("Role"))
            {
                connection.Execute("ALTER TABLE Users ADD COLUMN Role TEXT DEFAULT 'Manager'");
            }
        }
    }
}