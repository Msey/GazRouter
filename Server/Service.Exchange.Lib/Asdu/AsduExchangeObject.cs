using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using GazRouter.DataServices.Infrastructure;

namespace GazRouter.Service.Exchange.Lib.Asdu
{
    [XmlRoot("BusinessMessage", Namespace = "")]
    [Serializable]
    public class AsduExchangeObject 
    {
        [XmlElement("HeaderSection")]
        public AsduExchangeHeader HeaderSection { get; set; }

        [XmlElement("DataSection")]
        public List<AsduDataSection> DataSections { get; set; }

    }

    public class AsduExchangeHeader
    {
        public AsduExchangeHeader()
        {
            Scale = "PT2H";
            Receiver = new AsduElementId { Id = @"М АСДУ ЕСГ" };
            Generated = new AsduAtDateTime { Value = DateTime.Now };
            Comment = @"Файл со значениями технологических параметров. Часовой.";
        }



        [XmlElement(ElementName = "Sender")]
        public AsduElementId Sender { get; set; }
        public AsduElementId Receiver { get; set; }
        public AsduAtDateTime Generated { get; set; }
        public string Comment { get; set; }
        public AsduInDateTime ReferenceTime { get; set; }
        public string Scale { get; set; }
        public AsduElementId Template { get; set; }
    }

    public class AsduDateTime
    {
        protected const string DatetimeFormat = @"{0:yyyy""-""MM""-""dd""T""HH"":""mm"":""sszzz}";

        [XmlIgnore]
        public DateTime Value { get; set; }
    }

    public class AsduInDateTime : AsduDateTime
    {
        [XmlAttribute("time")]
        public string ValueString
        {
            get { return string.Format(DatetimeFormat, this.Value); }
            set { this.Value = DateTime.Parse(value); }
        }
    }

    public class AsduAtDateTime : AsduDateTime
    {
        [XmlAttribute("at")]
        public string ValueString
        {
            get { return string.Format(DatetimeFormat, this.Value); }
            set { this.Value = DateTime.Parse(value); }
        }
    }

    public class AsduElementId
    {
        [XmlAttribute(AttributeName = "id")]
        public string Id { get; set; }
    }

    public class AsduDataSection
    {
        public AsduExchangeIdentifier Identifier { get; set; }
        //public string ParameterFullName { get; set; }
        public AsduExchangeData Value { get; set; }

        //[XmlAttribute(AttributeName = "dimension")]
        //public string Dimension { get; set; }
    }

    public class AsduExchangeIdentifier
    {
        public AsduExchangeIdentifier()
        {
            Type = "ASDU_ESG";
        }

        [XmlAttribute(AttributeName = "type")]
        public string Type { get; set; }

        [XmlText]
        public string Id { get; set; }
    }

    public class AsduExchangeData
    {
        [XmlText]
        public string Value { get; set; }
    }
}