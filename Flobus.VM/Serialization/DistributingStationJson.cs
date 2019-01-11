using System;
using System.Windows;

namespace GazRouter.Flobus.VM.Serialization
{
    public class DistributingStationJson : PipelineElementJson
    {
        //public DistributingStationJson(Guid id, int textAngle) : base(id, textAngle)
        //{
        //}

        public DistributingStationJson()
        {
        }
        public DistributingStationJson(Guid id, int textAngle, Point container, bool hidden = false) : base(id, textAngle, container, hidden)
        {
        }

        public DistributingStationJson(DistributingStationJson ds) : this(ds.Id, ds.TextAngle, ds.ContainerPosition, ds.Hidden)
        { }
        
    }
}