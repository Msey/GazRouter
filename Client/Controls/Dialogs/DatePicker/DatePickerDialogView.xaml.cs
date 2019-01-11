using System.Globalization;

namespace GazRouter.Controls.Dialogs.DatePicker
{
    public partial class DatePickerDialogView
    {
        public DatePickerDialogView()
        {
            InitializeComponent();
            datePicker.Culture = new CultureInfo("ru-RU") { DateTimeFormat = { ShortDatePattern = "MMMM yyyy" } };
        }
    }
}