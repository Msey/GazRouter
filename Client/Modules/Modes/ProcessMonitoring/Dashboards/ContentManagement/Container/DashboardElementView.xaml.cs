using System;
using System.Windows;
using System.Windows.Controls;
namespace GazRouter.Modes.ProcessMonitoring.Dashboards.ContentManagement.Container
{
    public partial class DashboardElementView : UserControl
    {
        public DashboardElementView()
        {
            InitializeComponent();
            _timer = new System.Windows.Threading.DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };            
        }

        private System.Windows.Threading.DispatcherTimer _timer;
        private void GridLoaded(object sender, RoutedEventArgs e)
        {
            _timer.Tick += TimerTick;
            _timer.Start();            
        }

        private void GridUnloaded(object sender, RoutedEventArgs e)
        {
            _timer.Tick -= TimerTick;
            _timer.Stop();
        }

        private void TimerTick(object sender, EventArgs e)
        {
            ((DashboardViewModel)DataContext)?.OnDimensionChanged();
        }
    }
}
