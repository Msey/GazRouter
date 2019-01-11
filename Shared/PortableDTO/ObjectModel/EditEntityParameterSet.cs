using System;

namespace GazRouter.DTO.ObjectModel
{
    public class EditEntityParameterSet : BaseEntityParameterSet
    {
        public Guid Id { get; set; }
        public int SortOrder { get; set; }
    }

	public class SetSortOrderParameterSet
	{
		public Guid Id { get; set; }
		public int SortOrder { get; set; }
	}
}
