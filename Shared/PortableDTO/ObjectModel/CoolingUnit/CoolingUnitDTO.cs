
using System.Runtime.Serialization;
using GazRouter.DTO.Dictionaries.EntityTypes;

namespace GazRouter.DTO.ObjectModel.CoolingUnit
{
	[DataContract]
	public class CoolingUnitDTO : EntityDTO
	{
		[DataMember]
		public int CoolingUnitTypeId { get; set; }

		public override EntityType EntityType
		{
			get { return EntityType.CoolingUnit; }
		}
	}
}
