using System.Runtime.Serialization;

namespace GazRouter.DTO.Dictionaries.PowerUnitTypes
{
	[DataContract]
	public class PowerUnitTypeDTO : BaseDictionaryDTO
	{
        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public double RatedPower { get; set; }

        [DataMember]
        public double FuelConsumptionRate { get; set; }

        [DataMember]
        public EngineGroup EngineGroup { get; set; }

        [DataMember]
        public string EngineGroupName { get; set; }

        [DataMember]
        public string EngineTypeName { get; set; }
        

	    public override string ToString()
	    {
	        return Name;
	    }
        
	}
}