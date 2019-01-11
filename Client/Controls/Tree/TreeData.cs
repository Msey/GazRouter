using System.Collections.Generic;
using GazRouter.DTO.Dictionaries.Enterprises;
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

namespace GazRouter.Controls.Tree
{
    public class TreeData
    {
        public TreeData()
        {
            Sites = new List<SiteDTO>();
            CompStations = new List<CompStationDTO>();
            CompShops = new List<CompShopDTO>();
            CompUnits = new List<CompUnitDTO>();
            DistrStations = new List<DistrStationDTO>();
            ReducingStations = new List<ReducingStationDTO>();
            BoilerPlants = new List<BoilerPlantDTO>();
            Boilers = new List<BoilerDTO>();
            Consumers = new List<ConsumerDTO>();
            CoolingStations = new List<CoolingStationDTO>();
            CoolingUnits = new List<CoolingUnitDTO>();
            DistrStationOutlets = new List<DistrStationOutletDTO>();
            PowerPlants = new List<PowerPlantDTO>();
            PowerUnits = new List<PowerUnitDTO>();
            MeasPoints = new List<MeasPointDTO>();
            MeasStations = new List<MeasStationDTO>();
            MeasLines = new List<MeasLineDTO>();
            OperConsumers = new List<OperConsumerDTO>();
        }

        public List<EnterpriseDTO> Enterprises { get; set; }
        public List<SiteDTO> Sites { get; set; }
        public List<CompStationDTO> CompStations { get; set; }
        public List<CompShopDTO> CompShops { get; set; }
        public List<CompUnitDTO> CompUnits { get; set; }
        public List<DistrStationDTO> DistrStations { get; set; }
        public List<MeasStationDTO> MeasStations { get; set; }
        public List<ReducingStationDTO> ReducingStations { get; set; }
        public List<MeasLineDTO> MeasLines { get; set; }
        public List<DistrStationOutletDTO> DistrStationOutlets { get; set; }
        public List<ConsumerDTO> Consumers { get; set; }
        public List<MeasPointDTO> MeasPoints { get; set; }
        public List<CoolingStationDTO> CoolingStations { get; set; }
		public List<CoolingUnitDTO> CoolingUnits { get; set; }
        public List<PowerPlantDTO> PowerPlants { get; set; }
        public List<PowerUnitDTO> PowerUnits { get; set; }
        public List<BoilerDTO> Boilers { get; set; }
        public List<BoilerPlantDTO> BoilerPlants { get; set; }
        public List<PipelineDTO> Pipelines { get; set; }
        public List<ValveDTO> LinearValves { get; set; }
        public List<OperConsumerDTO> OperConsumers { get; set; } 
    }
}