using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace GazRouter.DTO.Repairs.Agreed
{
    [DataContract]
    public class AgreedUserDTO : BaseDto<int>
    {
        /// <summary>
        /// Идентифиактор согласующего пользователя
        /// </summary>
        [DataMember]
        public int AgreedUserId { get; set; }
        /// <summary>
        /// Идентификатор пользователя
        /// </summary>
        [DataMember]
        public int UserID { get; set; }
        /// <summary>
        /// ФИО
        /// </summary>
        [DataMember]
        public string FIO { get; set; }
        /// <summary>
        /// Должность
        /// </summary>
        [DataMember]
        public string Position { get; set; }

        [DataMember]
        public Guid? SiteId { get; set; }
        [DataMember]
        public string SiteName { get; set; }

        /// <summary>
        /// Дата начала полномочий
        /// </summary>
        [DataMember]
        public DateTime StartDate { get; set; }
        /// <summary>
        /// Дата окончания полномочий
        /// </summary>
        [DataMember]
        public DateTime EndDate { get; set; }
        /// <summary>
        /// Идентифиактор замещаемого пользователя 
        /// </summary>
        [DataMember]
        public int? ActingUserID { get; set; }
        /// <summary>
        /// Идентифиактор согласующего замещаемого пользователя
        /// </summary>
        [DataMember]
        public int? ActingAgreedUserId { get; set; }
        /// <summary>
        /// Имя пользователя замещаемого
        /// </summary>
        [DataMember]
        public string ActingName { get; set; }


        /// <summary>
        /// ФИО в Родительном падеже
        /// </summary>
        [DataMember]
        public string FIO_R { get; set; }
        /// <summary>
        /// Должность в родительном падеже
        /// </summary>
        [DataMember]
        public string Position_r { get; set; }

        [DataMember]
        public bool IsHead { get; set; }
    }
}