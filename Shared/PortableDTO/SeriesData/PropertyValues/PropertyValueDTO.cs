using System.Runtime.Serialization;

namespace GazRouter.DTO.SeriesData.PropertyValues
{
    [DataContract]
    public abstract class PropertyValueDTO<T> : BasePropertyValueDTO
    {
        [DataMember]
        public T Value { get; set; }
    }
}