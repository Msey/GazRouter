using System;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Utils.Units
{
    /// <summary>
    /// Структура для хранения давления
    /// </summary>
 
    public struct Pressure : IComparable, IComparable<Pressure>, IXmlSerializable
    {
        const double OneKgh= 98066.5; //в Па
        const double OneMmHg = 133.32266752; //в Па 
        
        //базовая единица 
    
        private  double _pascals;

        public Pressure(double pascals)
        {
            _pascals = pascals;
        }

        public static Pressure Zero => new Pressure();

        public double Kgh => _pascals / OneKgh;

        public double Mpa => _pascals / 1000000.0;

        /// <summary>
        /// Давление в мм. рт. ст. (торр)
        /// </summary>
        public double MmHg => _pascals / OneMmHg;

        /// <summary>
        /// Получение давления из кг/cм2
        /// </summary>
        /// <param name="kgh">Давление в  кг/cм2</param>
        public static Pressure FromKgh(double kgh)
        {
  
            return new Pressure(kgh * OneKgh);
        }


        /// <summary>
        /// Получение давления из МПа
        /// </summary>
        /// <param name="mpa">Давление в МПа</param>
        /// <returns></returns>
        public static Pressure FromMpa(double mpa)
        {
            return  new Pressure((mpa) * 1000000.0);
        }


        /// <summary>
        /// Получение давления из мм. рт. ст. (торр)
        /// </summary>
        /// <param name="torrs">Давление в  мм. рт. ст.</param>
        /// <returns></returns>
        public static Pressure FromMmHg(double torrs)
        {
            return new Pressure(torrs * OneMmHg );
        }

        public static Pressure From(double value, PressureUnit fromUnit)
        {
            switch (fromUnit)
            {
                case PressureUnit.Kgh:
                    return FromKgh(value);
                case PressureUnit.Mpa:
                    return FromMpa(value);
                case PressureUnit.MmHg:
                    return FromMmHg(value);
                default:
                    throw new NotSupportedException($"{nameof(fromUnit)}: {fromUnit}");
            }
        }

        public override string ToString()
        {
            return ToString(PressureUnit.Kgh);
        }

        public string ToString(PressureUnit pressureUnit)
        {
            return $"{As(pressureUnit)} {GetAbbreviation(pressureUnit)}";
        }

        public static string GetAbbreviation(PressureUnit pressureUnit)
        {
            switch (pressureUnit)
            {
                case PressureUnit.Kgh:
                    return "кгс/см²";
                case PressureUnit.Mpa:
                    return "МПа";
                case PressureUnit.MmHg:
                    return "мм рт.ст.";
                default:
                    throw new NotSupportedException($"{nameof(pressureUnit)}: {pressureUnit}");
            }
        }

        public double As(PressureUnit pressureUnit)
        {
            switch (pressureUnit)
            {
                case PressureUnit.Kgh:
                    return Kgh;
                case PressureUnit.Mpa:
                    return Mpa;
                case PressureUnit.MmHg:
                    return MmHg;
                default:
                    throw new NotSupportedException($"{nameof(pressureUnit)}: {pressureUnit}");
            }
        }

        public static Pressure operator +(Pressure left, Pressure right)
        {
            return new Pressure(left._pascals + right._pascals);
        }

        public static double operator /(Pressure left, Pressure right)
        {
            return Convert.ToDouble(left._pascals / right._pascals);
        }


        public static Pressure operator /(Pressure left, double right)
        {
            return new Pressure(left._pascals / right);
        }

        public static bool operator ==(Pressure left, Pressure right)
        {
            return left._pascals == right._pascals;
        }

        public static bool operator !=(Pressure left, Pressure right)
        {
            return left._pascals != right._pascals;
        }

        public static bool operator <=(Pressure left, Pressure right)
        {
            return left._pascals <= right._pascals;
        }

        public static bool operator >=(Pressure left, Pressure right)
        {
            return left._pascals >= right._pascals;
        }

        public static bool operator <(Pressure left, Pressure right)
        {
            return left._pascals < right._pascals;
        }

        public static bool operator >(Pressure left, Pressure right)
        {
            return left._pascals > right._pascals;
        }


        public int CompareTo(object obj)
        {
            if (obj == null)
                throw new ArgumentNullException(nameof(obj));
            if (!(obj is Pressure))
                throw new ArgumentException("Expected type Pressure.", nameof(obj));
            return CompareTo((Pressure)obj);
        }

        public int CompareTo(Pressure other)
        {
            return _pascals.CompareTo(other._pascals);
        }


        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;
            return _pascals.Equals(((Pressure)obj)._pascals);
        }

        public override int GetHashCode()
        {
            return _pascals.GetHashCode();
        }

        XmlSchema IXmlSerializable.GetSchema()
        {
            return null;
        }

        void IXmlSerializable.ReadXml(XmlReader reader)
        {
            double.TryParse(reader.GetAttribute("Pascals"), out _pascals) ;
        }

        void IXmlSerializable.WriteXml(XmlWriter writer)
        {
            writer.WriteAttributeString("Pascals",_pascals.ToString());
        }
    }
}