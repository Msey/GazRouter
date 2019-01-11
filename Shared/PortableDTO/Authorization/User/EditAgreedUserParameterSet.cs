using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GazRouter.DTO.Authorization.User
{
    public class EditAgreedUserParameterSet : AddAgreedUserParameterSet
    {
        public int AgreedUserId { get; set; }
    }
}
