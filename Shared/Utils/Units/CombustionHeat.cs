using System;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Utils.Units
{
    /// <summary>
    /// Структура для хранения давления
    /// </summary>
 
    public struct CombustionHeat : IComparable, IComparable<CombustionHeat>, IXmlSerializable
    {
        private const double _coef = 238.846;
        private  double _kcals;

        public CombustionHeat(double kcals)
        {
            _kcals = kcals;
        }
        
        public double MJ => Math.Round(_kcals / _coef, 3);

        public double KCal => _kcals;

        public static CombustionHeat FromKCal(double kcals)
        {
            return new CombustionHeat(kcals);
        }
        
        public static CombustionHeat FromMJ(double mj)
        {
            return new CombustionHeat(Math.Round(mj * _coef));
        }

        
        public static CombustionHeat From(double value, CombustionHeatUnit fromUnit)
        {
            switch (fromUnit)
            {
                case CombustionHeatUnit.MJ:
                    return FromMJ(value);
                case CombustionHeatUnit.kcal:
                    return FromKCal(value);
                
                default:
                    throw new NotSupportedException($"{nameof(fromUnit)}: {fromUnit}");
            }
        }

        public override string ToString()
        {
            return ToString(CombustionHeatUnit.kcal);
        }

        public string ToString(CombustionHeatUnit units)
        {
            return $"{As(units)} {GetAbbreviation(units)}";
        }

        public static string GetAbbreviation(CombustionHeatUnit units)
        {
            switch (units)
            {
                case CombustionHeatUnit.MJ:
                    return "МДж/м3";
                case CombustionHeatUnit.kcal:
                    return "ккал";
                default:
                    throw new NotSupportedException($"{nameof(units)}: {units}");
            }
        }

        public double As(CombustionHeatUnit units)
        {
            switch (units)
            {
                case CombustionHeatUnit.MJ:
                    return MJ;
                case CombustionHeatUnit.kcal:
                    return KCal;
                
                default:
                    throw new NotSupportedException($"{nameof(units)}: {units}");
            }
        }

        public static CombustionHeat operator +(CombustionHeat left, CombustionHeat right)
        {
            return new CombustionHeat(left._kcals + right._kcals);
        }

        public static double operator /(CombustionHeat left, CombustionHeat right)
        {
            return Convert.ToDouble(left._kcals / right._kcals);
        }


        public static CombustionHeat operator /(CombustionHeat left, double right)
        {
            return new CombustionHeat(left._kcals / right);
        }

        public static bool operator ==(CombustionHeat left, CombustionHeat right)
        {
            return left._kcals == right._kcals;
        }

        public static bool operator !=(CombustionHeat left, CombustionHeat right)
        {
            return left._kcals != right._kcals;
        }

        public static bool operator <=(CombustionHeat left, CombustionHeat right)
        {
            return left._kcals <= right._kcals;
        }

        public static bool operator >=(CombustionHeat left, CombustionHeat right)
        {
            return left._kcals >= right._kcals;
        }

        public static bool operator <(CombustionHeat left, CombustionHeat right)
        {
            return left._kcals < right._kcals;
        }

        public static bool operator >(CombustionHeat left, CombustionHeat right)
        {
            return left._kcals > right._kcals;
        }


        public int CompareTo(object obj)
        {
            if (obj == null)
                throw new ArgumentNullException(nameof(obj));
            if (!(obj is CombustionHeat))
                throw new ArgumentException("Expected type Pressure.", nameof(obj));
            return CompareTo((CombustionHeat)obj);
        }

        public int CompareTo(CombustionHeat other)
        {
            return _kcals.CompareTo(other._kcals);
        }


        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;
            return _kcals.Equals(((CombustionHeat)obj)._kcals);
        }

        public override int GetHashCode()
        {
            return _kcals.GetHashCode();
        }

        XmlSchema IXmlSerializable.GetSchema()
        {
            return null;
        }

        void IXmlSerializable.ReadXml(XmlReader reader)
        {
            double.TryParse(reader.GetAttribute("KCal"), out _kcals) ;
        }

        void IXmlSerializable.WriteXml(XmlWriter writer)
        {
            writer.WriteAttributeString("KCal", _kcals.ToString());
        }
    }
}