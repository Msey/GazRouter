using System;


namespace GazRouter.DTO.Repairs.RepairReport
{
    public class RepairReportParametersSet
    {
        public int? Id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int RepairID { get; set; }
        /// <summary>
        /// дата создания
        /// </summary>
        //public DateTime CreationDate { get; set; }
        /// <summary>
        /// отчетное время
        /// </summary>
        public DateTime ReportDate { get; set; }
        /// <summary>
        /// текст  отчета
        /// </summary>
        public string Comment { get; set; }
        /// <summary>
        /// Пользователь создавший отчет
        /// </summary>
        public string CreateUser { get; set; }
    }
}
