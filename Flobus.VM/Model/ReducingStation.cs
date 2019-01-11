using System;
using GazRouter.DTO.ObjectModel.ReducingStations;
using GazRouter.Flobus.UiEntities.FloModel.Measurings;
using JetBrains.Annotations;

namespace GazRouter.Flobus.VM.Model
{
    public class ReducingStation : PipelineElementOmBase<ReducingStationDTO>
    {
        public ReducingStation(ReducingStationDTO reducingStationDto, [NotNull] Pipeline pipe)
            : base(reducingStationDto, pipe)
        {
            if (pipe == null)
            {
                throw new ArgumentNullException(nameof(pipe));
            }
            Pipe = pipe;
            ReducingStationMeasuring = new ReducingStationMeasuring(Dto);
        }

        /// <summary>
        ///     Километр установки регулятора давления
        /// </summary>
        public override double Km => Dto.Kilometer;

        public ReducingStationMeasuring ReducingStationMeasuring { get; }

    }
}