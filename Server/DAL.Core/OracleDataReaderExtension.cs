using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.Core
{
    public static class OracleDataReaderExtension
    {
        public static T GetValue<T>(this OracleDataReader reader, string columnName)
        {
            return TypeMapper.GetValueFromReader<T>(reader, columnName);
        }
    }
}
