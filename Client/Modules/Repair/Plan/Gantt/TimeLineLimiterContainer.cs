using System.Windows.Controls;
using System.Windows.Media;
using Telerik.Windows.Rendering;

namespace GazRouter.Repair.Plan.Gantt
{
    public class TimeLineLimiterContainer: Control, IDataContainer
    {
        private object _data;

        public TimeLineLimiterContainer()
        {
            DefaultStyleKey = typeof (TimeLineLimiterContainer);
        }

        public object DataItem
        {
            get { return _data; }
            set
            {
                if (_data != value)
                {
                    _data = value;
                    var info = (TimeLineLimiterEventInfo) value;
                    if (info != null)
                        Foreground = new SolidColorBrush(info.LineColor);
                }
            }
        }
    }


}
