using System;
using System.Windows.Controls;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.ScheduleView;

namespace GazRouter.DataLoadMonitoring.Views
{
    public partial class LoadMonitorView
    {
        public LoadMonitorView()
        {
            InitializeComponent();
           // this.DataContext = new LoadMonViewModel(SeriesType.oddSeries);
           // this.SetTimeLineView();
        }

        void SetTimeLineView()
        {
            var timeLineView = new TimelineViewDefinition
            {
                Orientation = Orientation.Vertical,
                MinorTickLength = new FixedTickProvider(new DateTimeInterval(2, 0, 0, 0)),
                MajorTickLength = new FixedTickProvider(new DateTimeInterval(2, 0, 0, 0)),
                //GroupTickLength = new FixedTickProvider(new DateTimeInterval(2, 0, 0)),
                DayStartTime = new TimeSpan(9,0,0),
                DayEndTime = new TimeSpan(11, 0, 0),
                VisibleDays = 2
            };
            DataMonitor.ViewDefinitions.Add(timeLineView);
        }

        /// <summary>
        /// блокировка окна редактирования элемента Appointments
        /// </summary>
        private void DataMonitor_OnShowDialog(object sender, ShowDialogEventArgs e)
        {
            if (e.DialogViewModel is AppointmentDialogViewModel)
                e.Cancel = true;
        }

    }
}
