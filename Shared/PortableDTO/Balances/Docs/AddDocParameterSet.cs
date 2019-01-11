
using System;
using GazRouter.DTO.Attachments;

namespace GazRouter.DTO.Balances.Docs
{
    public class AddDocParameterSet : AddAttachmentParameterSet<int>
    {
        public DateTime DocDate { get; set; }
       
    }

}