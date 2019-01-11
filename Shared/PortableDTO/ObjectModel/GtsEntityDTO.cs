using System.Runtime.Serialization;

namespace GazRouter.DTO.ObjectModel
{
    [DataContract]
    public class GtsEntityDTO : CommonEntityDTO
    {
        [DataMember]
        public int SystemId { get; set; }
    }
}
