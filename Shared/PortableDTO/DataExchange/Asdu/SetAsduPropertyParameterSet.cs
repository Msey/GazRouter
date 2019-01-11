using System;
using GazRouter.DTO.Dictionaries.PropertyTypes;

namespace GazRouter.DTO.DataExchange.Asdu
{
    public class SetAsduPropertyParameterSet
    {
        public Guid? ParameterGid { get; set; }
        public Guid EntityId { get; set; }
        public PropertyType? PropertyTypeId { get; set; }
    }
}