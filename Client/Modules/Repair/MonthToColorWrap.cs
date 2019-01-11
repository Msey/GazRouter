using System;
using System.Windows.Media;
using GazRouter.Common.ViewModel;

namespace GazRouter.Repair
{
    public class MonthToColorWrap : PropertyChangedBase
    {
        public MonthToColorWrap(MonthToColor mtc)
        {
            MonthToColor = mtc;
        }

        public MonthToColor MonthToColor { get; set; }

        public Color Color
        {
            get { return MonthToColor.Color; }
            set
            {
                MonthToColor.Color = value;
                OnPropertyChanged(() => Color);
            }
        }

        public string MonthName => new DateTime(2016, MonthToColor.Month, 1).ToString("MMMM");
    }
}