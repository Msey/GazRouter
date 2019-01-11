using System;
using GazRouter.DAL.Core;
using GazRouter.DTO.Dictionaries.PeriodTypes;
using GazRouter.DTO.Dictionaries.ValueTypes;
using GazRouter.DTO.SeriesData.PropertyValues;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.SeriesData
{
    public static class PropertyValueHelper
    {
        private const string ValueStrColumnName = "value_str";
        private const string ValueNumColumnName = "value_num";
        private const string ValueDatColumnName = "value_dat";
        private const string ValueTypeColumnName = "value_type";
        private const string KeyDateColumnName = "key_date";
        private const string SeriesIdColumnName = "series_id";
        private const string PeriodTypeColumnName = "period_type_id";
        private const string SourceTypeColumnName = "source_type";

        public static BasePropertyValueDTO GetValue(OracleDataReader reader, bool createEmpty = false)
        {
            var date = reader.GetValue<DateTime?>(KeyDateColumnName);
            var seriesId = reader.GetValue<int>(SeriesIdColumnName);
            var sourceType = reader.GetValue<SourceType?>(SourceTypeColumnName);
            var periodType = reader.GetValue<PeriodType>(PeriodTypeColumnName);
            if (!date.HasValue)
                return null;

            BasePropertyValueDTO emptyDto = createEmpty
                ? new PropertyValueEmptyDTO
                {
                    Date = date.Value,
                    PeriodTypeId = periodType,
                    SeriesId = seriesId,
                    SourceType = sourceType
                }
                : null;

            var type = GetValueType(reader, ValueTypeColumnName);

            if (!type.HasValue)
            {
                return emptyDto;
            }

            switch (type.Value)
            {
                case ValueTypesEnum.STRING:
                {
                    var val = reader.GetValue<string>(ValueStrColumnName);
                    return string.IsNullOrEmpty(val)
                        ? emptyDto
                        : new PropertyValueStringDTO
                        {
                            Value = val,
                            Date = date.Value,
                            PeriodTypeId = periodType,
                            SeriesId = seriesId,
                            SourceType = sourceType
                        };
                }
                case ValueTypesEnum.DOUBLE:
                case ValueTypesEnum.INTEGER:
                {
                    var val = reader.GetValue<double?>(ValueNumColumnName);
                    return val.HasValue
                        ? new PropertyValueDoubleDTO
                        {
                            Value = val.Value,
                            Date = date.Value,
                            Year = date.Value.Year,
                            Month = date.Value.Month,
                            Day = date.Value.Day,
                            PeriodTypeId = periodType,
                            SeriesId = seriesId,
                            SourceType = sourceType
                        }
                        : emptyDto;
                }

                case ValueTypesEnum.DATE:
                {
                    var val = reader.GetValue<DateTime?>(ValueDatColumnName);
                    return val.HasValue
                        ? new PropertyValueDateDTO
                        {
                            Value = val.Value,
                            Date = date.Value,
                            PeriodTypeId = periodType,
                            SeriesId = seriesId,
                            SourceType = sourceType
                        }
                        : emptyDto;
                }
                default:
                    throw new Exception("Тип не поддерживается");
            }
        }

        public static ValueTypesEnum? GetValueType(OracleDataReader reader, string valueTypeColumnName)
        {
            ValueTypesEnum type;
            if (Enum.TryParse(reader.GetValue<string>(valueTypeColumnName), true, out type))
            {
                return type;
            }
            return null;
        }
    }
}
