using System.Runtime.Serialization;

namespace GazRouter.DTO.Dictionaries.RegulatorTypes
{
    [DataContract]
    public class RegulatorTypeDTO : BaseDictionaryDTO
    {
		[DataMember]
		public string Description { get; set; }

		[DataMember]
		public int SortOrder { get; set; }

		[DataMember]
		public double GasConsumptionRate { get; set; }

        //TODO: проверить, используется ли. Что делать при добавлении типа?
        public RegulatorType RegulatorType
        {
            get { return (RegulatorType)Id; }
        }
        
    }
}
