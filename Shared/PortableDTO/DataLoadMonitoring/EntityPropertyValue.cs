using System;
using System.Runtime.Serialization;
using GazRouter.DTO.Dictionaries.PropertyTypes;
using GazRouter.DTO.Dictionaries.ValueTypes;
using GazRouter.DTO.SeriesData.Series;

namespace GazRouter.DTO.DataLoadMonitoring
{
    [DataContract]
    public abstract class BaseEntityProperty
    {
        //серия 
        [DataMember]
        public SeriesDTO DataSeries { get; set; }
        //ЛПУ
        [DataMember]
        public Guid SiteId { get; set; }

        [DataMember]
        public string SiteName { get; set; }
        //ид объекта
        [DataMember]
        public Guid EntityId { get; set; }
        //короткое имя объекта
        [DataMember]
        public string EntityShortName { get; set; }
        //наименование типа объекта
        [DataMember]
        public string EntityTypeName { get; set; } 
        //комментарий к значению
        [DataMember]
        public string Annotation { get; set; }
        //тип свойства
        [DataMember]
        public PropertyType Property { get; set; }
        //ид типа объекта
        [DataMember]
        public int EntityTypeId { get; set; }
    }
    [DataContract]
    public abstract class EntityPropertyValueDTO<T> :  BaseEntityProperty
    {
        //значение свойства
        [DataMember]
        public T Value { get; set; }
    }

    [DataContract]
    public class EntityPropertyValueStringDTO : EntityPropertyValueDTO<string>
    {
    }
    [DataContract]
    public class EntityPropertyValueDateDTO : EntityPropertyValueDTO<DateTime?>
    {
    }
    [DataContract]
    public class EntityPropertyValueDoubleDTO : EntityPropertyValueDTO<double?>
    {
    }


    public static class EntityPropertyValueHelper
    {
       
        public static ValueTypesEnum? GetValueType(string valueInputTypeString)
        {
            ValueTypesEnum type;
            if (Enum.TryParse(valueInputTypeString, true, out type))
            {
                return type;
            }
            return null;
        }

    }
    
  
}