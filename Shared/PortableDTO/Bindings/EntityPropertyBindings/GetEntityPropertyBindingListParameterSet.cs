using System;
using System.Collections.Generic;
using GazRouter.DTO.Bindings.EntityBindings;
using GazRouter.DTO.Dictionaries.PeriodTypes;
using GazRouter.DTO.Dictionaries.Targets;

namespace GazRouter.DTO.Bindings.EntityPropertyBindings
{
    public class GetEntityPropertyBindingListParameterSet 
    {
        public GetEntityPropertyBindingListParameterSet()
        {
            PeriodTypeId = PeriodType.Twohours;
        }

        public int SourceId { get; set; }

        public string NamePart { get; set; }

//		public EntityTypeEnum EntityType { get; set; }

        //public Guid CompUnitId { get; set; }

        public SortBy SortBy { get; set; }

        public SortOrder SortOrder { get; set; }
        
        public PeriodType PeriodTypeId { get; set; }

    }
}