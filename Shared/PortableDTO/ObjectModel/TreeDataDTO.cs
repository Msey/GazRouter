using System.Collections.Generic;
using System.Runtime.Serialization;
using GazRouter.DTO.ObjectModel.BoilerPlants;
using GazRouter.DTO.ObjectModel.Boilers;
using GazRouter.DTO.ObjectModel.CompShops;
using GazRouter.DTO.ObjectModel.CompStations;
using GazRouter.DTO.ObjectModel.CompUnits;
using GazRouter.DTO.ObjectModel.Consumers;
using GazRouter.DTO.ObjectModel.CoolingStations;
using GazRouter.DTO.ObjectModel.CoolingUnit;
using GazRouter.DTO.ObjectModel.DistrStationOutlets;
using GazRouter.DTO.ObjectModel.DistrStations;
using GazRouter.DTO.ObjectModel.MeasLine;
using GazRouter.DTO.ObjectModel.MeasPoint;
using GazRouter.DTO.ObjectModel.MeasStations;
using GazRouter.DTO.ObjectModel.OperConsumers;
using GazRouter.DTO.ObjectModel.Pipelines;
using GazRouter.DTO.ObjectModel.PowerPlants;
using GazRouter.DTO.ObjectModel.PowerUnits;
using GazRouter.DTO.ObjectModel.ReducingStations;
using GazRouter.DTO.ObjectModel.Sites;
using GazRouter.DTO.ObjectModel.Valves;

namespace GazRouter.DTO.ObjectModel
{
    [DataContract]
    public class TreeDataDTO
    {
        [DataMember]
        public List<SiteDTO> Sites { get; set; }
        [DataMember]
        public List<CompStationDTO> CompStations { get; set; }
        [DataMember]
        public List<CompShopDTO> CompShops { get; set; }
        [DataMember]
        public List<CompUnitDTO> CompUnits { get; set; }
        [DataMember]
        public List<DistrStationDTO> DistrStations { get; set; }
        [DataMember]
        public List<MeasStationDTO> MeasStations { get; set; }
        [DataMember]
        public List<ReducingStationDTO> ReducingStations { get; set; }
        [DataMember]
        public List<MeasLineDTO> MeasLines { get; set; }
        [DataMember]
        public List<DistrStationOutletDTO> DistrStationOutlets { get; set; }
        [DataMember]
        public List<ConsumerDTO> Consumers { get; set; }
        [DataMember]
        public List<MeasPointDTO> MeasPoints { get; set; }
        [DataMember]
        public List<CoolingStationDTO> CoolingStations { get; set; }
		[DataMember]
		public List<CoolingUnitDTO> CoolingUnits { get; set; }
        [DataMember]
        public List<PowerPlantDTO> PowerPlants { get; set; }
        [DataMember]
        public List<PowerUnitDTO> PowerUnits { get; set; }
        [DataMember]
        public List<BoilerDTO> Boilers { get; set; }
        [DataMember]
        public List<BoilerPlantDTO> BoilerPlants { get; set; }
        [DataMember]
        public List<PipelineDTO> Pipelines { get; set; }
        [DataMember]
        public List<ValveDTO> LinearValves { get; set; }
        [DataMember]
        public List<OperConsumerDTO> OperConsumers { get; set; }


    }
}
