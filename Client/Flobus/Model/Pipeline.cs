using System;
using System.Collections.Generic;
using System.Windows;
using GazRouter.Flobus.UiEntities.FloModel;

namespace GazRouter.Flobus.Model
{
    [Obsolete]
    public interface IObsoletePipeline
    {
        void NotifyPipelineSegmentChanged(IPipelinePoint ptBegining, IPipelinePoint ptEnd, IPipelinePoint point, SegmentChangedReason remove);
        PipelineSegment FindSegment(double km);
        PipelineSegment FindSegment(Point pt);
        double KmBegining { get; }
        double KmEnd { get; }
        IEnumerable<IPipelinePoint> IntermediatePoints { get; }
        IPipelinePoint Start { get; }
        IPipelinePoint End { get;  }
        double MinAllowedKm(IPipelinePoint pipelinePoint);
        double MaxAllowedKm(IPipelinePoint pipelinePoint);
    }
}