using System;
using Telerik.Charting;
using Telerik.Windows.Controls.ChartView;

namespace GazRouter.Modes.Alarms
{
    public partial class AlarmsView
    {
        public AlarmsView()
        {
            InitializeComponent();
        }
        

        private void ChartTrackBallBehavior_OnTrackInfoUpdated(object sender, TrackBallInfoEventArgs e)
        {
            if (e == null || date == null || value == null) return;
            var closesDataPt = e.Context.ClosestDataPoint.DataPoint as CategoricalDataPoint;
            date.Text = ((DateTime)closesDataPt.Category).ToString("dd.MM.yyyy HH:mm");
            value.Text = closesDataPt.Value.Value.ToString("0.###");
        }
    }


    
}