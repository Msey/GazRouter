using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace GazRouter.Service.Exchange.Lib.AsduEsg.ExchangeFormat
{
    public class XmlInDateTime : XmlDateTime
    {
        [XmlAttribute("time")]
        public string ValueString
        {
            get { return Value.ToStringWithMoscowZone(); }
            set { this.Value = DateTime.Parse(value); }
        }
    }
}
