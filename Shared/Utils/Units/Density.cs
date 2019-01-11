using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Utils.Units
{
    /// <summary>
    /// Плотность
    /// </summary>
    public struct Density : IXmlSerializable
    {
        private double _kilogramsPerCubicMeter;

        private Density(double kilogramspercubicmeter) : this()
        {
            _kilogramsPerCubicMeter = kilogramspercubicmeter;
        }


        public double KilogramsPerCubicMeter => _kilogramsPerCubicMeter;

        public static Density FromKilogramsPerCubicMeter(double kilogramspercubicmeter)
        {
            return new Density(kilogramspercubicmeter);
        }



        XmlSchema IXmlSerializable.GetSchema()
        {
            return null;
        }

        void IXmlSerializable.ReadXml(XmlReader reader)
        {
            double.TryParse(reader.GetAttribute("KilogramsPerCubicMeter"), out _kilogramsPerCubicMeter);
        }

        void IXmlSerializable.WriteXml(XmlWriter writer)
        {
            writer.WriteAttributeString("KilogramsPerCubicMeter", _kilogramsPerCubicMeter.ToString());
        }
    }
}