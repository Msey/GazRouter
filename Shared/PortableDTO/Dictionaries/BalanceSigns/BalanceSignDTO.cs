using System.Runtime.Serialization;

namespace GazRouter.DTO.Dictionaries.BalanceSigns
{
	[DataContract]
	public class BalanceSignDTO : BaseDictionaryDTO
	{

		public Sign BalanceSign
		{
			get { return (Sign)Id; }
		}

	}
}