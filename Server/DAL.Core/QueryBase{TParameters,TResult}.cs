using System;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.Core
{
    public abstract class QueryBase<TParameters, TResult> : CommandBase<TParameters>
    {
        protected QueryBase(ExecutionContext context)
            : base(context)
        {
        }

        public abstract TResult Execute(TParameters parameters);

        protected TResult SafeExecute(TParameters parameters, string query,
            Func<OracleCommand, TParameters, TResult> func)
        {
            using (var comm = Context.CreateCommand(query, IsStoredProcedure))
            {
                try
                {
                    return func(comm, parameters);
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