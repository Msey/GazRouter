using System;
using GazRouter.DTO.Bindings.EntityBindings;
using GazRouter.DTO.Dictionaries.PeriodTypes;

namespace GazRouter.DTO.Bindings.EntityPropertyBindings
{
    public class GetEntityPropertyBindingsParameterSet : GetPageParameterSet
	{
		public int SourceId { get; set; }

		public string NamePart { get; set; }

        public Guid EntityId { get; set; }

		public SortBy SortBy { get; set; }

		public SortOrder SortOrder { get; set; }

        public PeriodType PeriodTypeId { get; set; }
	}
}
