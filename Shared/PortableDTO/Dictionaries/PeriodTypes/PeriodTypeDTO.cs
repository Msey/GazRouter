using System.Runtime.Serialization;

namespace GazRouter.DTO.Dictionaries.PeriodTypes
{
	[DataContract]
	public class PeriodTypeDTO : BaseDictionaryDTO
	{
		[DataMember]
		public string SysName { get; set; }

		public PeriodType PeriodType
		{
			get { return (PeriodType)Id; }
		}

	}
}