using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace GazRouter.DTO.Repairs.Plan
{
    
    [DataContract]
    public class RepairAttachmentDTO : BaseDto<int>
    {
        /// <summary>
        /// Идентификатор ремработ
        /// </summary>
        [DataMember]
        public Guid RepairID { get; set; }
        /// <summary>
        /// Идентификатор блоба
        /// </summary>
        [DataMember]
        public Guid BlobID { get; set; }
        /// <summary>
        /// Описание
        /// </summary>
        [DataMember]
        public string Description { get; set; }
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
