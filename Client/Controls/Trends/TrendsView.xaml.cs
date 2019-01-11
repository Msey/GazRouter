using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using GazRouter.Application;
using GazRouter.Common.Cache;
using Microsoft.Practices.ServiceLocation;
using Telerik.Windows.Controls.ChartView;
using Telerik.Windows.Documents.FormatProviders.Pdf;
using Telerik.Windows.Documents.Layout;
using Telerik.Windows.Documents.Model;
using Telerik.Windows.Media.Imaging;
using Border = Telerik.Windows.Documents.Model.Border;

namespace GazRouter.Controls.Trends
{
    public partial class TrendsView
    {
        
		public TrendsView()
        {
            InitializeComponent();
        }


        private void ChartTrackBallBehavior_OnTrackInfoUpdated(object sender, TrackBallInfoEventArgs e)
        {
            var pt = e?.Context?.ClosestDataPoint?.DataPoint.DataItem as PropertyValue;
            if (pt != null)
                e.Header = pt.Date.ToString("dd.MM.yyyy HH:mm");
        }





        //      public new TrendsViewModel DataContext
        //      {
        //          get { return (TrendsViewModel) base.DataContext; }
        //          set
        //          {
        //              value.Control = ChartControl;
        //              value.RadTimeBar = RadTimeBarControl;
        //              base.DataContext = value;
        //          }
        //      }






        private void OnExportPDFButtonClicked(object sender, RoutedEventArgs e)
        {
            var dialog = new SaveFileDialog { Filter = "Adobe PDF Document (*.pdf)|*.pdf" };
            if (dialog.ShowDialog() != true) return;

            using (var fileStream = dialog.OpenFile())
            {
                var document = new RadDocument();
                AddChartAndGridToDocument(document);
                
                document.LayoutMode = DocumentLayoutMode.Paged;
                document.Measure(RadDocument.MAX_DOCUMENT_SIZE);
                document.Arrange(new RectangleF(PointF.Empty, document.DesiredSize));
                var provider = new PdfFormatProvider();
                provider.Export(document, fileStream);
            }
        }
        
        private void AddChartAndGridToDocument(RadDocument document)
        {
            var context = (TrendsViewModel) DataContext;
            
            var section = new Section
            {
                PageOrientation = PageOrientation.Landscape,
                PageSize = PaperTypeConverter.ToSize(PaperTypes.A4)
            };
            document.Sections.Add(section);

            var mainTable = new Table();
            section.Blocks.Add(mainTable);
            



            // Добавить график
            var chartRow = new TableRow();
            mainTable.Rows.Add(chartRow);

            var chartCell = new TableCell();
            chartRow.Cells.Add(chartCell);

            var chartParagraph = new Paragraph();
            chartCell.Blocks.Add(chartParagraph);

            using (var ms = new MemoryStream())
            {
                ExportExtensions.ExportToImage(Chart, ms, new PngBitmapEncoder());

                var imageWidth = Chart.ActualWidth;
                var imageHeight = Chart.ActualHeight;

                if (imageWidth > 900)
                {
                    imageWidth = 900;
                    imageHeight = Chart.ActualHeight * imageWidth / Chart.ActualWidth;
                }

                var image = new ImageInline(ms, new Size(imageWidth, imageHeight), "png");

                chartParagraph.Inlines.Add(image);
            }
            



            // Добавить таблицу с перечнем трендов
            var gridTable = new Table { Borders = new TableBorders(new Border(BorderStyle.Single)) };

            var gridRow = new TableRow();
            var gridCell = new TableCell();
            gridCell.Blocks.Add(gridTable);
            gridRow.Cells.Add(gridCell);
            mainTable.Rows.Add(gridRow);

            //Headers part
            var headerRow = new TableRow();

            var header1 = new TableCell { PreferredWidth = new TableWidthUnit(TableWidthUnitType.Fixed, 100) };
            AddCellValue(header1, "Тип", true);
            headerRow.Cells.Add(header1);

            var header2 = new TableCell { PreferredWidth = new TableWidthUnit(TableWidthUnitType.Fixed, 300) };
            AddCellValue(header2, "Объект", true);
            headerRow.Cells.Add(header2);

            var header3 = new TableCell { PreferredWidth = new TableWidthUnit(TableWidthUnitType.Fixed, 100) };
            AddCellValue(header3, "Свойство", true);
            headerRow.Cells.Add(header3);

            var header4 = new TableCell { PreferredWidth = new TableWidthUnit(TableWidthUnitType.Fixed, 60) };
            AddCellValue(header4, "Min", true);
            headerRow.Cells.Add(header4);

            var header5 = new TableCell { PreferredWidth = new TableWidthUnit(TableWidthUnitType.Fixed, 60) };
            AddCellValue(header5, "Avg", true);
            headerRow.Cells.Add(header5);

            var header6 = new TableCell { PreferredWidth = new TableWidthUnit(TableWidthUnitType.Fixed, 60) };
            AddCellValue(header6, "Max", true);
            headerRow.Cells.Add(header6);

            var header7 = new TableCell { PreferredWidth = new TableWidthUnit(TableWidthUnitType.Fixed, 60) };
            AddCellValue(header7, "Период", true);
            headerRow.Cells.Add(header7);

            var header9 = new TableCell { PreferredWidth = new TableWidthUnit(TableWidthUnitType.Fixed, 60) };
            AddCellValue(header9, "Ед.изм.", true);
            headerRow.Cells.Add(header9);

            var header10 = new TableCell { PreferredWidth = new TableWidthUnit(TableWidthUnitType.Fixed, 20) };
            AddCellValue(header10, "   ", true);
            headerRow.Cells.Add(header10);

            gridTable.Rows.Add(headerRow);

            // Table body
            foreach (var trend in context.TrendList.Where(t => t.IsVisible))
            {
                var row = new TableRow();

                var cell1 = new TableCell();
                AddCellValue(cell1,
                    ClientCache.DictionaryRepository.EntityTypes.Single(et => et.EntityType == trend.Entity.EntityType)
                        .Name);
                row.Cells.Add(cell1);

                var cell2 = new TableCell();
                AddCellValue(cell2, trend.Entity.ShortPath);
                row.Cells.Add(cell2);

                var cell3 = new TableCell();
                AddCellValue(cell3, trend.PropertyTypeDto.Name);
                row.Cells.Add(cell3);

                var cell4 = new TableCell();
                AddCellValue(cell4, trend.Min.ToString());
                row.Cells.Add(cell4);

                var cell5 = new TableCell();
                AddCellValue(cell5, trend.Avg.ToString());
                row.Cells.Add(cell5);

                var cell6 = new TableCell();
                AddCellValue(cell6, trend.Max.ToString());
                row.Cells.Add(cell6);

                var cell7 = new TableCell();
                AddCellValue(cell7,
                    ClientCache.DictionaryRepository.PeriodTypes.Single(pt => pt.PeriodType == trend.PeriodType).Name);
                row.Cells.Add(cell7);

                var cell9 = new TableCell();
                AddCellValue(cell9, trend.UnitName);
                row.Cells.Add(cell9);

                var cell10 = CreateColorLineCell(trend);
                row.Cells.Add(cell10);

                gridTable.Rows.Add(row);
            }
        }
        private static IClientCache ClientCache => ServiceLocator.Current.GetInstance<IClientCache>();
        private void AddCellValue(TableCell cell, string value, bool isHeader = false)
        {
            if (value == string.Empty) return;
            var paragraph = new Paragraph();
            cell.Blocks.Add(paragraph);

            var span = new Span
            {
                Text = value,
                FontSize = 10,
                FontWeight = isHeader ? FontWeights.Bold : FontWeights.Normal
            };

            paragraph.Inlines.Add(span);
        }

        private TableCell CreateColorLineCell(Trend trend)
        {
            var resultCell = new TableCell { Padding = new Padding(0) };

            var paragraph = new Paragraph();
            resultCell.Blocks.Add(paragraph);

            var span1 = new Span
            {
                Text = "   ",
                FontSize = 6
            };
            var span2 = new Span
            {
                Text = "   ",
                FontSize = 6,
                HighlightColor = trend.Color
            };
            var span3 = new Span
            {
                Text = "   ",
                FontSize = 6
            };

            paragraph.Inlines.Add(span1);
            paragraph.Inlines.Add(span2);
            paragraph.Inlines.Add(span3);
            return resultCell;
        }
    }
}