using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.GridView;
using Telerik.Windows.Data;

namespace GazRouter.Repair
{
    public class CustomGroupingBehavior
    {
        public static readonly DependencyProperty IsEnabledProperty = DependencyProperty.RegisterAttached("IsEnabled",
            typeof(bool),
            typeof(CustomGroupingBehavior), new PropertyMetadata(OnIsEnabledPropertyChanged));

        private readonly RadGridView _grid;

        public CustomGroupingBehavior(RadGridView grid)
        {
            _grid = grid;
        }

        public static bool GetIsEnabled(UIElement element)
        {
            return (bool) element.GetValue(IsEnabledProperty);
        }

        public static void SetIsEnabled(UIElement element, bool value)
        {
            element.SetValue(IsEnabledProperty, value);
        }

        private static void OnIsEnabledPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var grid = d as RadGridView;
            if (grid != null)
            {
                if ((bool) e.NewValue)
                {
                    var behavior = new CustomGroupingBehavior(grid);
                    behavior.Attach();
                }
            }
        }

        private void Attach()
        {
            if (_grid != null)
            {
                _grid.Grouping += Grouping;

                //  AddCustomGroupDescriptors();
            }
        }

        private void Grouping(object sender, GridViewGroupingEventArgs e)
        {
            if (e.Action == GroupingEventAction.Place)
            {
                var cgd = e.GroupDescriptor as ColumnGroupDescriptor;
                if (cgd != null && cgd.Column.UniqueName == "StartDatePlan")
                {
                    e.Cancel = true;
                }

                if (_grid.GroupDescriptors.OfType<GroupDescriptorBase>()
                    .Any(d => d.DisplayContent.ToString() == "Месяц начала"))
                {
                    return;
                }
//                AddCustomGroupDescriptors();
            }
            /* else if (e.Action == GroupingEventAction.Remove)
            {
                var gd = e.GroupDescriptor as GroupDescriptorBase;
                GroupDescriptorBase gdToRemove = null;

                if (gd != null)
                {
                    if (gd.DisplayContent == "Месяц")
                    {
                        gdToRemove = _grid.GroupDescriptors.OfType<GroupDescriptorBase>().Where()
                    }
                }
            }*/
        }

        private void AddCustomGroupDescriptors()
        {
            _grid.GroupDescriptors.Add(new GroupDescriptor<RepairItem, string, int>
            {
                GroupingExpression = i => CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(i.StartDatePlan.Month),
                GroupSortingExpression =
                    grouping => DateTime.ParseExact(grouping.Key, "MMMM", CultureInfo.CurrentCulture).Month,
                DisplayContent = "Месяц начала",
                SortDirection = ListSortDirection.Ascending
            });
        }
    }
}