using System;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
namespace Utils.Units
{
    public struct Temperature : IComparable, IComparable<Temperature>, IXmlSerializable
    {
        private  double _kelvins;
        private Temperature(double kelvins) : this()
        {
            _kelvins = kelvins;
        }
        public static Temperature Zero => new Temperature();
        public double Celsius => Math.Round(_kelvins - 273.15, 1);
        public double Kelvins => _kelvins;
        public static Temperature FromCelsius(double celsius)
        {
            return new Temperature(celsius + 273.15);
        }
        public static Temperature FromKelvins(double kelvins)
        {
            return new Temperature(kelvins);
        }
        public static Temperature From(double value, TemperatureUnit fromUnit)
        {
            switch (fromUnit)
            {
                case TemperatureUnit.Celsius:
                    return FromCelsius(value);

                case TemperatureUnit.Kelvin:
                    return FromKelvins(value);
                    
                default:
                    throw new NotSupportedException($"{nameof(fromUnit)}: {fromUnit}");
            }
        }
        public int CompareTo(object obj)
        {
            if (obj == null)
                throw new ArgumentNullException(nameof(obj));
            if (!(obj is Temperature))
                throw new ArgumentException("Expected type Temperature.", nameof(obj));
            return CompareTo((Temperature)obj);
        }
        public int CompareTo(Temperature other)
        {
            return _kelvins.CompareTo(other._kelvins);
        }
        public static Temperature operator +(Temperature left, Temperature right)
        {
            return new Temperature(left._kelvins + right._kelvins);
        }
        public static Temperature operator /(Temperature left, double right)
        {
            return new Temperature(left._kelvins / right);
        }
        public static bool operator <=(Temperature left, Temperature right)
        {
            return left._kelvins <= right._kelvins;
        }
        public static bool operator >=(Temperature left, Temperature right)
        {
            return left._kelvins >= right._kelvins;
        }
        public static bool operator <(Temperature left, Temperature right)
        {
            return left._kelvins < right._kelvins;
        }
        public static bool operator >(Temperature left, Temperature right)
        {
            return left._kelvins > right._kelvins;
        }
        public override string ToString()
        {
            return ToString(TemperatureUnit.Celsius);
        }
        public string ToString(TemperatureUnit temperatureUnit)
        {
            return $"{As(temperatureUnit)} {GetAbbreviation(temperatureUnit)}";
        }
        public static string GetAbbreviation(TemperatureUnit unit)
        {
            switch (unit)
            {
                case TemperatureUnit.Celsius:
                    return "°C";

                case TemperatureUnit.Kelvin:
                    return "K";
                default:
                    throw new NotSupportedException($"{nameof(unit)}: {unit}");
            }
        }
        public double As(TemperatureUnit unit)
        {
            switch (unit)
            {
                case TemperatureUnit.Celsius:
                    return Celsius;
              
                case TemperatureUnit.Kelvin:
                    return Kelvins;
                default:
                    throw new NotSupportedException($"{nameof(unit)}: {unit}");
            }
        }
        XmlSchema IXmlSerializable.GetSchema()
        {
            return null;
        }
        void IXmlSerializable.ReadXml(XmlReader reader)
        {
            double.TryParse(reader.GetAttribute("Kelvins"), out _kelvins);
        }
        void IXmlSerializable.WriteXml(XmlWriter writer)
        {
            writer.WriteAttributeString("Kelvins", _kelvins.ToString());
        }
    }
}