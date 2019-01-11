using System.Collections.Generic;
using System.Runtime.Serialization;

namespace GazRouter.DTO.SeriesData.ExtendedPropertyValues
{
    [DataContract]
    public class ExtendedPropertyValuesPageDTO
    {
        [DataMember]
        public int TotalCount { get; set; }

        [DataMember]
        public List<ExtendedPropertyValueDTO> Entities { get; set; }
    }
}