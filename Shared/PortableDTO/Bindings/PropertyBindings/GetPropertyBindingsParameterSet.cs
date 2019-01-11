using GazRouter.DTO.Bindings.EntityBindings;

namespace GazRouter.DTO.Bindings.PropertyBindings
{
    public class GetPropertyBindingsParameterSet 
	{
		public string NamePart { get; set; }

		public SortBy SortBy { get; set; }

		public SortOrder SortOrder { get; set; }

		public int SourceId { get; set; }

		public bool ShowAll { get; set; }

	}
}
