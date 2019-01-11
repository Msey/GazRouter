using System.Runtime.Serialization;

namespace GazRouter.DTO.Dictionaries.BoilerTypes
{
	[DataContract]
	public class BoilerTypeDTO : BaseDictionaryDTO
	{
        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public double RatedHeatingEfficiency { get; set; }

        [DataMember]
        public double RatedEfficiencyFactor { get; set; }
        
        [DataMember]
        public string GroupName { get; set; }

        [DataMember]
        public bool IsSmall { get; set; }

        [DataMember]
        public double HeatingArea { get; set; }
        

	    public override string ToString()
	    {
	        return Name;
	    }
	}
}