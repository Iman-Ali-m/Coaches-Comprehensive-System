using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Dapper;
using ClosedXML.Excel;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Kernel.Font;
using iText.IO.Font;
using CCS.Core;
using CCS.Data;
namespace CCS.Services
{
    public class ResumeExportService
    {
        public void ExportExcel(long coachId,string destinationFolder)
        {
            using(var connection=Database.Open())
            {
                var coach=connection.QueryFirstOrDefault<Coach>("SELECT * FROM Coaches WHERE Id=@coachId",new{coachId});
                if(coach==null)return;
                using(var workbook=new XLWorkbook())
                {
                    var worksheet=workbook.Worksheets.Add("رزومه");
                    worksheet.RightToLeft=true;
                    var row=1;
                    void AddRow(string label,string value)
                    {
                        worksheet.Cell(row,1).Value=label;
                        worksheet.Cell(row,1).Style.Font.Bold=true;
                        worksheet.Cell(row,2).Value=value??"";
                        row++;
                    }
                    AddRow("نام",coach.FirstName);
                    AddRow("نام خانوادگی",coach.LastName);
                    AddRow("نام پدر",coach.FatherName);
                    AddRow("شماره ملی",coach.NationalCode);
                    AddRow("شماره شناسنامه",coach.BirthCertificateNumber);
                    AddRow("کد پرسنلی",coach.PersonnelCode);
                    AddRow("تاریخ تولد",coach.BirthDate);
                    AddRow("جنسیت",coach.Gender);
                    AddRow("وضعیت تاهل",coach.MaritalStatus);
                    AddRow("تعداد فرزندان",coach.ChildrenCount?.ToString());
                    AddRow("رشته تحصیلی",coach.FieldOfStudy);
                    AddRow("تحصیلات",coach.Education);
                    AddRow("نوع عضویت",coach.MembershipType);
                    AddRow("تاریخ ورود",coach.EntryDate);
                    AddRow("آخرین مسئولیت",coach.LastPosition);
                    AddRow("نوع پرونده",coach.FileType);
                    AddRow("نام ناحیه",coach.DistrictName);
                    AddRow("وضعیت سربازی",coach.MilitaryStatus);
                    AddRow("وضعیت جسمانی",coach.PhysicalStatus);
                    AddRow("وزن",coach.Weight?.ToString());
                    AddRow("قد",coach.Height?.ToString());
                    AddRow("گروه خونی",coach.BloodType);
                    AddRow("وضعیت ایثارگری",coach.SacrificeStatus);
                    AddRow("شغل",coach.Job);
                    AddRow("آدرس محل کار",coach.WorkAddress);
                    AddRow("آدرس منزل",coach.HomeAddress);
                    AddRow("شماره تماس ۱",coach.Phone1);
                    AddRow("شماره تماس ۲",coach.Phone2);
                    
                    var form=connection.QueryFirstOrDefault<CoachForm>("SELECT * FROM CoachForms WHERE CoachId=@coachId",new{coachId});
                    if(form!=null)
                    {
                        row++;
                        worksheet.Cell(row,1).Value="فرم سازمانی";
                        worksheet.Cell(row,1).Style.Font.Bold=true;
                        row++;
                        AddRow("سطح مربی",form.CoachLevel?.ToString());
                        AddRow("پایه مربی",form.CoachBase?.ToString());
                        AddRow("شماره پرونده فیزیکی",form.PhysicalFileNumber?.ToString());
                        AddRow("وضعیت همکاری",form.CollaborationStatus);
                    }

                    var courses=connection.Query<CoachingCourse>("SELECT * FROM CoachingCourses WHERE CoachId=@coachId",new{coachId}).ToList();
                    if(courses.Count>0)
                    {
                        row++;
                        worksheet.Cell(row,1).Value="دوره‌های مربیگری";
                        worksheet.Cell(row,1).Style.Font.Bold=true;
                        row++;
                        worksheet.Cell(row,1).Value="نوع";
                        worksheet.Cell(row,2).Value="نام دوره";
                        worksheet.Cell(row,3).Value="تاریخ شروع";
                        worksheet.Cell(row,4).Value="تاریخ پایان";
                        worksheet.Cell(row,5).Value="تعداد روز";
                        worksheet.Cell(row,6).Value="توضیحات";
                        worksheet.Row(row).Style.Font.Bold=true;
                        row++;
                        foreach(var c in courses)
                        {
                            worksheet.Cell(row,1).Value=c.CourseType;
                            worksheet.Cell(row,2).Value=c.CourseName;
                            worksheet.Cell(row,3).Value=c.StartDate;
                            worksheet.Cell(row,4).Value=c.EndDate;
                            worksheet.Cell(row,5).Value=c.DaysCount;
                            worksheet.Cell(row,6).Value=c.Description;
                            row++;
                        }
                    }

                    var banks=connection.Query<BankAccount>("SELECT * FROM BankAccounts WHERE CoachId=@coachId",new{coachId}).ToList();
                    if(banks.Count>0)
                    {
                        row++;
                        worksheet.Cell(row,1).Value="اطلاعات بانکی";
                        worksheet.Cell(row,1).Style.Font.Bold=true;
                        row++;
                        worksheet.Cell(row,1).Value="نام بانک";
                        worksheet.Cell(row,2).Value="شماره حساب";
                        worksheet.Cell(row,3).Value="شماره شبا";
                        worksheet.Cell(row,4).Value="شماره کارت";
                        worksheet.Row(row).Style.Font.Bold=true;
                        row++;
                        foreach(var b in banks)
                        {
                            worksheet.Cell(row,1).Value=b.BankName;
                            worksheet.Cell(row,2).Value=b.AccountNumber;
                            worksheet.Cell(row,3).Value=b.IBAN;
                            worksheet.Cell(row,4).Value=b.CardNumber;
                            row++;
                        }
                    }

                    var chess=connection.Query<ChessTrainingRecord>("SELECT * FROM ChessTrainingRecords WHERE CoachId=@coachId",new{coachId}).ToList();
                    if(chess.Count>0)
                    {
                        row++;
                        worksheet.Cell(row,1).Value="تداوم آموزش شطرنج";
                        worksheet.Cell(row,1).Style.Font.Bold=true;
                        row++;
                        worksheet.Cell(row,1).Value="سال";
                        worksheet.Cell(row,2).Value="نوع شطرنج";
                        worksheet.Cell(row,3).Value="تاریخ بازی";
                        worksheet.Cell(row,4).Value="نمره";
                        worksheet.Cell(row,5).Value="توضیحات";
                        worksheet.Row(row).Style.Font.Bold=true;
                        row++;
                        foreach(var ch in chess)
                        {
                            worksheet.Cell(row,1).Value=ch.Year;
                            worksheet.Cell(row,2).Value=ch.ChessType;
                            worksheet.Cell(row,3).Value=ch.PlayDate;
                            worksheet.Cell(row,4).Value=ch.Score;
                            worksheet.Cell(row,5).Value=ch.Description;
                            row++;
                        }
                    }

                    var customValues=connection.Query<dynamic>("SELECT cf.Title,cfv.Value FROM CustomFieldValues cfv INNER JOIN CustomFields cf ON cf.Id=cfv.FieldId WHERE cfv.CoachId=@coachId",new{coachId}).ToList();
                    if(customValues.Count>0)
                    {
                        row++;
                        worksheet.Cell(row,1).Value="ویژگی‌های پویا";
                        worksheet.Cell(row,1).Style.Font.Bold=true;
                        row++;
                        worksheet.Cell(row,1).Value="عنوان";
                        worksheet.Cell(row,2).Value="مقدار";
                        worksheet.Row(row).Style.Font.Bold=true;
                        row++;
                        foreach(var cv in customValues)
                        {
                            worksheet.Cell(row,1).Value=(string)cv.Title;
                            worksheet.Cell(row,2).Value=(string)cv.Value;
                            row++;
                        }
                    }
                    
                    worksheet.Columns().AdjustToContents();
                    workbook.SaveAs(Path.Combine(destinationFolder,$"{coach.NationalCode}.xlsx"));
                }
            }
        }

        public void ExportPdf(long coachId,string destinationFolder)
        {
            using(var connection=Database.Open())
            {
                var coach=connection.QueryFirstOrDefault<Coach>("SELECT * FROM Coaches WHERE Id=@coachId",new{coachId});
                if(coach==null)return;
                var filePath=Path.Combine(destinationFolder,$"{coach.NationalCode}.pdf");
                using(var pdfWriter=new PdfWriter(filePath))
                using(var pdfDocument=new PdfDocument(pdfWriter))
                using(var document=new Document(pdfDocument))
                {
                    var font=PdfFontFactory.CreateFont("Tahoma",PdfEncodings.IDENTITY_H);
                    document.SetFont(font);
                    document.SetTextAlignment(TextAlignment.RIGHT);

                    document.Add(new Paragraph($"{coach.FirstName} {coach.LastName}").SetFontSize(20).SetBold());
                    document.Add(new Paragraph($"کد ملی: {coach.NationalCode}").SetFontSize(12));
                    document.Add(new Paragraph(" "));

                    void AddSection(string title)
                    {
                        document.Add(new Paragraph(title).SetFontSize(14).SetBold().SetBackgroundColor(iText.Kernel.Colors.ColorConstants.LIGHT_GRAY).SetPadding(5));
                    }
                    void AddRow(string label,string value)
                    {
                        var p=new Paragraph().Add(new Text($"{label}: ").SetBold()).Add(new Text(value??""));
                        p.SetFontSize(10);
                        document.Add(p);
                    }

                    AddSection("اطلاعات فردی");
                    AddRow("نام پدر",coach.FatherName);
                    AddRow("شماره شناسنامه",coach.BirthCertificateNumber);
                    AddRow("کد پرسنلی",coach.PersonnelCode);
                    AddRow("تاریخ تولد",coach.BirthDate);
                    AddRow("جنسیت",coach.Gender);
                    AddRow("وضعیت تاهل",coach.MaritalStatus);
                    AddRow("تعداد فرزندان",coach.ChildrenCount?.ToString());
                    AddRow("رشته تحصیلی",coach.FieldOfStudy);
                    AddRow("تحصیلات",coach.Education);
                    AddRow("نوع عضویت",coach.MembershipType);
                    AddRow("تاریخ ورود",coach.EntryDate);
                    AddRow("آخرین مسئولیت",coach.LastPosition);
                    AddRow("نوع پرونده",coach.FileType);
                    AddRow("نام ناحیه",coach.DistrictName);
                    AddRow("وضعیت سربازی",coach.MilitaryStatus);
                    AddRow("وضعیت جسمانی",coach.PhysicalStatus);
                    AddRow("وزن",coach.Weight?.ToString());
                    AddRow("قد",coach.Height?.ToString());
                    AddRow("گروه خونی",coach.BloodType);
                    AddRow("وضعیت ایثارگری",coach.SacrificeStatus);
                    AddRow("شغل",coach.Job);
                    AddRow("آدرس محل کار",coach.WorkAddress);
                    AddRow("آدرس منزل",coach.HomeAddress);
                    AddRow("شماره تماس ۱",coach.Phone1);
                    AddRow("شماره تماس ۲",coach.Phone2);

                    var form=connection.QueryFirstOrDefault<CoachForm>("SELECT * FROM CoachForms WHERE CoachId=@coachId",new{coachId});
                    if(form!=null)
                    {
                        document.Add(new Paragraph(" "));
                        AddSection("فرم سازمانی");
                        AddRow("سطح مربی",form.CoachLevel?.ToString());
                        AddRow("پایه مربی",form.CoachBase?.ToString());
                        AddRow("شماره پرونده فیزیکی",form.PhysicalFileNumber?.ToString());
                        AddRow("وضعیت همکاری",form.CollaborationStatus);
                    }

                    var courses=connection.Query<CoachingCourse>("SELECT * FROM CoachingCourses WHERE CoachId=@coachId",new{coachId}).ToList();
                    if(courses.Count>0)
                    {
                        document.Add(new Paragraph(" "));
                        AddSection("دوره‌های مربیگری");
                        var table=new Table(new float[]{1,2,1,1,1,2});
                        table.SetWidth(UnitValue.CreatePercentValue(100));
                        table.AddHeaderCell(new Cell().Add(new Paragraph("نوع").SetFontSize(9)));
                        table.AddHeaderCell(new Cell().Add(new Paragraph("نام دوره").SetFontSize(9)));
                        table.AddHeaderCell(new Cell().Add(new Paragraph("شروع").SetFontSize(9)));
                        table.AddHeaderCell(new Cell().Add(new Paragraph("پایان").SetFontSize(9)));
                        table.AddHeaderCell(new Cell().Add(new Paragraph("روز").SetFontSize(9)));
                        table.AddHeaderCell(new Cell().Add(new Paragraph("توضیحات").SetFontSize(9)));
                        foreach(var c in courses)
                        {
                            table.AddCell(new Cell().Add(new Paragraph(c.CourseType??"").SetFontSize(8)));
                            table.AddCell(new Cell().Add(new Paragraph(c.CourseName??"").SetFontSize(8)));
                            table.AddCell(new Cell().Add(new Paragraph(c.StartDate??"").SetFontSize(8)));
                            table.AddCell(new Cell().Add(new Paragraph(c.EndDate??"").SetFontSize(8)));
                            table.AddCell(new Cell().Add(new Paragraph(c.DaysCount?.ToString()??"").SetFontSize(8)));
                            table.AddCell(new Cell().Add(new Paragraph(c.Description??"").SetFontSize(8)));
                        }
                        document.Add(table);
                    }

                    var banks=connection.Query<BankAccount>("SELECT * FROM BankAccounts WHERE CoachId=@coachId",new{coachId}).ToList();
                    if(banks.Count>0)
                    {
                        document.Add(new Paragraph(" "));
                        AddSection("اطلاعات بانکی");
                        var table=new Table(new float[]{2,2,2,2});
                        table.SetWidth(UnitValue.CreatePercentValue(100));
                        table.AddHeaderCell(new Cell().Add(new Paragraph("بانک").SetFontSize(9)));
                        table.AddHeaderCell(new Cell().Add(new Paragraph("حساب").SetFontSize(9)));
                        table.AddHeaderCell(new Cell().Add(new Paragraph("شبا").SetFontSize(9)));
                        table.AddHeaderCell(new Cell().Add(new Paragraph("کارت").SetFontSize(9)));
                        foreach(var b in banks)
                        {
                            table.AddCell(new Cell().Add(new Paragraph(b.BankName??"").SetFontSize(8)));
                            table.AddCell(new Cell().Add(new Paragraph(b.AccountNumber??"").SetFontSize(8)));
                            table.AddCell(new Cell().Add(new Paragraph(b.IBAN??"").SetFontSize(8)));
                            table.AddCell(new Cell().Add(new Paragraph(b.CardNumber??"").SetFontSize(8)));
                        }
                        document.Add(table);
                    }

                    var chess=connection.Query<ChessTrainingRecord>("SELECT * FROM ChessTrainingRecords WHERE CoachId=@coachId",new{coachId}).ToList();
                    if(chess.Count>0)
                    {
                        document.Add(new Paragraph(" "));
                        AddSection("تداوم آموزش شطرنج");
                        var table=new Table(new float[]{1,2,1,1,2});
                        table.SetWidth(UnitValue.CreatePercentValue(100));
                        table.AddHeaderCell(new Cell().Add(new Paragraph("سال").SetFontSize(9)));
                        table.AddHeaderCell(new Cell().Add(new Paragraph("نوع").SetFontSize(9)));
                        table.AddHeaderCell(new Cell().Add(new Paragraph("تاریخ").SetFontSize(9)));
                        table.AddHeaderCell(new Cell().Add(new Paragraph("نمره").SetFontSize(9)));
                        table.AddHeaderCell(new Cell().Add(new Paragraph("توضیحات").SetFontSize(9)));
                        foreach(var ch in chess)
                        {
                            table.AddCell(new Cell().Add(new Paragraph(ch.Year?.ToString()??"").SetFontSize(8)));
                            table.AddCell(new Cell().Add(new Paragraph(ch.ChessType??"").SetFontSize(8)));
                            table.AddCell(new Cell().Add(new Paragraph(ch.PlayDate??"").SetFontSize(8)));
                            table.AddCell(new Cell().Add(new Paragraph(ch.Score?.ToString()??"").SetFontSize(8)));
                            table.AddCell(new Cell().Add(new Paragraph(ch.Description??"").SetFontSize(8)));
                        }
                        document.Add(table);
                    }

                    var customValues=connection.Query<dynamic>("SELECT cf.Title,cfv.Value FROM CustomFieldValues cfv INNER JOIN CustomFields cf ON cf.Id=cfv.FieldId WHERE cfv.CoachId=@coachId",new{coachId}).ToList();
                    if(customValues.Count>0)
                    {
                        document.Add(new Paragraph(" "));
                        AddSection("ویژگی‌های پویا");
                        foreach(var cv in customValues)
                        {
                            AddRow((string)cv.Title,(string)cv.Value);
                        }
                    }
                }
            }
        }
    }
}