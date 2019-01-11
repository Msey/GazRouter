using System;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;

namespace GazRouter.DAL.Core
{
    public static class TypeMapper
    {
        private const int Varchar2MaxLength = 4000;

        public static OracleDbType GetOracleType(Type netType, int? length = null)
        {
            var type = GetUnderliyngType(netType);
            if ((type == typeof(int)) ||
                (type == typeof(double)) || type.IsEnum)
            {
                return OracleDbType.Decimal;
            }

            if (type == typeof(string))
            {
                if (!length.HasValue || length.Value <= Varchar2MaxLength)
                {
                    return OracleDbType.Varchar2;
                }
                return OracleDbType.Clob;
            }

            if (type == typeof(Guid))
            {
                return OracleDbType.Raw;
            }

            if (type == typeof(DateTime))
            {
                return OracleDbType.Date;
            }

            if (type == typeof(bool))
            {
                return OracleDbType.Decimal;
            }

            if (type == typeof(byte[]))
            {
                return OracleDbType.Blob;
            }

            throw new Exception("Тип не поддерживается");
        }

        public static T GetValueFromReader<T>(OracleDataReader reader, string columnName)
        {
            var colNum = reader.GetOrdinal(columnName);

            if (reader.IsDBNull(colNum))
            {
                var defaultValue = default(T);
                var resType = typeof(T);
                if (resType.IsEnum)
                {
                    ThrowEnumError(resType, "null");
                }
                return defaultValue;
            }
            switch (reader.GetDataTypeName(colNum))
            {
                case "Raw":
                    return (T) (object) new Guid((byte[]) reader.GetValue(colNum));
                case "Int16":
                    return ExtractFromInt<T, short>((short) reader.GetValue(colNum));
                case "Int32":
                    return ExtractFromInt<T, int>((int) reader.GetValue(colNum));
                case "Decimal":
                    return ExtractFromOracleDecimal<T>(reader.GetOracleDecimal(colNum));
                case "Clob":
                case "Char":
                case "Varchar2":
                case "NVarchar2":
                    return ExtractFromString<T>((string) reader.GetValue(colNum));
                case "Date":
                case "TimeStamp":
                    return ExtractFromDate<T>((DateTime) reader.GetValue(colNum));
                case "Blob":
                    return ExtractFromBlob<T>((byte[]) reader.GetValue(colNum));
                default:
                    throw new Exception("Тип не поддерживается");
            }
        }

        public static T GetValueFromParameter<T>(OracleParameter parameter)
        {
            if (((INullable) parameter.Value).IsNull)
            {
                return default(T);
            }

            switch (parameter.OracleDbType)
            {
                case OracleDbType.Decimal:
                    return ExtractFromOracleDecimal<T>((OracleDecimal) parameter.Value);
                case OracleDbType.Varchar2:
                    return ExtractFromString<T>(((OracleString) parameter.Value).Value);
                case OracleDbType.Date:
                    return ExtractFromDate<T>(((OracleDate) parameter.Value).Value);
                case OracleDbType.Raw:
                    return ExtractFromBinary<T>(((OracleBinary) parameter.Value).Value);
                case OracleDbType.Blob:
                    return ExtractFromBlob<T>(((OracleBlob) parameter.Value).Value);
                case OracleDbType.Clob:
                    return ExtractFromClob<T>(((OracleClob) parameter.Value).Value);
                default:
                    throw new Exception("Тип не поддерживается");
            }
        }


        private static Type GetUnderliyngType(Type type)
        {
            return Nullable.GetUnderlyingType(type) ?? type;
        }

        private static void CheckEnum(Type resType, object value)
        {
            if (!Enum.IsDefined(resType, value))
            {
                ThrowEnumError(resType, value);
            }
        }

        private static void ThrowEnumError(Type resType, object value)
        {
            throw new Exception($"Невозможно преобразовать значение {value} к типу {resType}");
        }

        private static TRes ExtractFromInt<TRes, TArg>(TArg val)
        {
            var resType = GetUnderliyngType(typeof(TRes));

            if (resType == typeof(bool))
            {
                var v = Convert.ToInt32(val);
                return (TRes) (object) ConvertIntToBool(v, typeof(TArg), typeof(TRes));
            }
            if (typeof(int) == resType || typeof(double) == resType)
            {
                return (TRes) Convert.ChangeType(val, resType);
            }
            if (resType.IsEnum)
            {
                var v = Convert.ToInt32(val);
                CheckEnum(resType, v);
                return (TRes) Enum.ToObject(resType, val);
            }
            throw new Exception("Тип не поддерживается");
        }

        private static TRes ExtractFromOracleDecimal<TRes>(OracleDecimal val)
        {
            var resType = GetUnderliyngType(typeof(TRes));
            if (resType == typeof(double))
            {
                return (TRes) (object) val.ToDouble();
            }
            if (resType == typeof(int))
            {
                return (TRes) (object) val.ToInt32();
            }
            if (resType == typeof(bool))
            {
                var v = val.ToInt32();
                return (TRes) (object) ConvertIntToBool(v, typeof(OracleDecimal), typeof(TRes));
            }
            if (resType.IsEnum)
            {
                var intValue = Convert.ToInt32(val.Value);
                CheckEnum(resType, intValue);
                return (TRes) Enum.ToObject(resType, intValue);
            }
            throw new Exception("Тип не поддерживается");
        }

        private static bool ConvertIntToBool(int v, Type resType, Type argType)
        {
            if (!((v == 1) || (v == 0)))
            {
                throw new Exception($"Ошибка преобразования {argType} в {resType}");
            }

            return v == 1;
        }

        private static T ExtractFromString<T>(string val)
        {
            if (typeof(string) == typeof(T))
            {
                return (T) (object) val;
            }

            throw new Exception("Тип не поддерживается");
        }

        private static T ExtractFromDate<T>(DateTime val)
        {
            var type = GetUnderliyngType(typeof(T));
            if (typeof(DateTime) == type)
            {
                return (T) Convert.ChangeType(DateTime.SpecifyKind(val, DateTimeKind.Local), type);
            }

            throw new Exception("Тип не поддерживается");
        }

        private static T ExtractFromBinary<T>(byte[] val)
        {
            var type = GetUnderliyngType(typeof(T));
            if (typeof(Guid) == typeof(T))
            {
                return (T) Convert.ChangeType(new Guid(val), type);
            }
            throw new Exception("Тип не поддерживается");
        }

        private static T ExtractFromBlob<T>(byte[] val)
        {
            if (typeof(byte[]) == typeof(T))
            {
                return (T) (object) val;
            }

            throw new Exception("Тип не поддерживается");
        }

        private static T ExtractFromClob<T>(string val)
        {
            if (typeof(string) == typeof(T))
            {
                return (T)(object)val;
            }

            throw new Exception("Тип не поддерживается");
        }

    }
}