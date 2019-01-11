using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace GazRouter.DTO.Repairs.RepairReport
{
    [DataContract]
    public class RepairReportDTO : BaseDto<int>
    {
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public int RepairID { get; set; }
        /// <summary>
        /// дата создания
        /// </summary>
        [DataMember]
        public DateTime CreationDate { get; set; }
        /// <summary>
        /// отчетное время
        /// </summary>
        [DataMember]
        public DateTime ReportDate { get; set; }
        /// <summary>
        /// текст  отчета
        /// </summary>
        [DataMember]
        public string Comment { get; set; }
        /// <summary>
        /// Пользователь создавший отчет
        /// </summary>
        [DataMember]
        public string CreateUser { get; set; }
    }
}
