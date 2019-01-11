using System;
using GazRouter.DTO.Dictionaries.InconsistencyTypes;

namespace GazRouter.DTO.ObjectModel.Inconsistency
{
	public class AddInconsistencyParameterSet
    {
		public InconsistencyType InconsistencyTypeId { get; set; }
		public Guid EntityId { get; set; }
    }
}
