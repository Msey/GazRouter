﻿using System.Runtime.Serialization;

// Типы ремонтных работ 
namespace GazRouter.DTO.Dictionaries.RepairWorksType
{
    [DataContract]
	public class RepairWorkTypeDTO : BaseDto<int>
    {
		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public string SystemName { get; set; }

		[DataMember]
		public string Description { get; set; }

		[DataMember]
		public int SortOrder { get; set; }
        
    }
}
