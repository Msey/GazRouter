using System.Runtime.Serialization;
using System.Windows.Media;

namespace GazRouter.Repair
{
    [DataContract]
    public class MonthToColor
    {
        public MonthToColor(int month, Color color)
        {
            Month = month;
            Color = color;
        }

        [DataMember]
        public int Month { get; set; }

        [DataMember]
        public Color Color { get; set; }
    }
}