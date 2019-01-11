using System;
using GazRouter.DTO.Dictionaries.PeriodTypes;
using GazRouter.DTO.Dictionaries.PropertyTypes;

namespace GazRouter.DAL.Bindings.EntityPropertyBindings
{
    public class GetEntityPropertyBindingParameterSetBase
    {
        public int SourceId { get; set; }
    }

	public class GetEntityPropertyBindingParameterSet : GetEntityPropertyBindingParameterSetBase
	{
		public string ExtKey { get; set; }
	}

	public class GetEntityPropertyBindingSourceParameterSet : GetEntityPropertyBindingParameterSetBase
	{
		public Guid EntityId { get; set; }
		public PeriodType PeriodTypeId { get; set; }
		public PropertyType PropertyTypeId { get; set; }
	}
}
