using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.ServiceModel;
using GazRouter.DTO.Balances.BalanceGroups;
using GazRouter.DTO.Balances.SortOrder;
using GazRouter.DTO.Balances.Values;
using GazRouter.DTO.Dictionaries.Enterprises;
using GazRouter.DTO.Dictionaries.PropertyTypes;
using GazRouter.DTO.GasCosts;
using GazRouter.DTO.ObjectModel;
using GazRouter.DTO.ObjectModel.Aggregators;
using GazRouter.DTO.ObjectModel.DistrStationOutlets;
using GazRouter.DTO.ObjectModel.DistrStations;
using GazRouter.DTO.ObjectModel.MeasLine;
using GazRouter.DTO.ObjectModel.MeasStations;
using GazRouter.DTO.ObjectModel.OperConsumers;
using GazRouter.DTO.ObjectModel.Sites;
using GazRouter.DTO.SeriesData.PropertyValues;
using GazRouter.DTO.SeriesData.Series;

namespace GazRouter.DTO.Balances.DayBalance
{
	[DataContract]
	public class DayBalanceDataDTO
	{
        [DataMember]
        public SeriesDTO Serie { get; set; }

        [DataMember]
        public List<BalanceGroupDTO> BalanceGroups { get; set; }

        [DataMember]
        public List<SiteDTO> Sites { get; set; }

        [DataMember]
        public List<EnterpriseDTO> Enterprises { get; set; }

        [DataMember]
        public List<MeasStationDTO> MeasStations { get; set; }

        [DataMember]
        public List<MeasLineDTO> MeasLines { get; set; }

        [DataMember]
        public List<DistrStationDTO> DistrStations { get; set; }

        [DataMember]
        public List<DistrStationOutletDTO> DistrStationOutlets { get; set; }

        [DataMember]
        public List<OperConsumerDTO> OperConsumers { get; set; }

        [DataMember]
        public List<AggregatorDTO> Aggregators { get; set; }


        [DataMember]
        public List<CommonEntityDTO> MiscTabEntities { get; set; }

        [DataMember]
        public List<GasCostDTO> AuxCosts { get; set; }

        [DataMember]
        public List<BalSortOrderDTO> SortOrderList { get; set; }

        [DataMember]
        public Dictionary<Guid, Dictionary<PropertyType, List<BasePropertyValueDTO>>> FactValues { get; set; }

        [DataMember]
        public List<BalanceValueDTO> PlanValues { get; set; }
    }
}