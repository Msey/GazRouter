using System.Runtime.Serialization;

namespace GazRouter.DTO.Dictionaries.ParameterTypes
{
	[DataContract]
	public class ParameterTypeDTO : BaseDictionaryDTO
	{
        public ParameterType ParameterType => (ParameterType)Id;

		[DataMember]
		public string SysName { get; set; }

	}
}