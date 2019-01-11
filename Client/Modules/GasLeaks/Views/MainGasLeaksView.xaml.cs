namespace GazRouter.GasLeaks.Views
{
    public partial class MainGasLeaksView
    {


        public MainGasLeaksView()
        {
            InitializeComponent();
        }


        //        private string GetReport()
        //        {
        //            var ms = new MemoryStream();
        //            var exportOptions = new GridViewExportOptions
        //                {
        //                    Format = ExportFormat.Html,
        //                    ShowColumnFooters = false,
        //                    ShowColumnHeaders = true,
        //                    ShowGroupFooters = false,
        //                };
        //            grid.Export(ms, exportOptions);
        //            ms.Seek(0L, SeekOrigin.Begin);
        //            var reader = new StreamReader(ms);
        //            string text = reader.ReadToEnd();

        //            string html =
        //                string.Format(
        //                    @"<!doctype html>
        //				<html><body>
        //				<header>
        //				<meta http-equiv='Content-Type' content='text/html; charset=UTF-8' />
        //				<style>
        //				 tr:first-child {{font-weight: bold;}}
        //				 td {{text-align: center;}}
        //				 </style>
        //				</header><div>Дата: {0:dd MMMM yyyy}</div><div>Время: {0:HH:mm}</div><div>ФИО: {1}</div><h4>Сводка по утечкам</h4>{2}
        //					</html><body/>",
        //                    DateTime.Now, UserProfile.Current.UserName, text);

        //            return html;
        //        }

        //private void ExportAsHtml(object sender, RoutedEventArgs e)
        //{
        //    string html = GetReport();
        //    HtmlPage.Window.Invoke("popupReport", html);
        //}

        //private void ExportAsExcel(object sender, RoutedEventArgs e)
        //{
        //    var dialog = new SaveFileDialog();

        //    const string extension = "xls";
        //    const string app = "Excel";

        //    dialog.DefaultExt = extension;
        //    dialog.Filter = String.Format("{1} files (*.{0})|*.{0}|All files (*.*)|*.*", extension, app);
        //    dialog.FilterIndex = 1;

        //    if (dialog.ShowDialog() != true) return;

        //    string html = GetReport();

        //    using (var w = new StreamWriter(dialog.OpenFile()))
        //    {
        //        w.WriteLine(html);
        //    }
        //}
    }
}