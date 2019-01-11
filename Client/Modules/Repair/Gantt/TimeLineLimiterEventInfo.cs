using System.Windows.Media;
using Telerik.Windows.Controls.Scheduling;
using Telerik.Windows.Core;

namespace GazRouter.Repair.Gantt
{
    public class TimeLineLimiterEventInfo : SlotInfo
    {
        private readonly Color _color;

        public TimeLineLimiterEventInfo(Range<long> timeRange, int index, Color color)
            : base(timeRange, index, index)
        {
            _color = color;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as TimeLineLimiterEventInfo);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        /// Цвет линии
        /// </summary>
        public Color LineColor
        {
            get { return _color; }
        }
    }


}
