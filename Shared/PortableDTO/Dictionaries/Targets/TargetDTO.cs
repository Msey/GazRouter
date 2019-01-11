using System.Runtime.Serialization;

namespace GazRouter.DTO.Dictionaries.Targets
{
	[DataContract]
	public class TargetDTO : BaseDictionaryDTO
	{
		[DataMember]
		public string SysName { get; set; }

		public Target Target
		{
			get { return (Target)Id; }
		}

	}
}