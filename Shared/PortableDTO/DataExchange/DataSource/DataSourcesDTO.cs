using System.Runtime.Serialization;

namespace GazRouter.DTO.DataExchange.DataSource
{
    [DataContract]
    public class DataSourceDTO : BaseDto<int>
    {        
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string SysName { get; set; }

        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public bool IsHidden { get; set; }

        [DataMember]
        public bool IsReadonly { get; set; }
        
    }
}
