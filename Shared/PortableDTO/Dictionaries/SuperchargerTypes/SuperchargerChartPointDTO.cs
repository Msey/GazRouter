using System.Runtime.Serialization;

namespace GazRouter.DTO.Dictionaries.SuperchargerTypes
{
    [DataContract]
	public class SuperchargerChartPointDTO : BaseDictionaryDTO
    {
        /// <summary>
        /// Id ���� ����������� (��� id ���������)
        /// </summary>
        [DataMember]
        public int ParentId { get; set; }

        /// <summary>
        /// ��� �����                                            
        /// </summary>
        [DataMember]
        public int LineType { get; set; }

        /// <summary>
        /// ���������� �� ��� �x                                         
        /// </summary>
        [DataMember]
        public double X { get; set; }

        /// <summary>
        /// ���������� �� ��� �y                                      
        /// </summary>
        [DataMember]
        public double Y { get; set; }

    }
}
