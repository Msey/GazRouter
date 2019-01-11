using GazRouter.DTO.Attachments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace GazRouter.DTO.Repairs.RepairReport
{
    [DataContract]
    public class RepairReportAttachmentDTO : AttachmentBaseDTO //BaseDto<int>
    {
        [DataMember]
        public int Id { get; set; }
        /// <summary>
        /// Идентификатор ремработ
        /// </summary>
        [DataMember]
        public int RepairReportID { get; set; }
        /// <summary>
        /// Идентификатор блоба
        /// </summary>
        //[DataMember]
        //public Guid BlobID { get; set; }
        /// <summary>
        /// Описание
        /// </summary>
        //[DataMember]
        //public string Description { get; set; }
        //[DataMember]
        //public string Filename { get; set; }
        //[DataMember]
        //public int DataLenght { get; set; }
        /// <summary>
        /// Дата создания
        /// </summary>
        [DataMember]
        public DateTime CreationDate { get; set; }
        /// <summary>
        /// Пользователь создавший вложение
        /// </summary>
        [DataMember]
        public string CreateUser { get; set; }
    }
}
