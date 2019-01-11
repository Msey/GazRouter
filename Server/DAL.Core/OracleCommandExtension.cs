using System;
using System.Data;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.Core
{
    public static class OracleCommandExtension
    {
        #region AddInputParameter

        public static void AddInputParameter(this OracleCommand command, string parameterName, string parameter)
        {
            AddInputParameterInternal(command, parameterName, parameter);
        }

        public static void AddInputParameter(this OracleCommand command, string parameterName, byte[] parameter)
        {
            AddInputParameterInternal(command, parameterName, parameter);
        }

        public static void AddInputParameter<T>(this OracleCommand command, string parameterName, T parameter)
            where T : struct
        {
            var oracleType = TypeMapper.GetOracleType(typeof(T));
            var val = ConvertToOracleParameterValue(parameter);
            command.Parameters.Add(parameterName, oracleType, val, ParameterDirection.Input);
        }

        public static void AddInputParameter<T>(this OracleCommand command, string parameterName, T? parameter)
            where T : struct
        {
            var oracleType = TypeMapper.GetOracleType(typeof(T));
            var val = ConvertToOracleParameterValue(parameter);
            command.Parameters.Add(parameterName, oracleType, val, ParameterDirection.Input);
        }

        #endregion

        private static void AddInputParameterInternal<T>(this OracleCommand command, string parameterName, T parameter)
        {
            var oracleType = TypeMapper.GetOracleType(typeof(T));
            var val = ConvertToOracleParameterValue(parameter);
            command.Parameters.Add(parameterName, oracleType, val, ParameterDirection.Input);
        }

        private static void AddInputParameterInternal(this OracleCommand command, string parameterName, string parameter)
        {
            var oracleType = string.IsNullOrEmpty(parameter)
                ? TypeMapper.GetOracleType(typeof (string))
                : TypeMapper.GetOracleType(typeof (string), parameter.Length);
            command.Parameters.Add(parameterName, oracleType, parameter, ParameterDirection.Input);
        }

        private static object ConvertToOracleParameterValue<T>(T parameter)
        {
            var netType = typeof(T);
            if (netType == typeof(Guid))
            {
                return ((Guid)(object)parameter).ToByteArray();
            }
            if (netType == typeof(bool))
            {
                return ((bool)(object)parameter) ? 1 : 0;
            }
            if (netType.IsEnum)
            {
                return (int)(object)parameter;
            }
            return parameter;
        }

        private static object ConvertToOracleParameterValue<T>(T? parameter)
            where T : struct
        {
            var innerType = typeof(T);
            if (innerType == typeof(Guid))
            {
                var p = (Guid?)(object)parameter;
                if (p.HasValue)
                {
                    return (p.Value).ToByteArray();
                }
            }
            if (innerType == typeof(bool))
            {
                var p = (bool?)(object)parameter;
                if (p.HasValue)
                {
                    return p.Value ? 1 : 0;
                }
            }
            if (innerType.IsEnum)
            {
                if (parameter.HasValue)
                {
                    return (int)(object)parameter.Value;
                }
                return null;
            }
            return parameter;
        }

        public static OracleParameter AddOutputParameter<T>(this OracleCommand command, string parameterName)
        {
            return AddOutputParameterInternal<T>(command, parameterName, ParameterDirection.Output);
        }

        public static OracleParameter AddReturnParameter<T>(this OracleCommand command, string parameterName, OracleDbType? oracleType = null)
        {
            return AddOutputParameterInternal<T>(command, parameterName, ParameterDirection.ReturnValue, oracleType);
        }


        private static OracleParameter AddOutputParameterInternal<T>(OracleCommand command, string parameterName, ParameterDirection parameterDirection, OracleDbType? oracleDbType = null)
        {
            var netType = typeof (T);
            var oracleType = oracleDbType ?? TypeMapper.GetOracleType(netType);
            //Не трогать код без необходимости. Здесь замешана магия, вернее баг ODP.Net.
            //Следующая строка не работает для OracleDbType.Varchar2(ORA-06502: character string buffer too small):
            //var parameter = new OracleParameter(parameterName, TypeMapper.GetOracleType(netType), 4000, ParameterDirection.Output);
            //А вот эта работает:
            var parameter = new OracleParameter(parameterName, oracleType) { Direction = parameterDirection };
            if (oracleType == OracleDbType.Varchar2)
            {
                parameter.Size = 4000;
            }
            if (oracleType == OracleDbType.Raw)
            {
                parameter.Size = 16;
            }
            command.Parameters.Add(parameter);
            return parameter;
        }

        public static void DisposeParameters(this OracleCommand command)
        {
            foreach (var parameter in command.Parameters)
            {
                ((OracleParameter)parameter).Dispose();
            }
        }
      
    }
}
