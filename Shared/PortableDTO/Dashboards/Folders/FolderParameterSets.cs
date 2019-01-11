namespace GazRouter.DTO.Dashboards.Folders
{
	public class EditFolderParameterSet 
	{
        public int FolderId { get; set; }
        public string Name { get; set; }
        public int? ParentId { get; set; }
        public int? SortOrder { get; set; }
    }

	public class AddFolderParameterSet : FolderParameterSet
    {
		public int SortOrder { get; set; }
    }

	public class FolderParameterSet
	{
		public string Name { get; set; }
		public int? ParentId { get; set; }
	}

	public class SetSortOrderParameterSet
	{
		public int Id { get; set; }
		public int? FolderId { get; set; }
		public int? SortOrder { get; set; }
		public bool IsFolder { get; set; }
	}
}