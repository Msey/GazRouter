using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GazRouter.DTO.Authorization.User
{
    public class AddAgreedUserParameterSet
    {      
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string Position { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndtDate { get; set; }
        public int? ActingId { get; set; }

        public string UserName_R { get; set; }
        public string Position_R { get; set; }
        public bool IsHead { get; set; }
    }
}
