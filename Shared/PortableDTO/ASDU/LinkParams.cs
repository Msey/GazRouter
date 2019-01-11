namespace GazRouter.DTO.ASDU
{
    public enum LinkAction
    {
        UnLinkObject = 0,
        LinkObject = 1,
        UnLinkMetaEntity = 100,
        LinkMetaEntity = 101,
        UnLinkMetaAttr = 110,
        LinkMetaAttr = 111,
        UnLinkMetaParam = 120,
        LinkMetaParam = 121,
    }

    public class LinkParams
    {
        public string IusType { get; set; }
        public string IusId { get; set; }
        public string AsduType { get; set; }
        public string AsduId { get; set; }
        //public string LinkRole { get; set; }
        public LinkAction LinkAction { get; set; }
    }
}