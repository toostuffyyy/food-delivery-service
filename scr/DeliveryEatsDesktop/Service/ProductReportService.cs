using desktop.Models;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System;
using System.Threading.Tasks;

namespace desktop.Services
{
    public class ProductReportService : IReportProductService
    {
        private readonly IFilePickerService _filePickerService;
        public ProductReportService(IFilePickerService filePickerService)
        {
            _filePickerService = filePickerService;
        }

        public async Task SaveDocument(ProductStatistic[] productStatistics, DateRange dateRange)
        {
            Uri saveFile = await _filePickerService.SaveFile(IFilePickerService.Filter.Docx);
            if (saveFile == null) return;
            
            using (WordprocessingDocument wordprocessingDocument = WordprocessingDocument.Create(saveFile.AbsolutePath, DocumentFormat.OpenXml.WordprocessingDocumentType.Document))
            {
                var mainPart = wordprocessingDocument.AddMainDocumentPart();
                mainPart.Document = new Document();
                Body body = new Body();
                mainPart.Document.Append(body);
                Paragraph paragraph = new Paragraph();
                ParagraphProperties paragraphProperties = new ParagraphProperties();
                paragraphProperties.AddChild(new Justification() { Val = JustificationValues.Center });
                paragraph.PrependChild(paragraphProperties);
                body.AppendChild(paragraph);
                
                string title = "Отчет по продажам продуктов";
                if (dateRange != null && dateRange.StartDate != null && dateRange.EndDate != null && dateRange.StartDate <= dateRange.EndDate)
                    title += $". За период с {dateRange.StartDate:dddd, dd-MMMM-yyyy} по {dateRange.EndDate:dddd, dd-MMMM-yyyy}";
                
                Run titleRun = new Run();
                titleRun.AppendChild(new Text(title));
                RunProperties runProperties = new RunProperties();
                runProperties.Append(new Bold());
                runProperties.Append(new FontSize() { Val = "20" });
                runProperties.Append(new RunFonts() { Ascii = "Times New Roman", HighAnsi = "Times New Roman", ComplexScript = "Times New Roman" });
                titleRun.PrependChild(runProperties);
                paragraph.AppendChild(titleRun);

                var table = new Table();
                TableProperties tableProperties = new TableProperties();
                TableBorders tableBorders = new TableBorders
                {
                    TopBorder = new TopBorder() { Val = new DocumentFormat.OpenXml.EnumValue<BorderValues>(BorderValues.Single) },
                    BottomBorder = new BottomBorder() { Val = new DocumentFormat.OpenXml.EnumValue<BorderValues>(BorderValues.Single) },
                    LeftBorder = new LeftBorder() { Val = new DocumentFormat.OpenXml.EnumValue<BorderValues>(BorderValues.Single) },
                    RightBorder = new RightBorder() { Val = new DocumentFormat.OpenXml.EnumValue<BorderValues>(BorderValues.Single) },
                    InsideHorizontalBorder = new InsideHorizontalBorder() { Val = BorderValues.Single },
                    InsideVerticalBorder = new InsideVerticalBorder() { Val = BorderValues.Single}
                };
                tableProperties.Append(tableBorders);
                tableProperties.Append(new Justification() { Val = JustificationValues.Center });
                table.AppendChild(tableProperties);

                TableRow titleTableRow = new TableRow();
                var tc1 = new TableCell();
                tc1.Append(new Paragraph(new Run(new Text("№"))));
                var tc2 = new TableCell();
                tc2.Append(new Paragraph(new Run(new Text("Название"))));
                var tc3 = new TableCell();
                tc3.Append(new Paragraph(new Run(new Text("Количество"))));
                var tc4 = new TableCell();
                tc4.Append(new Paragraph(new Run(new Text("Сумма"))));
                titleTableRow.Append(tc1);
                titleTableRow.Append(tc2);
                titleTableRow.Append(tc3);
                titleTableRow.Append(tc4);
                table.Append(titleTableRow);

                for (int i = 0; i < productStatistics.Length; i++)
                {
                    var newTableRow = new TableRow();
                    var ntc1 = new TableCell();
                    ntc1.Append(new Paragraph(new Run(new Text((i + 1).ToString()))));
                    var ntc2 = new TableCell();
                    ntc2.Append(new Paragraph(new Run(new Text(productStatistics[i].Product.Name))));
                    var ntc3 = new TableCell();
                    ntc3.Append(new Paragraph(new Run(new Text(productStatistics[i].Count.ToString()))));
                    var ntc4 = new TableCell();
                    ntc4.Append(new Paragraph(new Run(new Text(productStatistics[i].Sum.ToString($"{0:N} руб.")))));
                    newTableRow.Append(ntc1);
                    newTableRow.Append(ntc2);
                    newTableRow.Append(ntc3);
                    newTableRow.Append(ntc4);
                    table.Append(newTableRow);
                }
                body.Append(table);
                mainPart.Document.Save();
            }
        }
    }
}
