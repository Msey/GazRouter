﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace GazRouter.Service.Exchange.Lib.AsduEsg.ExchangeFormat
{
    public class XmlElementId
    {
        [XmlAttribute(AttributeName = "id")]
        public string Id { get; set; }
    }
}