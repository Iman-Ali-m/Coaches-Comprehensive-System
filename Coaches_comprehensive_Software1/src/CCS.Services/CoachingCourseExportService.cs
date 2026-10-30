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
    public class CoachingCourseExportService
    {
        public class CourseExportRow
        {
            public string FirstName{get;set;}
            public string LastName{get;set;}
            public string FatherName{get;set;}
            public string NationalCode{get;set;}
            public string BirthCertificateNumber{get;set;}
            public string FileType{get;set;}
            public string Phone1{get;set;}
            public string CourseType{get;set;}
            public string CourseName{get;set;}
        }

        private List<CourseExportRow> GetData(long coachId)
        {
            using(var connection=Database.Open())
            {
                var sql="SELECT c.FirstName,c.LastName,c.FatherName,c.NationalCode,c.BirthCertificateNumber,c.FileType,c.Phone1,cc.CourseType,cc.CourseName FROM Coaches c INNER JOIN CoachingCourses cc ON c.Id=cc.CoachId WHERE c.Id=@coachId";
                return connection.Query<CourseExportRow>(sql,new{coachId}).ToList();
            }
        }

        public void ExportExcel(List<long> coachIds,string filePath)
        {
            using(var workbook=new XLWorkbook())
            {
                var worksheet=workbook.Worksheets.Add("دوره های مربیگری");
                worksheet.RightToLeft=true;
                var row=1;
                worksheet.Cell(row,1).Value="نام";
                worksheet.Cell(row,2).Value="نام خانوادگی";
                worksheet.Cell(row,3).Value="نام پدر";
                worksheet.Cell(row,4).Value="شماره ملی";
                worksheet.Cell(row,5).Value="شماره شناسنامه";
                worksheet.Cell(row,6).Value="نوع پرونده";
                worksheet.Cell(row,7).Value="نوع دوره";
                worksheet.Cell(row,8).Value="نام دوره";
                worksheet.Cell(row,9).Value="شماره تماس";
                worksheet.Row(row).Style.Font.Bold=true;
                row++;
                foreach(var id in coachIds)
                {
                    var data=GetData(id);
                    foreach(var item in data)
                    {
                        var fileTypePersian=string.IsNullOrEmpty(item.FileType)?"":(FileTypes.PersianNames.ContainsKey(item.FileType)?FileTypes.PersianNames[item.FileType]:"");
                        var courseTypePersian=string.IsNullOrEmpty(item.CourseType)?"":(CourseTypes.PersianNames.ContainsKey(item.CourseType)?CourseTypes.PersianNames[item.CourseType]:"");
                        worksheet.Cell(row,1).Value=item.FirstName;
                        worksheet.Cell(row,2).Value=item.LastName;
                        worksheet.Cell(row,3).Value=item.FatherName;
                        worksheet.Cell(row,4).Value=item.NationalCode;
                        worksheet.Cell(row,5).Value=item.BirthCertificateNumber;
                        worksheet.Cell(row,6).Value=fileTypePersian;
                        worksheet.Cell(row,7).Value=courseTypePersian;
                        worksheet.Cell(row,8).Value=item.CourseName;
                        worksheet.Cell(row,9).Value=item.Phone1;
                        row++;
                    }
                }
                worksheet.Columns().AdjustToContents();
                workbook.SaveAs(filePath);
            }
        }

        public void ExportPdf(List<long> coachIds,string filePath)
        {
            using(var pdfWriter=new PdfWriter(filePath))
            using(var pdfDocument=new PdfDocument(pdfWriter))
            using(var document=new Document(pdfDocument))
            {
                var font=PdfFontFactory.CreateFont("Tahoma",PdfEncodings.IDENTITY_H);
                document.SetFont(font);
                document.SetTextAlignment(TextAlignment.RIGHT);
                document.Add(new Paragraph("گزارش دوره های مربیگری").SetFontSize(16).SetBold());
                document.Add(new Paragraph(" "));
                var table=new Table(new float[]{1,1,1,1,1,1,1,1,1});
                table.SetWidth(UnitValue.CreatePercentValue(100));
                table.AddHeaderCell(new Cell().Add(new Paragraph("نام").SetFontSize(9)));
                table.AddHeaderCell(new Cell().Add(new Paragraph("نام خانوادگی").SetFontSize(9)));
                table.AddHeaderCell(new Cell().Add(new Paragraph("نام پدر").SetFontSize(9)));
                table.AddHeaderCell(new Cell().Add(new Paragraph("شماره ملی").SetFontSize(9)));
                table.AddHeaderCell(new Cell().Add(new Paragraph("شماره شناسنامه").SetFontSize(9)));
                table.AddHeaderCell(new Cell().Add(new Paragraph("نوع پرونده").SetFontSize(9)));
                table.AddHeaderCell(new Cell().Add(new Paragraph("نوع دوره").SetFontSize(9)));
                table.AddHeaderCell(new Cell().Add(new Paragraph("نام دوره").SetFontSize(9)));
                table.AddHeaderCell(new Cell().Add(new Paragraph("شماره تماس").SetFontSize(9)));
                foreach(var id in coachIds)
                {
                    var data=GetData(id);
                    foreach(var item in data)
                    {
                        var fileTypePersian=string.IsNullOrEmpty(item.FileType)?"":(FileTypes.PersianNames.ContainsKey(item.FileType)?FileTypes.PersianNames[item.FileType]:"");
                        var courseTypePersian=string.IsNullOrEmpty(item.CourseType)?"":(CourseTypes.PersianNames.ContainsKey(item.CourseType)?CourseTypes.PersianNames[item.CourseType]:"");
                        table.AddCell(new Cell().Add(new Paragraph(item.FirstName??"").SetFontSize(8)));
                        table.AddCell(new Cell().Add(new Paragraph(item.LastName??"").SetFontSize(8)));
                        table.AddCell(new Cell().Add(new Paragraph(item.FatherName??"").SetFontSize(8)));
                        table.AddCell(new Cell().Add(new Paragraph(item.NationalCode??"").SetFontSize(8)));
                        table.AddCell(new Cell().Add(new Paragraph(item.BirthCertificateNumber??"").SetFontSize(8)));
                        table.AddCell(new Cell().Add(new Paragraph(fileTypePersian).SetFontSize(8)));
                        table.AddCell(new Cell().Add(new Paragraph(courseTypePersian).SetFontSize(8)));
                        table.AddCell(new Cell().Add(new Paragraph(item.CourseName??"").SetFontSize(8)));
                        table.AddCell(new Cell().Add(new Paragraph(item.Phone1??"").SetFontSize(8)));
                    }
                }
                document.Add(table);
            }
        }

        public List<long> GetCoachIdsWithCoursesByFileType(string fileType)
        {
            using(var connection=Database.Open())
            {
                if(string.IsNullOrEmpty(fileType))
                {
                    return connection.Query<long>("SELECT DISTINCT c.Id FROM Coaches c INNER JOIN CoachingCourses cc ON c.Id=cc.CoachId").ToList();
                }
                return connection.Query<long>("SELECT DISTINCT c.Id FROM Coaches c INNER JOIN CoachingCourses cc ON c.Id=cc.CoachId WHERE c.FileType=@fileType",new{fileType}).ToList();
            }
        }
    }
}