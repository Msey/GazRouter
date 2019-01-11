using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using GazRouter.Application;
using GazRouter.Application.Helpers;
using Microsoft.Practices.Prism.Commands;
using Telerik.Windows.Controls;

namespace GazRouter.Controls
{
    public partial class PeriodPicker
    {
        public PeriodPicker()
        {
            InitializeComponent();

            var setDateCommand = new DelegateCommand<DateListEnum?>(SetDate);
            RadContextMenuMenu.ItemsSource = new List<DateMenuItem>
            {
                new DateMenuItem(setDateCommand, "За последние сутки", DateListEnum.Day),
                new DateMenuItem(setDateCommand, "За последнюю неделю", DateListEnum.Week),
                new DateMenuItem(setDateCommand, "За последний месяц", DateListEnum.Month),
                new DateMenuItem(setDateCommand, "С начала месяца", DateListEnum.BeginMonth),
                new DateMenuItem(setDateCommand, "С начала квартала", DateListEnum.BeginQuarter),
                new DateMenuItem(setDateCommand, "С начала года", DateListEnum.BeginYear)
            };
            beginDate.ClockItemsSource = endDate.ClockItemsSource = SeriesHelper.GetHours();
        }

        public PeriodDates SelectedPeriodDates
        {
            get { return (PeriodDates) GetValue(SelectedPeriodDatesProperty); }
            set { SetValue(SelectedPeriodDatesProperty, value); }
        }

        public InputMode DatePickerType
        {
            get { return (InputMode)GetValue(DatePickerTypeProperty); }
            set { SetValue(DatePickerTypeProperty, value); }
        }

        public bool IsTwoHours
        {
            get { return (bool)GetValue(IsTwoHoursProperty); }
            set { SetValue(IsTwoHoursProperty, value); }
        }

        public static readonly DependencyProperty SelectedPeriodDatesProperty =
            DependencyProperty.Register("SelectedPeriodDatesProperty", typeof(PeriodDates), typeof(PeriodPicker),
                                new PropertyMetadata(OnSelectedPeriodChanged));

        private static void OnSelectedPeriodChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (PeriodPicker)d;
            control.RefreshMainDates();
        }

        public static readonly DependencyProperty DatePickerTypeProperty =
            DependencyProperty.Register("DatePickerTypeProperty", typeof(InputMode), typeof(PeriodPicker),
                                new PropertyMetadata(InputMode.DateTimePicker, OnDatePickerTypeChanged));

        private static void OnDatePickerTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (PeriodPicker)d;
            control.beginDate.InputMode = control.endDate.InputMode = (InputMode)e.NewValue;
            control.selector.Width = (InputMode) e.NewValue != InputMode.DatePicker ? 250 : 170;
        }

        public static readonly DependencyProperty IsTwoHoursProperty =
            DependencyProperty.Register("IsTwoHoursProperty", typeof(bool), typeof(PeriodPicker),
                                new PropertyMetadata(true, IsTwoHoursChanged));

        private static void IsTwoHoursChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (PeriodPicker)d;
            if ((bool) e.NewValue)
            {
                control.DatePickerType = InputMode.DateTimePicker;
                control.beginDate.ClockItemsSource = control.endDate.ClockItemsSource = SeriesHelper.GetHours();
            }
            else
            {
                control.DatePickerType = InputMode.DatePicker;
            }
        }
        

        private void BtnSelect_OnClick(object sender, RoutedEventArgs e)
        {
            beginDate.SelectedValue = SelectedPeriodDates.BeginDate;
            endDate.SelectedValue = SelectedPeriodDates.EndDate;
            popup.IsOpen = !popup.IsOpen;
        }

        private void CloseButton_OnClick(object sender, RoutedEventArgs e)
        {
            popup.IsOpen = false;
        }

        private void ApplyButton_OnClick(object sender, RoutedEventArgs e)
        {
            SelectedPeriodDates = new PeriodDates
                                      {
                                          BeginDate = beginDate.SelectedValue,
                                          EndDate = endDate.SelectedValue
                                      };
            popup.IsOpen = false;
        }

        private void RefreshMainDates()
        {
            if(SelectedPeriodDates != null)
                txtDates.Text = string.Format("{0} - {1}",
                                          DatePickerType == InputMode.DatePicker
                                              ? SelectedPeriodDates.BeginDate.Value.ToString("dd.MM.yyyy")
                                              : SelectedPeriodDates.BeginDate.Value.ToString("dd.MM.yyyy HH:mm"),
                                          DatePickerType == InputMode.DatePicker
                                              ? SelectedPeriodDates.EndDate.Value.ToString("dd.MM.yyyy")
                                              : SelectedPeriodDates.EndDate.Value.ToString("dd.MM.yyyy HH:mm"));
        }

        private void BeginDate_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!beginDate.SelectedValue.HasValue || !endDate.SelectedValue.HasValue) 
                return;

            if (beginDate.SelectedValue > endDate.SelectedValue)
            {
                endDate.SelectedValue = beginDate.SelectedValue.Value.AddDays(1);
            }
            SetMessageCount();
        }

        private void EndDate_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!beginDate.SelectedValue.HasValue || !endDate.SelectedValue.HasValue)
                return;

            if (beginDate.SelectedValue > endDate.SelectedValue)
            {
                beginDate.SelectedValue = endDate.SelectedValue.Value.AddDays(-1);
            }
            SetMessageCount();
        }

        private void SetMessageCount()
        {
            var tmp = endDate.SelectedValue.Value - beginDate.SelectedValue.Value;
            if (tmp.Days > 0)
            {
                messageCount.Text = "Кол-во дней:";
                daysCount.Text = tmp.Days.ToString();
            }
            else if (tmp.Hours > 0)
            {
                messageCount.Text = "Кол-во часов:";
                daysCount.Text = tmp.Hours.ToString();
            }
            else
            {
                messageCount.Text = "Кол-во минут:";
                daysCount.Text = tmp.Minutes.ToString();
            }
        }

        private void SetDate(DateListEnum? entityType)
        {
            if (entityType.HasValue)
            {
                var date = DateTime.Now;
                endDate.SelectedValue = date.ToLocalTime();
                switch (entityType)
                {
                    case DateListEnum.Day:
                        beginDate.SelectedValue = date.AddDays(-1).ToLocalTime();
                        break;

                    case DateListEnum.Week:
                        beginDate.SelectedValue = date.AddDays(-7).ToLocalTime();
                        break;

                    case DateListEnum.Month:
                        beginDate.SelectedValue = date.AddMonths(-1).ToLocalTime();
                        break;

                    case DateListEnum.BeginMonth:
                        beginDate.SelectedValue = new DateTime(date.Year, date.Month, 1).ToLocalTime();
                        break;

                    case DateListEnum.BeginYear:
                        beginDate.SelectedValue = new DateTime(date.Year, 1, 1).ToLocalTime();
                        break;

                    case DateListEnum.BeginQuarter:
                        if (date.Month >= 1 && date.Month < 4)
                        {
                            beginDate.SelectedValue = new DateTime(date.Year, 1, 1);
                        }
                        else
                        {
                            if (date.Month >= 4 && date.Month < 7)
                            {
                                beginDate.SelectedValue = new DateTime(date.Year, 4, 1);
                            }
                            else
                            {
                                if (date.Month >= 7 && date.Month < 10)
                                {
                                    beginDate.SelectedValue = new DateTime(date.Year, 7, 1);
                                }
                                else
                                {
                                    beginDate.SelectedValue = new DateTime(date.Year, 10, 1);
                                }
                            }
                        }
                        break;
                }
            }
        }
    }

    public class PeriodDates
    {
        public DateTime? BeginDate { get; set; }
        public DateTime? EndDate { get; set; }

        public static PeriodDates DefaultDates()
        {
            var result = new PeriodDates();
            var date = DateTime.Now;
            if (Settings.DispatherDayStartHour % 2 == 0)
            {
                result.EndDate = date.Hour % 2 == 0 ? 
                    new DateTime(date.Year, date.Month, date.Day, date.Hour + 2, 0, 0) :
                    new DateTime(date.Year, date.Month, date.Day, date.Hour + 1, 0, 0);
            }
            else
            {
                result.EndDate = date.Hour % 2 == 0 ? 
                    new DateTime(date.Year, date.Month, date.Day, date.Hour + 1, 0, 0) :
                    new DateTime(date.Year, date.Month, date.Day, date.Hour + 2, 0, 0);
            }
            result.BeginDate = result.EndDate.Value.AddDays(-1);
            return result;
        }
    }

    public class DateMenuItem
    {
        public DateMenuItem(ICommand command, string header, DateListEnum type)
        {
            Type = type;
            Header = header;
            Command = command;
        }

        public DateListEnum Type { get; private set; }
        public string Header { get; private set; }
        public ICommand Command { get; private set; }
    }

    public enum DateListEnum
    {
        Day,
        Week,
        Month,
        BeginMonth,
        BeginQuarter,
        BeginYear
    }
}
