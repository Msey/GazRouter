using System.Collections.Generic;
using System.Runtime.Serialization;

namespace GazRouter.DTO.Dictionaries.SuperchargerTypes
{
    [DataContract]
	public class SuperchargerTypeDTO : BaseDictionaryDTO
    {
        /// <summary>
        /// ����������� ����������� �.�.�. ���
        /// </summary>
        [DataMember]
        public double? NCbnRated { get; set; }
        
        /// <summary>
        /// ����������� ������� ������                                                 
        /// </summary>
        [DataMember]
        public double? ERated { get; set; }

        /// <summary>
        /// ����������� ����������� ��������                                           
        /// </summary>
        [DataMember]
        public double? KaRated { get; set; }

        /// <summary>
        /// ����������� ����������� �����������                                        
        /// </summary>
        [DataMember]
        public double? ZRated { get; set; }

        /// <summary>
        /// ����������� ������� ����������, ���/(��*�)                                 
        /// </summary>
        [DataMember]
        public double? RRated { get; set; }

        /// <summary>
        /// ����������� �����������, �                                                 
        /// </summary>
        [DataMember]
        public double? TRated { get; set; }

        /// <summary>
        /// ������������ ������� �����������, ��/���                                   
        /// </summary>
        [DataMember]
        public double? RpmRated { get; set; }

        /// <summary>
        /// ���������� ���������� �������� ������, �3/���                              
        /// </summary>
        [DataMember]
        public double? QMin { get; set; }

        /// <summary>
        /// ����������� ���������� �������� ������, �3/���                             
        /// </summary>
        [DataMember]
        public double? QMax { get; set; }

        /// <summary>
        /// ����� �� ���
        /// </summary>
        [DataMember]
        public List<SuperchargerChartPointDTO> ChartPoints { get; set; }

    }
}
