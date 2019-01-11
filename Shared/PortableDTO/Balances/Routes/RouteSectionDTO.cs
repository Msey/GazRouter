using System;
using System.Runtime.Serialization;

namespace GazRouter.DTO.Balances.Routes
{
    [DataContract]
	public class RouteSectionDTO
    {
        /// <summary>
        /// Id ������ ��������
        /// </summary>
        [DataMember]
        public int RouteSectionId { get; set; }

        /// <summary>
        /// Id ��������
        /// </summary>
        [DataMember]
        public int RouteId { get; set; }

        /// <summary>
        /// Id �����
        /// </summary>
        [DataMember]
        public Guid PipelineId { get; set; }


        /// <summary>
        /// ��� �����
        /// </summary>
        [DataMember]
        public string PipelineName { get; set; }


        /// <summary>
        /// �� ����� ������
        /// </summary>
        [DataMember]
        public double? KilometerEnd { get; set; }

        /// <summary>
        /// �� ������ ������
        /// </summary>
        [DataMember]
        public double? KilometerStart { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public int? SortOrder{ get; set; }

	}
}