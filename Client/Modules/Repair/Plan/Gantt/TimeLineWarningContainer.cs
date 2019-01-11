using System.Windows;
using System.Windows.Controls;
using Telerik.Windows.Rendering;

namespace GazRouter.Repair.Plan.Gantt
{
    public class TimeLineWarningContainer: Control, IDataContainer
    {
        public static readonly DependencyProperty WarningProperty = DependencyProperty.Register("Warning", typeof (string),
            typeof (TimeLineWarningContainer), new PropertyMetadata(string.Empty));

        private object _data;

        public TimeLineWarningContainer()
        {
            DefaultStyleKey = typeof(TimeLineWarningContainer);
        }

        public string Warning
        {
            get { return (string)GetValue(WarningProperty); }
            set { SetValue(WarningProperty, value); }
        }

        public object DataItem
        {
            get { return _data; }
            set
            {
                if (_data != value)
                {
                    _data = value;
                    var info = (TimeLineWarningEventInfo) value;
                    if (info != null)
                        Warning = info.Warning;
                }
            }
        }
    }


}
