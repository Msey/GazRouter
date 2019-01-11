using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace GazRouter.DTO.Repairs.Agreed
{
    [DataContract]
    public class AddEditAgreedRepairRecordParameterSet : BaseDto<int>
    {
        /// <summary>
        /// Идентификатор ремонтной работы
        /// </summary>
        [DataMember]
        public int RepairID { get; set; }
        /// <summary>
        /// Идентификатор согласующего пользователя
        /// </summary>
        [DataMember]
        public int AgreedUserId { get; set; }
        /// <summary>
        /// Идентификатор фактически подписавшего пользователя
        /// </summary>
        [DataMember]
        public int? RealAgreedUserId { get; set; }
        /// <summary>
        /// Дата согласования
        /// </summary>
        [DataMember]
        public DateTime? AgreedDate { get; set; }
        /// <summary>
        /// Дата создания
        /// </summary>
        [DataMember]
        public DateTime CreationDate { get; set; }
        /// <summary>
        /// Примечание
        /// </summary>
        [DataMember]
        public string Comment { get; set; }
        /// <summary>
        /// Результат согласования
        /// </summary>
        [DataMember]
        public bool? AgreedResult { get; set; }
    }
}