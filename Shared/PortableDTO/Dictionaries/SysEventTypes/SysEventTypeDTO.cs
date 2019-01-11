using System.Runtime.Serialization;

namespace GazRouter.DTO.Dictionaries.SysEventTypes
{
    [DataContract]
    public class SysEventTypeDTO : BaseDictionaryDTO
    {
        public string Description { get; set; }

        public string SysName { get; set; }
    }
}