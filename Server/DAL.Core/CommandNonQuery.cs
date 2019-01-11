using System;
using Oracle.ManagedDataAccess.Client;
namespace GazRouter.DAL.Core
{
    public abstract class CommandNonQuery<TParameters> : CommandBase<TParameters>
    {
        protected CommandNonQuery(ExecutionContext context)
            : base(context)
        {
        }
        public void Execute(TParameters parameters)
        {
            var query = GetCommandTextAndLog(parameters);
            SafeExecute(parameters, query, (comm, parameters1) =>
                                               {
                                                   BindParameters(comm, parameters1);
                                                   comm.ExecuteNonQuery();
                                               });
        }

        private void SafeExecute(TParameters parameters, string query, Action<OracleCommand, TParameters> action)
        {
            using (var comm = Context.CreateCommand(query, IsStoredProcedure))
            {
                try
                {
                    action(comm, parameters);
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

    public abstract class CommandNonQuery : CommandBase
    {
        protected CommandNonQuery(ExecutionContext context)
            : base(context)
        {

        }

        public void Execute()
        {
            var query = GetCommandTextAndLog();
            SafeExecute(query, comm =>
            {
                BindParameters(comm);
                comm.ExecuteNonQuery();
            });
        }

        private void SafeExecute(string query, Action<OracleCommand> action)
        {
            using (var comm = Context.CreateCommand(query, IsStoredProcedure))
            {
                try
                {
                    action(comm);
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