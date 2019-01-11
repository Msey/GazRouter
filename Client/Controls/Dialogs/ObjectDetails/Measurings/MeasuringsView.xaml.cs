using System;
using System.Windows;
using System.Windows.Controls;
using Telerik.Charting;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.ChartView;

namespace GazRouter.Controls.Dialogs.ObjectDetails.Measurings
{
    public partial class MeasuringsView
    {
        public MeasuringsView()
        {
            InitializeComponent();
        }

        private double _oldHeight = 250;
        private void OnShowTrendClick(object sender, RoutedEventArgs e)
        {
            var box = (CheckBox)sender;
            if (!box.IsChecked.HasValue || !box.IsChecked.Value)
            {
                _oldHeight = TheGrid.RowDefinitions[2].Height.Value;
                TheGrid.RowDefinitions[2].Height = new GridLength(0);
                TheGrid.RowDefinitions[2].MinHeight = 0;
            }
            else TheGrid.RowDefinitions[2].Height = new GridLength(_oldHeight);
        }

        private void ChartTrackBallBehavior_OnTrackInfoUpdated(object sender, TrackBallInfoEventArgs e)
        {
            if (e == null || date == null || value == null) return;
            date.Text = "";
            value.Text = "";
            var closesDataPt = e.Context?.ClosestDataPoint?.DataPoint as CategoricalDataPoint;
            if (closesDataPt != null)
            {
                var timestamp = closesDataPt.Category as DateTime?;
                date.Text = timestamp.HasValue ? timestamp.Value.ToString("dd.MM.yyyy HH:mm") : "";
                value.Text = closesDataPt.Value.HasValue ? closesDataPt.Value.Value.ToString("0.###") : "";
            }
        }
    }
}