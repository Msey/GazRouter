namespace GazRouter.DTO.ASDU
{
    public enum AsduRequestKind
    {
        IusEntityCreate = 1,
        IusEntityCreateAll = 2,
        IusEntityUpdateName = 10,
        IusEntityUpdateDesc = 11,
        IusEntityUpdateAll = 12,
        //IusAttrCreate,
        IusAttrUpdateValue = 20,
        IusParamCreate = 30,
        AsduEntityDelete = 40,
        // To be continued
    }

    public class AsduRequestParams
    {
        public string RequestKey { get; set; }

        public AsduRequestKind RequestKind { get; set; }
        public string EntityId { get; set; }

        public string Type { get; set; }
        public string Id { get; set; }
    }
}