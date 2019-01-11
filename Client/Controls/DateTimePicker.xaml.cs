using System;
using System.Windows;
using Telerik.Windows.Controls;

namespace GazRouter.Controls
{
	public partial class DateTimePicker
	{
        #region SelectedDateTime dependency property

        public DateTime? SelectedDateTime
        {
            get
            {
                return (DateTime?)GetValue(SelectedDateTimeProperty);
            }
            set
            {
                SetValue(SelectedDateTimeProperty, value);
            }
        }

        public static readonly DependencyProperty SelectedDateTimeProperty =
            DependencyProperty.Register("SelectedDateTime",
            typeof(DateTime?),
            typeof(DateTimePicker),
            new PropertyMetadata(null, SelectedDateTimeChanged));

        private static void SelectedDateTimeChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var c = sender as DateTimePicker;

            if (c != null)
            {
                c.TimePicker.SelectedValue = (DateTime?) e.NewValue;
            }
        }

        #endregion

        #region DisableSelectFuture dependency property

	    public static readonly DependencyProperty DisableSelectFutureProperty =
            DependencyProperty.Register("DisableSelectFuture", typeof(bool), typeof(DateTimePicker), new PropertyMetadata(false, DisableSelectFutureChanged));

	    public bool DisableSelectFuture
	    {
	        get { return (bool) GetValue(DisableSelectFutureProperty); }
	        set { SetValue(DisableSelectFutureProperty, value); }
	    }

     

	    private static void DisableSelectFutureChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	    {
            var c = d as DateTimePicker;

            if (c != null)
            {
                bool disable = e.NewValue is bool && (bool) e.NewValue;

                var selectableDateEnd = disable ? DateTime.Now : (DateTime?) null;
                c.TimePicker.SelectableDateEnd = selectableDateEnd;
                c.TimePicker.DisplayDateEnd = selectableDateEnd;
                c.DatePicker.SelectableDateEnd = selectableDateEnd;
                c.DatePicker.DisplayDateEnd = selectableDateEnd;

            }
	    }

	    #endregion

        public DateTimePicker()
        {
            InitializeComponent();

            TimePicker.SelectionChanged +=  TimePickerOnSelectionChanged;
        }

	    private void TimePickerOnSelectionChanged(object sender, SelectionChangedEventArgs selectionChangedEventArgs)
	    {
            SelectedDateTime = TimePicker.SelectedValue;
	    }

	    #region Event handlers


    


	   

	    private void RadRangeBase_OnValueChanged(object sender, RadRangeBaseValueChangedEventArgs e)
	    {
            if (TimePicker != null && TimePicker.SelectedValue != null)
                TimePicker.SelectedValue = TimePicker.SelectedValue.Value.AddMinutes((double)(e.NewValue - e.OldValue));

        }

        #endregion

    }
}
