using System.Runtime.Serialization;

namespace GazRouter.DTO.Dictionaries.ValvePurposes
{
	[DataContract]
	public class ValvePurposeDTO : BaseDictionaryDTO
	{
        [DataMember]
        public string Description { get; set; }

        [DataMember]
		public int SortOrder { get; set; }

		public ValvePurpose ValvePurpose
	    {
			get { return (ValvePurpose)Id; }
	    }
        
	}
}