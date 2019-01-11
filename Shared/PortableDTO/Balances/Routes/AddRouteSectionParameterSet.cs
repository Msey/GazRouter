using System;
using System.Runtime.Serialization;

namespace GazRouter.DTO.Balances.Routes
{
    [DataContract]
	public class AddRouteSectionParameterSet
    {
        /// <summary>
        /// Id ��������
        /// </summary>
        public int RouteId { get; set; }

        /// <summary>
        /// Id �����
        /// </summary>
        public Guid PipelineId { get; set; }

        /// <summary>
        /// �� ����� ������
        /// </summary>
        public double? KilometerEnd { get; set; }

        /// <summary>
        /// �� ������ ������
        /// </summary>
        public double? KilometerStart { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int? SortOrder{ get; set; }

	}
}