using System.Runtime.Serialization;

namespace GazRouter.DTO.Dictionaries.AnnuledReasons
{
	[DataContract]
	public class AnnuledReasonDTO : BaseDictionaryDTO
	{

		public AnnuledReason AnnuledReason
		{
			get { return (AnnuledReason)Id; }
		}

	}
}