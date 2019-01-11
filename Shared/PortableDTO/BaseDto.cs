using System.Runtime.Serialization;

namespace GazRouter.DTO
{
    [DataContract]
    public abstract class BaseDto<TId> : IListItem<TId>
    {
        [DataMember]
        public TId Id { get; set; }
    }
}