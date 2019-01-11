using System.Runtime.Serialization;

namespace GazRouter.DTO.Dictionaries.EventTypes
{
    [DataContract]
    public class EventTypeDTO : BaseDictionaryDTO
    {
        public string Description { get; set; }
    }
}