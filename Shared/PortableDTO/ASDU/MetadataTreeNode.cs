namespace GazRouter.DTO.ASDU
{
    public enum MetadataTreeNodeType
    {
        IusObj = 1,
        IusAttr = 2,
        IusAttrLink = 20,
        IusParam = 3,
        AsduObj = 101,
        AsduAttr = 102,
        AsduAttrLink = 120,
        AsduParam = 103
    }
    
    public class MetadataTreeNode
    {
        public MetadataTreeNodeType Nodetype { get; set; }
        public string Id { get; set; }
        public string ParentId { get; set; }
        public string Name { get; set; }
        public string LinkedId { get; set; }
        public int ErrorCount { get; set; }
        public string MiscInfo { get; set; }
        public string RawId { get; set; }
        public string RawType { get; set; }
    }
}