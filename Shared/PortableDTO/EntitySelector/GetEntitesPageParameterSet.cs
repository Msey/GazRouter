using System;
using System.Collections.Generic;
using GazRouter.DTO.Dictionaries.EntityTypes;
using GazRouter.DTO.Dictionaries.PipelineTypes;

namespace GazRouter.DTO.EntitySelector
{
    public class GetEntitesPageParameterSet : GetPageParameterSet
    {
        public GetEntitesPageParameterSet()
        {
            EntityTypes = new List<EntityType>();
        }

        public string NamePart { get; set; }
        public Guid Token { get; set; }
        public List<EntityType> EntityTypes { get; set; }
        public PipelineType? PipeLineType { get; set; }
        public Guid? SiteId { get; set; }
        public Guid? EnterpriseId { get; set; }

        public bool OnlyCurrentEnterpriseEntities { get; set; }
    }
}
