

using System;
using Telerik.Charting;
using Telerik.Windows.Controls.ChartView;

namespace GazRouter.Modes.ProcessMonitoring.Reports.Forms.GasInPipes
{
    public partial class GasInPipesView
    {
        public GasInPipesView()
        {
            InitializeComponent();
        }

        private void ChartTrackBallBehavior_OnTrackInfoUpdated(object sender, TrackBallInfoEventArgs e)
        {
            if (e == null || date == null || value == null) return;
            var closesDataPt = e.Context?.ClosestDataPoint?.DataPoint as CategoricalDataPoint;
            if (closesDataPt == null) return;

            date.Text = ((DateTime)closesDataPt.Category).ToString("dd.MM.yyyy HH:mm");
            value.Text = closesDataPt.Value.HasValue ? closesDataPt.Value.Value.ToString("0.###") : "";
        }
        
    }
}