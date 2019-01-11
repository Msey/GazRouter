using System.Runtime.Serialization;

namespace GazRouter.DTO.Dictionaries.CoolingUnitTypes
{
	[DataContract]
	public class CoolingUnitTypeDTO : BaseDictionaryDTO
	{
        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public double RatedPower { get; set; }

        [DataMember]
        public double FuelConsumptionRate { get; set; }
        

	    public override string ToString()
	    {
	        return Name;
	    }
        
	}
}