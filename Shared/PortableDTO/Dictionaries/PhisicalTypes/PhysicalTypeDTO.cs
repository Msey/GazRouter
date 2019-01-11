using System.Collections.Generic;
using System.Runtime.Serialization;
using GazRouter.DTO.Dictionaries.ValueTypes;
using Utils.Units;

namespace GazRouter.DTO.Dictionaries.PhisicalTypes
{
    [DataContract]
	public class PhysicalTypeDTO : BaseDictionaryDTO
    {
        [DataMember]
        public string SysName { get; set; }

        [DataMember]
        public string UnitName { get; set; }

        [DataMember]
        public ValueTypesEnum? ValueType { get; set; }

        [DataMember]
        public int DefaultPrecision { get; set; }

        [DataMember]
        public bool TrendAllowed { get; set; }

        [DataMember]
        public double? ValueMin { get; set; }

        [DataMember]
        public double? ValueMax { get; set; }


        public PhysicalType PhysicalType => (PhysicalType)Id;


        public List<UnitConverterBase> GetConverters()
        {
            var convs = new List<UnitConverterBase>();
            if (!string.IsNullOrEmpty(UnitName))
            {
                object unit = null;
                switch (PhysicalType)
                {
                    case PhysicalType.Pressure:
                        unit = PressureUnit.Kgh;
                        break;
                    case PhysicalType.Temperature:
                        unit = TemperatureUnit.Celsius;
                        break;
                }
                convs.Add(new UnitConverterBase(UnitName, p => p, p => p, unit));
            }
            convs.AddRange(AllConverters.GetConverters(PhysicalType));
            return convs;
        }



    }
}
