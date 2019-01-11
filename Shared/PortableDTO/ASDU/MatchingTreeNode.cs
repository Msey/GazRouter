namespace GazRouter.DTO.ASDU
{
    public class MatchingTreeNodeParams
    {
        public MatchingTreeNodeParams()
        {
            
        }
        public MatchingTreeNodeParams(MatchingTreeNode parentNode, MatchingTreeNode nodeToRefresh)
        {
            ParentNode = parentNode;
            NodeToRefresh = nodeToRefresh;
        }

        public MatchingTreeNodeParams(MatchingTreeNode parentNode)
        {
            ParentNode = parentNode;
            NodeToRefresh = null;
        }

        public MatchingTreeNode ParentNode { get; set; }

        public MatchingTreeNode NodeToRefresh { get; set; }
    }

    public class MatchingTreeNode
    {
        public static MatchingTreeNode AsParam(string nodeType, string type, string id) => new MatchingTreeNode
        {
            Nodetype = nodeType,
            Type = type,
            Id = id
        };
        public string Nodetype { get; set; }
        public string Type { get; set; }
        public string Id { get; set; }
        public string Name { get; set; }
        public string Linkstate { get; set; }
        public string Changestate { get; set; }
        public string Linkedobjid { get; set; }
        public string Linkedrole { get; set; }
        public string Somefile { get; set; }
        public bool CanLink { get; set; }
        public bool IsEndNode { get; set; }
        public string Value { get; set; }
    }
}