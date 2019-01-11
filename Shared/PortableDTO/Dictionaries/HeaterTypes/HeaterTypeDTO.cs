using System.Runtime.Serialization;

namespace GazRouter.DTO.Dictionaries.HeaterTypes
{
	[DataContract]
	public class HeaterTypeDTO : BaseDictionaryDTO
	{
        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public double GasConsumptionRate { get; set; }

        [DataMember]
        public double? EffeciencyFactorRated { get; set; }

	    public override string ToString()
	    {
	        return Name;
	    }
	}
}