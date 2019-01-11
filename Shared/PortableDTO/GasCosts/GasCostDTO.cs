using System;
using System.Runtime.Serialization;
using GazRouter.DTO.Dictionaries.GasCostItemGroups;
using GazRouter.DTO.Dictionaries.Targets;
using GazRouter.DTO.ObjectModel;
namespace GazRouter.DTO.GasCosts
{
    public class GasCostDTO : BaseDto<int>
    {
        [DataMember]
        public CostType CostType { get; set; }
        [DataMember]
        public CommonEntityDTO Entity { get; set; }
        [DataMember]
        public DateTime Date { get; set; }
        [DataMember]
        public int ImportId { get; set; }
        [DataMember]
        public double? CalculatedVolume { get; set; }
        [DataMember]
        public double? MeasuredVolume { get; set; }

        public double Volume => MeasuredVolume ?? CalculatedVolume ?? 0;

        [DataMember]
        public string InputString { get; set; }
        [DataMember]
        public Target Target { get; set; }
        [DataMember]
        public Guid SiteId { get; set; }
        [DataMember]
        public string ChangeUserName { get; set; }
        [DataMember]
        public string ChangeUserSiteName { get; set; }
        [DataMember]
        public DateTime? ChangeDate { get; set; }

        [DataMember]
        public int SeriesId { get; set; }

        [DataMember]
        public int BalanceGroupId { get; set; }

        [DataMember]
        public GasCostItemGroup ItemGroup { get; set; }

    }
}