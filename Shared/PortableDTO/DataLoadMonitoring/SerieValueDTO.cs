using System;
using System.Runtime.Serialization;
using GazRouter.DTO.Dictionaries.PropertyTypes;

namespace GazRouter.DTO.DataLoadMonitoring
{
    [DataContract]
    public abstract class BaseSerieValue
    {
        //серия 
        [DataMember]
        public int SerieId { get; set; }
        //ид объекта
        [DataMember]
        public Guid EntityId { get; set; }
        //комментарий к значению
        [DataMember]
        public string Annotation { get; set; }
        //тип свойства
        [DataMember]
        public PropertyType Property { get; set; }
    }
    [DataContract]
    public abstract class BaseSerieValueDTO<T> : BaseSerieValue
    {
        //значение свойства
        [DataMember]
        public T Value { get; set; }
    }

    [DataContract]
    public class SerieValueStringDTO : BaseSerieValueDTO<string>
    {
    }
    [DataContract]
    public class SerieValueDateDTO : BaseSerieValueDTO<DateTime?>
    {
    }
    [DataContract]
    public class SerieValueDoubleDTO : BaseSerieValueDTO<double?>
    {
    }

}
