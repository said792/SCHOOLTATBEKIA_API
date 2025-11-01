using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;

public class ReportTemplate : IDocument
{
    Dictionary<string, object> Company { get; }
    Dictionary<string, object> InvoiceHeader { get; }
    List<Dictionary<string, object>> InvoiceDetails { get; }

    string HeaderTitle { get; }
    string DetailsTitle { get; }

    public ReportTemplate(
        Dictionary<string, object> company,
        Dictionary<string, object> header,
        List<Dictionary<string, object>> details,
        string headerTitle = "ملخص الفاتورة",
        string detailsTitle = "تفاصيل الفاتورة")
    {
        Company = company;
        InvoiceHeader = header;
        InvoiceDetails = details;
        HeaderTitle = headerTitle;
        DetailsTitle = detailsTitle;
    }

    public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

    public void Compose(IDocumentContainer container)
    {
        container.Page(page =>
        {
            page.Margin(20);
            page.Header().Element(ComposeHeader); // الهيدر كما هو
            page.Content().Element(ComposeContent);
            page.Footer().Element(ComposeFooter);
        });
    }

    void ComposeHeader(IContainer container)
    {
        container.Row(row =>
        {
            // اللوجو على اليمين
            row.ConstantColumn(100).Height(100).AlignMiddle().AlignLeft().Element(e =>
            {
                var logoData = Company.TryGetValue("اللوجو", out var logoVal) ? logoVal?.ToString() : "";
                if (!string.IsNullOrEmpty(logoData))
                {
                    var imageData = Convert.FromBase64String(logoData.Contains(",") ? logoData.Split(',')[1] : logoData);
                    e.Image(imageData).FitHeight();
                }
                else
                {
                    e.Text("لا يوجد شعار").Italic().FontSize(10);
                }
            });

            // بيانات الشركة على اليسار
            row.RelativeColumn().Stack(stack =>
            {
                stack.Item().Text(Company.TryGetValue("المكتب", out var name) ? name?.ToString() : "اسم الشركة")
                    .FontSize(18).Bold().AlignRight();

                stack.Item().Text($"📞 {Company.GetValueOrDefault("رقم الهاتف")}")
                    .AlignRight();
                stack.Item().Text($"📧 {Company.GetValueOrDefault("ايميل المكتب")}")
                    .AlignRight();
                stack.Item().Text($"📍 {Company.GetValueOrDefault("العنوان")}")
                    .AlignRight();
            });
        });
    }

    void ComposeContent(IContainer container)
    {
        container.Stack(stack =>
        {
            // -------------------------------
            // 🔹 جدول العناوين الديناميكي الأول
            // -------------------------------
            stack.Item().PaddingVertical(15)
               .Text(HeaderTitle)
                .Bold().FontSize(14)
                .AlignRight();

            if (InvoiceHeader != null && InvoiceHeader.Any())
            {
                var keys = InvoiceHeader.Keys.Reverse().ToList();
                var values = InvoiceHeader.Values.Reverse().ToList();

                stack.Item().Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        foreach (var _ in keys)
                            columns.RelativeColumn();
                    });

                    foreach (var key in keys)
                    {
                        table.Cell().Element(CellStyle)
                            .Background(Colors.Grey.Lighten2)
                            .Padding(5)
                            .Text(key)
                            .Bold()
                            .FontSize(10)
                            .AlignCenter();
                    }

                    foreach (var value in values)
                    {
                        table.Cell().Element(CellStyle)
                            .Padding(5)
                            .Text(value?.ToString() ?? "")
                            .FontSize(10)
                            .AlignCenter();
                    }
                });
            }

            // -------------------------------
            // 🔹 جدول العناوين الديناميكي الثاني
            // -------------------------------
            stack.Item().PaddingVertical(15)
                .Text(DetailsTitle)
                .FontSize(14)
                .Bold()
                .AlignRight();

            if (InvoiceDetails != null && InvoiceDetails.Any())
            {
                var columns = InvoiceDetails[0].Keys.Reverse().ToList();

                stack.Item().Table(table =>
                {
                    table.ColumnsDefinition(cols =>
                    {
                        foreach (var _ in columns)
                            cols.RelativeColumn();
                    });

                    // ✅ رؤوس الأعمدة تتكرر في كل صفحة
                    table.Header(header =>
                    {
                        foreach (var key in columns)
                        {
                            header.Cell().Element(CellStyle)
                                .Background(Colors.Grey.Lighten2)
                                .Padding(5)
                                .Text(key)
                                .Bold()
                                .FontSize(10)
                                .AlignCenter();
                        }
                    });

                    // البيانات
                    foreach (var row in InvoiceDetails)
                    {
                        foreach (var key in columns)
                        {
                            var value = row[key]?.ToString();
                            table.Cell().Element(CellStyle)
                                .Padding(5)
                                .Text(value ?? "")
                                .FontSize(10)
                                .AlignCenter();
                        }
                    }
                });
            }
            else
            {
                stack.Item().Text("لا توجد تفاصيل")
                    .Italic()
                    .FontColor(Colors.Grey.Darken2)
                    .AlignRight();
            }
        });
    }

    void ComposeFooter(IContainer container)
    {
        var companyName = Company.TryGetValue("المكتب", out var val) ? val?.ToString() : "اسم الشركة";
        container.AlignCenter().Text($"شكراً لتعاملكم معنا مع تحيات شركة {companyName}")
            .FontSize(12)
            .Italic()
            .FontColor(Colors.Grey.Darken2);
    }

    IContainer CellStyle(IContainer container)
    {
        return container.Border(1).BorderColor(Colors.Grey.Lighten2).Padding(5);
    }
}
