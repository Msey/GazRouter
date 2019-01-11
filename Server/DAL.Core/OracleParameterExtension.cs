using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.Core
{
    public static class OracleParameterExtension
    {
        public static TResult GetValue<TResult>(this OracleParameter parameter)
        {
            return TypeMapper.GetValueFromParameter<TResult>(parameter);
        }
    }
}
