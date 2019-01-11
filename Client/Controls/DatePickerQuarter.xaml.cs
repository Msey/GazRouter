using System;
using System.Globalization;
using System.Windows;
using System.Windows.Input;
using GazRouter.DTO.Dictionaries.PeriodTypes;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.Calendar;
using Utils.Extensions;

namespace GazRouter.Controls
{
    public partial class DatePickerQuarter
    {
        public DatePickerQuarter()
        {
            InitializeComponent();
        }

        #region Dependency properties

        #region SelectedDate

        public DateTime? SelectedDate
        {
            get
            {
                return (DateTime?)GetValue(SelectedDateProperty);
            }
            set
            {
                SetValue(SelectedDateProperty, value);
            }
        }

        public static readonly DependencyProperty SelectedDateProperty =
            DependencyProperty.Register("SelectedDate",
            typeof(DateTime?),
            typeof(DatePickerQuarter),
            new PropertyMetadata(SelectedDateChanged));

        private static void SelectedDateChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var c = sender as DatePickerQuarter;
            if (c != null)
            {
                c.datePicker.SelectedValue = (DateTime?) e.NewValue;
            }
        }

        #endregion

        #region PeriodType

        public PeriodTypeDTO PeriodType
        {
            get
            {
                return (PeriodTypeDTO)GetValue(PeriodTypeProperty);
            }
            set
            {
                SetValue(PeriodTypeProperty, value);
            }
        }

        public static readonly DependencyProperty PeriodTypeProperty =
            DependencyProperty.Register("PeriodType",
            typeof(PeriodTypeDTO),
            typeof(DatePickerQuarter),
            new PropertyMetadata(SelectedPeriodTypeChanged));

        private static void SelectedPeriodTypeChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var c = sender as DatePickerQuarter;
            var dto = e.NewValue as PeriodTypeDTO;
            if (c != null && dto != null)
            {
                switch (dto.PeriodType)
                {
                    case DTO.Dictionaries.PeriodTypes.PeriodType.Month:
                        c.datePicker.Culture = new CultureInfo("ru-RU") { DateTimeFormat = { ShortDatePattern = "MMMM yyyy" } };
                        c.datePicker.CalendarStyle = null;
                        c.datePicker.DateSelectionMode = DateSelectionMode.Month;
                        c.datePicker.SelectedValue = new DateTime(c.datePicker.SelectedValue.Value.Date.Year, c.datePicker.SelectedValue.Value.Date.Month, 1);
                        break;

                    case DTO.Dictionaries.PeriodTypes.PeriodType.Year:
                        c.datePicker.Culture = new CultureInfo("ru-RU") { DateTimeFormat = { ShortDatePattern = "yyyy" } };
                        c.datePicker.CalendarStyle = null;
                        c.datePicker.DateSelectionMode = DateSelectionMode.Year;
                        c.datePicker.SelectedValue = new DateTime(c.datePicker.SelectedValue.Value.Date.Year, 1, 1);
                        break;

                    case DTO.Dictionaries.PeriodTypes.PeriodType.Quarter:
                        var culture =  new CultureInfo("ru-RU")
                                           {
                                               DateTimeFormat =
                                                   {
                                                       AbbreviatedMonthNames = new[] { "1", "1", "1", "2", "2", "2", "3", "3", "3", "4", "4", "4", string.Empty },
                                                       ShortDatePattern = "MMM квартал yyyy"
                                                   }
                                           };
                        c.datePicker.Culture = culture;
                        c.datePicker.CalendarStyle = c.QuarterStyle;
                        c.datePicker.DateSelectionMode = DateSelectionMode.Month;
                        if (c.datePicker.SelectedValue.Value.Month >= 1 && c.datePicker.SelectedValue.Value.Month < 4)
                        {
                            c.datePicker.SelectedValue = new DateTime(c.datePicker.SelectedValue.Value.Date.Year, 1, 1);
                        }
                        else
                        {
                            if (c.datePicker.SelectedValue.Value.Month >= 4 && c.datePicker.SelectedValue.Value.Month < 7)
                            {
                                c.datePicker.SelectedValue = new DateTime(c.datePicker.SelectedValue.Value.Date.Year, 4, 1);
                            }
                            else
                            {
                                if (c.datePicker.SelectedValue.Value.Month >= 7 && c.datePicker.SelectedValue.Value.Month < 10)
                                {
                                    c.datePicker.SelectedValue = new DateTime(c.datePicker.SelectedValue.Value.Year, 7, 1);
                                }
                                else
                                {
                                    c.datePicker.SelectedValue = new DateTime(c.datePicker.SelectedValue.Value.Year, 10, 1);
                                }
                            }
                        }
                        break;
                }
            }
        }

        #endregion

        #endregion

        private void datePicker_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectedDate = datePicker.SelectedValue.Value.Date.ToLocal();
        }

        private void datePicker_KeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = true;
        }
    }

    public class CustomMonthButtonStyleSelector : StyleSelector
    {
        public Style DefaultStyle { get; set; }
        public Style HiddenStyle { get; set; }

        public override Style SelectStyle(object item, DependencyObject container)
        {
            var month = (item as CalendarButtonContent).Date.Month;

            if (month == 1 || month == 4 || month == 7 || month == 10)
                return DefaultStyle;

            return HiddenStyle;
        }
    }
}
