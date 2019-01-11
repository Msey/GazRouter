using System;
using GazRouter.DTO.ObjectModel.MeasLine;
using GazRouter.Flobus.UiEntities.FloModel.Measurings;
using JetBrains.Annotations;

namespace GazRouter.Flobus.VM.Model
{
    public class MeasuringLine : PipelineElementOmBase<MeasLineDTO>
    {
        public MeasuringLine(MeasLineDTO measLineDto, [NotNull] Pipeline pipe) : base(measLineDto, pipe)
        {
            if (pipe == null)
            {
                throw new ArgumentNullException(nameof(pipe));
            }
            Pipe = pipe;

            MeasuringLineMeasuring = new MeasuringLineMeasuring(Dto);
        }

        public MeasuringLineMeasuring MeasuringLineMeasuring { get; }

        public override double Km => Dto.KmOfConn;

    }
}