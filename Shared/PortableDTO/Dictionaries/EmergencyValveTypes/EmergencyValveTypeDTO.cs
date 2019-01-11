using System.Runtime.Serialization;

namespace GazRouter.DTO.Dictionaries.EmergencyValveTypes
{
    [DataContract]
    public class EmergencyValveTypeDTO : BaseDictionaryDTO
    {
		[DataMember]
		public string Description { get; set; }

		[DataMember]
		public int SortOrder { get; set; }


        public EmergencyValveType EmergencyValveType
        {
            get { return (EmergencyValveType) Id; }
        }

        [DataMember]
        public double InnerDiameter { get; set; }
    }
}
