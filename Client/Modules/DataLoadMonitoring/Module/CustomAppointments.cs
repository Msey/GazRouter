using System;
using GazRouter.DTO.ObjectModel.Sites;
using GazRouter.DTO.SeriesData.Series;
using Telerik.Windows.Controls.ScheduleView;

namespace GazRouter.DataLoadMonitoring.Module
{
    /// <summary>
    /// прямоугольник статистики по ЛПУ (appointment)
    /// </summary>
    public class SiteAppointment : Appointment
    {
        private string countParams;
        public DateTime endLoadTime { get; set; }
        public SiteDTO Site { get; set; }
        public SeriesDTO KeyDate { get; set; }

        public bool IsSummary  { get; set; }

        public string CountParams
        {
            get
            {
                return countParams;
            }
            set
            {

                if (countParams != value)
                {
                    countParams = value;
                    OnPropertyChanged(() => CountParams);
                }
            }
        }
    }

    public class SummaryAppointment : Appointment
    {
        public string countParams { get; set; }
        public string countErrors { get; set; }
        public DateTime endLoadTime { get; set; }
        public SeriesDTO KeyDate { get; set; }
        
    }
}
