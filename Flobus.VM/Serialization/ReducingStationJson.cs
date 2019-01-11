using GazRouter.Flobus.VM.Model;
using System;
using System.Windows;

namespace GazRouter.Flobus.VM.Serialization
{
    public class ReducingStationJson : PipelineElementJson
    {
        public ReducingStationJson()
        {
        }
        public ReducingStationJson(Guid id, int textAngle, Point container, bool hidden = false) : base(id, textAngle,container, hidden)
        {
        }

        public ReducingStationJson(ReducingStation m) : this(m.Id, m.TextAngle, m.ContainerPosition, m.Hidden)
        { }
    }
}