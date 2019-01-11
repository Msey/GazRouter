using GazRouter.Flobus.VM.Model;
using System;
using System.Windows;

namespace GazRouter.Flobus.VM.Serialization
{
    public class MeasuringLineJson : PipelineElementJson
    {
        public MeasuringLineJson()
        {
        }
        public MeasuringLineJson(Guid id, int textAngle, Point container, bool hidden = false) : base(id, textAngle,container, hidden)
        {
        }

        public MeasuringLineJson(MeasuringLine m) : this(m.Id, m.TextAngle,m.ContainerPosition, m.Hidden)
        { }
    }
}