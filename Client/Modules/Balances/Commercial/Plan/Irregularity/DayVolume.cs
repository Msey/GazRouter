using System;
using GazRouter.Common.ViewModel;

namespace GazRouter.Balances.Commercial.Plan.Irregularity
{
    public class DayVolume : PropertyChangedBase
    {
        private double _volume;

        public int DayNum { get; set; }

        public double MonthVolume { get; set; }

        public double Volume
        {
            get { return _volume; }
            set
            {
                if (SetProperty(ref _volume, value))
                {
                    OnPropertyChanged(() => VolumePercent);
                    OnPropertyChanged(() => Delta);
                }
            }
        }

        public double AvgVolume { get; set; }

        public double VolumePercent => Volume/MonthVolume*200;
        public double AvgPercent => AvgVolume/MonthVolume*200;

        public double Delta => Math.Round(Volume - AvgVolume, 3);
    }
}