using System.Collections.Generic;
using System.Runtime.Serialization;

namespace GazRouter.DTO.Dictionaries.SuperchargerTypes
{
    [DataContract]
	public class SuperchargerTypeDTO : BaseDictionaryDTO
    {
        /// <summary>
        /// Номинальный политропный к.п.д. ЦБН
        /// </summary>
        [DataMember]
        public double? NCbnRated { get; set; }
        
        /// <summary>
        /// Номинальная степень сжатия                                                 
        /// </summary>
        [DataMember]
        public double? ERated { get; set; }

        /// <summary>
        /// Номинальный коэффициент адиабаты                                           
        /// </summary>
        [DataMember]
        public double? KaRated { get; set; }

        /// <summary>
        /// Номинальный коэффициент сжимаемости                                        
        /// </summary>
        [DataMember]
        public double? ZRated { get; set; }

        /// <summary>
        /// Номинальная газовая постоянная, кДж/(кг*К)                                 
        /// </summary>
        [DataMember]
        public double? RRated { get; set; }

        /// <summary>
        /// Номинальная температура, К                                                 
        /// </summary>
        [DataMember]
        public double? TRated { get; set; }

        /// <summary>
        /// Номимнальные обороты нагнетателя, об/мин                                   
        /// </summary>
        [DataMember]
        public double? RpmRated { get; set; }

        /// <summary>
        /// Минимально допустимая объемная подача, м3/мин                              
        /// </summary>
        [DataMember]
        public double? QMin { get; set; }

        /// <summary>
        /// Максимально допустимая объемная подача, м3/мин                             
        /// </summary>
        [DataMember]
        public double? QMax { get; set; }

        /// <summary>
        /// Точки на ГДХ
        /// </summary>
        [DataMember]
        public List<SuperchargerChartPointDTO> ChartPoints { get; set; }

    }
}
