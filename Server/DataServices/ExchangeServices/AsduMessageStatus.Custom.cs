using System;

namespace GazRouter.DataServices.ExchangeServices
{

    public partial class XmlMessageStatus
    {
        public static string OkXml()
        {
            return XmlMessageFileHelper.ToXml(new XmlMessageStatus {Status = "Ok"});
        }
        
        public static string ErrorXml(Guid id, string message, int code)
        {
            return XmlMessageFileHelper.ToXml(new XmlMessageStatus { Status = "Error", Error = new XmlMessageStatusError { Message = message , Code = code, LogRecordId = id.ToString()} });
        }

    }
}
