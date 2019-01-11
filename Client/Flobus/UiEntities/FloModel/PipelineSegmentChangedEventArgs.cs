using GazRouter.Flobus.Model;

namespace GazRouter.Flobus.UiEntities.FloModel
{
    /// <summary>
    /// Класс описывающий изменения сегмента газопровода для события SegmentChanged
    /// </summary>
    public class PipelineSegmentChangedEventArgs : System.EventArgs
    {
        /// <summary>
        /// Сегмент газопровода подвергшийся изменениям
        /// </summary>
        public PipelineSegment ChangedSegment { get; internal set; }

        /// <summary>
        /// Точка газопровода, которая стала причиной изменений
        /// </summary>
        public IPipelinePoint ChangedPoint { get; internal set; }

        /// <summary>
        /// Суть изменений, действия с точкой {Add, Remove, Move}
        /// </summary>
        public SegmentChangedReason Reason { get; internal set; }

/*
        public PipelineSegmentChangedEventArgs(IPipelinePoint pointBegining, IPipelinePoint pointEnd,
            IPipelinePoint changedPoint, SegmentChangedReason reason)
        {
            ChangedSegment = new PipelineSegment(pointBegining, pointEnd);
            ChangedPoint = changedPoint;
            Reason = reason;
        }
*/
    }
}