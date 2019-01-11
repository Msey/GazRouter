using System;
using System.Windows;

namespace GazRouter.Flobus.VM.Serialization
{
    public class ValveJson : PipelineElementJson
    {
        public ValveJson(Guid id, int textAngle) : base(id, textAngle, new Point(0, 0))
        {
        }
    }
}