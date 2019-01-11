using System;
using GazRouter.DTO.ObjectModel.DistrStations;
using GazRouter.Flobus.Interfaces;
using GazRouter.Flobus.UiEntities.FloModel.Measurings;
using JetBrains.Annotations;
using GazRouter.Flobus.UiEntities.FloModel;

namespace GazRouter.Flobus.VM.Model
{
    public class DistributingStation : PipelineElementOmBase<DistrStationDTO>, IDistrStation
    {
        private object _data;

        public DistributingStation(DistrStationDTO distrStationDTO, [NotNull] Pipeline connectedPipeline)
            : base(distrStationDTO, connectedPipeline)
        {
            if (connectedPipeline == null)
            {
                throw new ArgumentNullException(nameof(connectedPipeline));
            }
            ConnectedPipeline = connectedPipeline;

            DistributingStationMeasuring = new DistributingStationMeasuring(Dto);
        }

        public DistributingStationMeasuring DistributingStationMeasuring
        {
            get { return Data as DistributingStationMeasuring; }
            set { Data = value; }
        }

        public Pipeline ConnectedPipeline { get; }

        public bool HasData => _data != null;

        public override double Km => ConnectedPipeline.KmEnd;

        public object Data
        {
            get { return _data; }
            set
            {
                if (SetProperty(ref _data, value))
                {
                    OnPropertyChanged(() => HasData);
                }
            }
        }
    }
}