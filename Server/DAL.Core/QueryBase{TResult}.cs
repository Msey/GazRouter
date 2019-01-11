using System;
using Oracle.ManagedDataAccess.Client;
namespace GazRouter.DAL.Core
{
    public abstract class QueryBase<TResult> : CommandBase
    {
        protected QueryBase(ExecutionContext context)
            : base(context)
        {
        }

        public abstract TResult Execute();

        protected TResult SafeExecute(string query, Func<OracleCommand, TResult> func)
        {
            using (var comm = Context.CreateCommand(query, IsStoredProcedure))
            {
                try
                {
                    return func(comm);
                }
                catch (OracleException ex)
                {
                    Context.Rollback();
                    TryThrowIntegrityConstraintExceptions(ex);
                    throw;
                }
                catch (Exception)
                {
                    Context.Rollback();
                    throw;
                }
                finally
                {
                    comm.DisposeParameters();
                }
            }
        }
    }
}