using GazRouter.DTO.Dictionaries.EntityTypes;
using GazRouter.DTO.Dictionaries.PipelineTypes;

namespace GazRouter.DTO.Bindings.EntityBindings
{
    public class GetEntityBindingsPageParameterSet : GetPageParameterSet
    {
        public string NamePart { get; set; }

        public EntityType EntityType { get; set; }

        public SortBy SortBy { get; set; }

        public SortOrder SortOrder { get; set; }

        public int SourceId { get; set; }

        public bool ShowAll { get; set; }
        public PipelineType? PipelineTypeId { get; set; }
    }
}