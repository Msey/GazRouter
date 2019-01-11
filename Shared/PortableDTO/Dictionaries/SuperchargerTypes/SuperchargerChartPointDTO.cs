using System.Runtime.Serialization;

namespace GazRouter.DTO.Dictionaries.SuperchargerTypes
{
    [DataContract]
	public class SuperchargerChartPointDTO : BaseDictionaryDTO
    {
        /// <summary>
        /// Id типа нагнетателя (или id испытания)
        /// </summary>
        [DataMember]
        public int ParentId { get; set; }

        /// <summary>
        /// Тип линии                                            
        /// </summary>
        [DataMember]
        public int LineType { get; set; }

        /// <summary>
        /// Координата по оси Оx                                         
        /// </summary>
        [DataMember]
        public double X { get; set; }

        /// <summary>
        /// Координата по оси Оy                                      
        /// </summary>
        [DataMember]
        public double Y { get; set; }

    }
}
