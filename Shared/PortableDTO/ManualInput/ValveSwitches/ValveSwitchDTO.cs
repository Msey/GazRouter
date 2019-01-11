using System;
using System.Runtime.Serialization;
using GazRouter.DTO.Dictionaries.PipelineTypes;
using GazRouter.DTO.Dictionaries.StatesModel;

namespace GazRouter.DTO.ManualInput.ValveSwitches
{
    [DataContract]
    public class ValveSwitchDTO : BaseDto<Guid>
    {
		[DataMember]
        public DateTime SwitchingDate { get; set; }

        [DataMember]
        public ValveSwitchType SwitchType { get; set; }

        [DataMember]
        public ValveState State { get; set; }

        [DataMember]
        public int? OpenPercent { get; set; }


        [DataMember]
        public string ValveName { get; set; }
		
        [DataMember]
        public double Kilometr { get; set; }

        [DataMember]
        public int ValveTypeId { get; set; }

        [DataMember]
        public int Bypass1TypeId { get; set; }

        [DataMember]
        public int Bypass2TypeId { get; set; }

        [DataMember]
        public int Bypass3TypeId { get; set; }
        
        [DataMember]
        public string ValvePurposeName { get; set; }
		
        [DataMember]
        public Guid CompStationId { get; set; }

        [DataMember]
        public string CompStationName { get; set; }

        [DataMember]
        public Guid? CompShopId { get; set; }
        
        [DataMember]
        public string CompShopName { get; set; }
        

        [DataMember]
        public Guid SiteId { get; set; }
        
        [DataMember]
        public string SiteName { get; set; }


        [DataMember]
        public Guid PipelineId { get; set; }

        [DataMember]
        public string PipelineName { get; set; }
        
        [DataMember]
        public PipelineType PipelineType { get; set; }


        [DataMember]
        public string ChangeUserName { get; set; }


        [DataMember]
        public string ChangeUserSite { get; set; }


    }
}
