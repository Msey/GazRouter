using System.Windows.Forms;
using ExcelDna.Integration;
namespace GR_ExcelFunc.View
{
    internal static class WinFormManager
    {
        private static Form _form;
        public static void ShowForm()
        {
            if (_form == null)
            {
               //     var apl = (Microsoft.Office.Interop.Excel.Application)ExcelDnaUtil.Application;
                var data = new Model.SelectObjectParameterData();
                var view = new View.GetObjectParameterControl();
                var presnter = new Presenter.SelectObjectParameterPresenter(view, data);
                _form = new Form
                {
                    AutoSize = true,
                    StartPosition = FormStartPosition.CenterParent,
                    MinimizeBox = false,
                    MaximizeBox = false,
                    Text = "Выбор параметра",
                    ShowIcon = false,
                    ShowInTaskbar = false
                    
                };

                
                _form.Controls.Add(view);
                _form.ShowDialog();


            }
            else
            {
                _form.ShowDialog();
            }
        }
    }
}
