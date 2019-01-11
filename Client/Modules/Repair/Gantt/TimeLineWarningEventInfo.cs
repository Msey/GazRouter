using Telerik.Windows.Controls.Scheduling;
using Telerik.Windows.Core;

namespace GazRouter.Repair.Gantt
{
    public class TimeLineWarningEventInfo : SlotInfo
    {
        private readonly string _warning;

        public TimeLineWarningEventInfo(Range<long> timeRange, int index, string warning)
            : base(timeRange, index, index)
        {
            _warning = warning;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as TimeLineWarningEventInfo);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        /// Цвет линии
        /// </summary>
        public string Warning
        {
            get { return _warning; }
        }
    }


}
