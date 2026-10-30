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
    public class ExportPaymentService
    {
        public void ExportExcel(List<long> coachIds,string filePath)
        {
            using(var workbook=new XLWorkbook())
            {
                var worksheet=workbook.Worksheets.Add("پرداخت ها");
                worksheet.RightToLeft=true;
                var row=1;
                worksheet.Cell(row,1).Value="نام";
                worksheet.Cell(row,2).Value="نام خانوادگی";
                worksheet.Cell(row,3).Value="نام پدر";
                worksheet.Cell(row,4).Value="شماره ملی";
                worksheet.Cell(row,5).Value="موضوع";
                worksheet.Cell(row,6).Value="تاریخ";
                worksheet.Cell(row,7).Value="ساعت شروع";
                worksheet.Cell(row,8).Value="ساعت پایان";
                worksheet.Cell(row,9).Value="مکان";
                worksheet.Cell(row,10).Value="تعداد روز";
                worksheet.Cell(row,11).Value="تعداد کلاس";
                worksheet.Cell(row,12).Value="مبلغ پرداختی";
                worksheet.Cell(row,13).Value="مبلغ برگشتی";
                worksheet.Cell(row,14).Value="تاریخ پرداخت";
                worksheet.Row(row).Style.Font.Bold=true;
                row++;
                using(var connection=Database.Open())
                {
                    foreach(var id in coachIds)
                    {
                        var coach=connection.QueryFirstOrDefault<Coach>("SELECT * FROM Coaches WHERE Id=@id",new{id});
                        if(coach==null)continue;
                        var payments=connection.Query<Payment>("SELECT * FROM Payments WHERE CoachId=@id",new{id});
                        foreach(var pay in payments)
                        {
                            worksheet.Cell(row,1).Value=coach.FirstName;
                            worksheet.Cell(row,2).Value=coach.LastName;
                            worksheet.Cell(row,3).Value=coach.FatherName;
                            worksheet.Cell(row,4).Value=coach.NationalCode;
                            worksheet.Cell(row,5).Value=pay.Subject;
                            worksheet.Cell(row,6).Value=pay.PaymentDate;
                            worksheet.Cell(row,7).Value=pay.StartTime;
                            worksheet.Cell(row,8).Value=pay.EndTime;
                            worksheet.Cell(row,9).Value=pay.Location;
                            worksheet.Cell(row,10).Value=pay.DaysCount;
                            worksheet.Cell(row,11).Value=pay.ClassesCount;
                            worksheet.Cell(row,12).Value=pay.PaidAmount;
                            worksheet.Cell(row,13).Value=pay.ReturnedAmount;
                            worksheet.Cell(row,14).Value=pay.PaidDate;
                            row++;
                        }
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
                foreach(var id in coachIds)
                {
                    using(var connection=Database.Open())
                    {
                        var coach=connection.QueryFirstOrDefault<Coach>("SELECT * FROM Coaches WHERE Id=@id",new{id});
                        if(coach==null)continue;
                        var payments=connection.Query<Payment>("SELECT * FROM Payments WHERE CoachId=@id",new{id}).ToList();
                        if(payments.Count==0)continue;
                        document.Add(new Paragraph($"{coach.FirstName} {coach.LastName} - {coach.NationalCode}").SetFontSize(14).SetBold());
                        var table=new Table(new float[]{2,2,2,2,2,2,2,2,2,2});
                        table.SetWidth(UnitValue.CreatePercentValue(100));
                        table.AddHeaderCell(new Cell().Add(new Paragraph("موضوع").SetFontSize(9)));
                        table.AddHeaderCell(new Cell().Add(new Paragraph("تاریخ").SetFontSize(9)));
                        table.AddHeaderCell(new Cell().Add(new Paragraph("شروع").SetFontSize(9)));
                        table.AddHeaderCell(new Cell().Add(new Paragraph("پایان").SetFontSize(9)));
                        table.AddHeaderCell(new Cell().Add(new Paragraph("مکان").SetFontSize(9)));
                        table.AddHeaderCell(new Cell().Add(new Paragraph("تعداد روز").SetFontSize(9)));
                        table.AddHeaderCell(new Cell().Add(new Paragraph("تعداد کلاس").SetFontSize(9)));
                        table.AddHeaderCell(new Cell().Add(new Paragraph("مبلغ پرداختی").SetFontSize(9)));
                        table.AddHeaderCell(new Cell().Add(new Paragraph("مبلغ برگشتی").SetFontSize(9)));
                        table.AddHeaderCell(new Cell().Add(new Paragraph("تاریخ پرداخت").SetFontSize(9)));
                        foreach(var pay in payments)
                        {
                            table.AddCell(new Cell().Add(new Paragraph(pay.Subject??"").SetFontSize(8)));
                            table.AddCell(new Cell().Add(new Paragraph(pay.PaymentDate??"").SetFontSize(8)));
                            table.AddCell(new Cell().Add(new Paragraph(pay.StartTime??"").SetFontSize(8)));
                            table.AddCell(new Cell().Add(new Paragraph(pay.EndTime??"").SetFontSize(8)));
                            table.AddCell(new Cell().Add(new Paragraph(pay.Location??"").SetFontSize(8)));
                            table.AddCell(new Cell().Add(new Paragraph(pay.DaysCount?.ToString()??"").SetFontSize(8)));
                            table.AddCell(new Cell().Add(new Paragraph(pay.ClassesCount?.ToString()??"").SetFontSize(8)));
                            table.AddCell(new Cell().Add(new Paragraph(pay.PaidAmount?.ToString()??"").SetFontSize(8)));
                            table.AddCell(new Cell().Add(new Paragraph(pay.ReturnedAmount?.ToString()??"").SetFontSize(8)));
                            table.AddCell(new Cell().Add(new Paragraph(pay.PaidDate??"").SetFontSize(8)));
                        }
                        document.Add(table);
                        document.Add(new AreaBreak());
                    }
                }
            }
        }

        public List<long> GetCoachIdsByFileType(string fileType)
        {
            using(var connection=Database.Open())
            {
                if(string.IsNullOrEmpty(fileType))
                {
                    return connection.Query<long>("SELECT Id FROM Coaches").ToList();
                }
                return connection.Query<long>("SELECT Id FROM Coaches WHERE FileType=@fileType",new{fileType}).ToList();
            }
        }
    }
}