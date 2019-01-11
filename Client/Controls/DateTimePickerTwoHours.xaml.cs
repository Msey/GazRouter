using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using GazRouter.Application;
using GazRouter.Application.Helpers;
using Telerik.Windows.Controls;

namespace GazRouter.Controls
{
	public partial class DateTimePickerTwoHours
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
            typeof(DateTimePickerTwoHours),
            new PropertyMetadata(null, SelectedDateTimeChanged));

        private static void SelectedDateTimeChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var c = sender as DateTimePickerTwoHours;

            if (c != null)
            {
                c.DatePicker.SelectedValue = (DateTime?)e.NewValue;
            }
        }

        #endregion

        #region DisableSelectFuture dependency property

	    public static readonly DependencyProperty DisableSelectFutureProperty =
            DependencyProperty.Register("DisableSelectFuture", typeof(bool), typeof(DateTimePickerTwoHours), new PropertyMetadata(false, DisableSelectFutureChanged));

	    public bool DisableSelectFuture
	    {
	        get { return (bool) GetValue(DisableSelectFutureProperty); }
	        set { SetValue(DisableSelectFutureProperty, value); }
	    }

     

	    private static void DisableSelectFutureChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	    {
            var c = d as DateTimePickerTwoHours;

            if (c != null)
            {
                bool disable = e.NewValue is bool && (bool) e.NewValue;

                var selectableDateEnd = disable ? DateTime.Now : (DateTime?) null;
                c.DatePicker.SelectableDateEnd = selectableDateEnd;
                c.DatePicker.DisplayDateEnd = selectableDateEnd;
                c.DatePicker.SelectableDateEnd = selectableDateEnd;
                c.DatePicker.DisplayDateEnd = selectableDateEnd;

            }
	    }

	    #endregion

        #region VisibilityRadUpDown dependency property

        public Visibility VisibilityRadUpDown
        {
            get
            {
                return (Visibility)GetValue(VisibilityRadUpDownProperty);
            }
            set
            {
                SetValue(VisibilityRadUpDownProperty, value);
            }
        }

        public static readonly DependencyProperty VisibilityRadUpDownProperty =
            DependencyProperty.Register("VisibilityRadUpDown",
            typeof(Visibility?),
            typeof(DateTimePickerTwoHours),
            new PropertyMetadata(Visibility.Visible, VisibilityRadUpDownChanged));

        private static void VisibilityRadUpDownChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var c = sender as DateTimePickerTwoHours;

            if (c != null)
            {
                c.UpDown.Visibility = (Visibility)e.NewValue;
            }
        }

        #endregion

        public DateTimePickerTwoHours()
        {
            InitializeComponent();
            DatePicker.ClockItemsSource = SeriesHelper.GetHours();
            DatePicker.SelectionChanged += DatePicker_SelectionChanged;
        }

        void DatePicker_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectedDateTime = DatePicker.SelectedValue;
        }

	    public static DateTime NowDateTime => SeriesHelper.GetCurrentSession();

	    private void RadRangeBase_OnValueChanged(object sender, RadRangeBaseValueChangedEventArgs e)
        {
            if (DatePicker != null && DatePicker.SelectedValue != null)
                //Ограниение выбора будущей даты
                /*
                if (DatePicker.SelectedValue.Value.AddHours((double)((e.NewValue - e.OldValue) * 2)) <= DateTime.Now)
                    DatePicker.SelectedValue = DatePicker.SelectedValue.Value.AddHours((double)((e.NewValue - e.OldValue) * 2));
                else
                    return;*/
            DatePicker.SelectedValue = DatePicker.SelectedValue.Value.AddHours((double)((e.NewValue - e.OldValue) * 2));


        }

    }
}
