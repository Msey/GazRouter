using System.Runtime.Serialization;
using GazRouter.DTO.Dictionaries.PeriodTypes;

namespace GazRouter.DTO.Calculations.Calculation
{
    [DataContract]
    public class CalculationDTO : BaseDto<int>
    {
        [DataMember]
        public string SysName { get; set; }

        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public string Expression { get; set; }

        [DataMember]
        public string Sql { get; set; }

        [DataMember]
        public bool IsInvalid { get; set; }

        [DataMember]
        public string Errm { get; set; }

        [DataMember]
        public PeriodType PerionTypeId { get; set; }

        [DataMember]
        public int SortOrder { get; set; }


        [DataMember]
        public CalculationStage CalcStage { get; set; }



    }
}
