using System;
using GazRouter.DTO.Dictionaries.EntityTypes;

namespace GazRouter.DTO.ObjectModel
{
	public class DeleteEntityParameterSet
	{
		public Guid Id { get; set; }
		public EntityType EntityType { get; set; }
	}
}
