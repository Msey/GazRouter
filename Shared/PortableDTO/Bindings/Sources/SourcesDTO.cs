using System.Runtime.Serialization;

namespace GazRouter.DTO.Bindings.Sources
{
    /// <summary>
    /// внешние системы (таблица sources)
    /// </summary>
    [DataContract]
    public class Source
    {        
        [DataMember]
        public int SourceId { get; set; }
        
        [DataMember]
        public string SourceName { get; set; }

        [DataMember]
        public string SystemName { get; set; }

        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public bool IsHidden { get; set; }

        [DataMember]
        public bool IsReadonly { get; set; }
        
    }
}
