using GazRouter.DTO.Dictionaries.EntityTypes;

namespace GazRouter.DTO.Bindings.EntityBindings
{
    public class GetEntityBindingsListParameterSet : GetPageParameterSet
    {
        public string NamePart { get; set; }

        public EntityType EntityType { get; set; }

        public SortBy SortBy { get; set; }

        public SortOrder SortOrder { get; set; }

        public int SourceId { get; set; }

        public bool ShowAll { get; set; }

    }
}