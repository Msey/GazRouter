using GazRouter.DTO.Dictionaries.PeriodTypes;
using GazRouter.Service.Exchange.Lib.AsduEsg.ExchangeFormat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace GazRouter.Service.Exchange.Lib.AsduEsg
{
    [XmlRoot("BusinessMessage", Namespace = "")]
    [Serializable]
    public class XmlExchangeObject
    {
        [XmlElement("HeaderSection")]
        public XmlExchangeHeader HeaderSection { get; set; }

        [XmlElement("DataSection")]
        public List<XmlDataSection> DataSections { get; set; }

    }

    public class XmlDataSection
    {
        public XmlExchangeIdentifier Identifier { get; set; }
        public string ParameterFullName { get; set; }
        public XmlExchangeData Value { get; set; }
        //[XmlAttribute(AttributeName = "dimension")]
        public string Dimension { get; set; }
    }

    public class XmlExchangeIdentifier
    {
        public XmlExchangeIdentifier()
        {
            Type = "ASDU_ESG";
        }

        [XmlAttribute(AttributeName = "type")]
        public string Type { get; set; }

        [XmlText]
        public string Id { get; set; }
    }

    public class XmlExchangeData
    {
        [XmlText]
        public string Value { get; set; }
    }

    public class XmlExchangeHeader
    {
        public XmlExchangeHeader()
        {
            Receiver = new XmlElementId { Id = @"М АСДУ ЕСГ" };
            Generated = new XmlAtDateTime { Value = DateTime.Now };
        }

        public XmlExchangeHeader(PeriodType periodType, DateTime keyDate, string template, string sender, string senderFullName)
        {
            DateTime refTime;
            switch (periodType)
            {
                case PeriodType.Twohours:
                    refTime = keyDate.ToMoscow();
                    break;
                case PeriodType.Day:
                    refTime = new DateTime(keyDate.Year, keyDate.Month, keyDate.Day, 10, 0, 0);
                    break;
                case PeriodType.Month:
                    refTime = new DateTime(keyDate.Year, keyDate.Month, 01, 10, 0, 0);
                    break;
                default:
                    refTime = keyDate.ToMoscow();
                    break;
            }

            //var refTime = periodType == PeriodType.Twohours ? keyDate.ToMoscow() : new DateTime(keyDate.Year, keyDate.Month, keyDate.Day, 10, 0, 0);
            //var refTimeMos = refTime.ToMoscow();
            Scale = periodType.ScaleName();
            Receiver = new XmlElementId { Id = @"М АСДУ ЕСГ" };
            Generated = new XmlAtDateTime { Value = DateTime.Now.ToMoscow() };
            Comment = periodType.Comment();
            //if (periodType != PeriodType.Month)
            ReferenceTime = new XmlInDateTime { Value = refTime };
            Template = new XmlElementId { Id = template };
            Sender = new XmlElementId { Id = sender };
            FullName = senderFullName;
        }

        [XmlElement(ElementName = "Sender")]
        public XmlElementId Sender { get; set; }
        public XmlElementId Receiver { get; set; }
        public XmlAtDateTime Generated { get; set; }
        public string Comment { get; set; }
        public XmlInDateTime ReferenceTime { get; set; }
        public string Scale { get; set; }
        public XmlElementId Template { get; set; }
        public string FullName { get; set; }
    }
}
