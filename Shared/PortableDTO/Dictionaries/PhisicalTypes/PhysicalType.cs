using System;
using System.Collections.Generic;
using Utils.Units;

namespace GazRouter.DTO.Dictionaries.PhisicalTypes
{
	public enum PhysicalType
	{
        Pressure = 102,
		Temperature = 103,
		Volume = 104,
		Density = 105,
		Quantity = 106,
		Caloricity = 107,
		StateSet = 108,
		Timestamp = 109,
		Text = 110,
        Coefficient = 111,
		Rpm = 112,
        Concentration = 113,
	    CombustionHeat = 115,
        None = 118,
        Work = 119,
        Percentage = 120,
        Power = 121,
        Content = 122,
        PressureAir = 123
	}

    public class UnitConverterBase
    {
        public UnitConverterBase(string name, Func<double, double> forwardFunc, Func<double, double> backwardFunc, object unit)
        {
            Name = name;
            ForwardFunc = forwardFunc;
            BackwardFunc = backwardFunc;
            Unit = unit;
        }
        public string Name { get; protected set; }
        public Func<double, double> ForwardFunc { get; protected set; }
        public Func<double, double> BackwardFunc { get; protected set; }
        public object Unit { get; set; }
    }

    public static class AllConverters
    {
        private static readonly Dictionary<PhysicalType, IEnumerable<UnitConverterBase>> _converters;

        static AllConverters()
        {
            _converters = new Dictionary<PhysicalType, IEnumerable<UnitConverterBase>>
                {
                    //кг/см² -> МПа    
                    {
                        PhysicalType.Pressure, new List<UnitConverterBase>
                            {
                                new UnitConverterBase("МПа", PhysicalQuantityConversions.Kgh2Mpa, PhysicalQuantityConversions.Mpa2Kgh, PressureUnit.Mpa)
                            }
                    },
                    //Гр.С -> Гр.К 
                    {
                        PhysicalType.Temperature, new List<UnitConverterBase>
                            {
                                new UnitConverterBase("K", PhysicalQuantityConversions.C2K, PhysicalQuantityConversions.K2C, TemperatureUnit.Kelvin)
                            }
                    }
                };
        }

        public static List<UnitConverterBase> GetConverters(PhysicalType key)
        {
            return _converters.ContainsKey(key)
                       ? new List<UnitConverterBase>(_converters[key])
                       : new List<UnitConverterBase>();
        }
    }
}
