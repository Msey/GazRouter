using System.Collections.Generic;
using System.Linq;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.Core
{
    public abstract class CommandBase<TParameters> : CommBase
    {
        protected CommandBase(ExecutionContext context) : base(context)
        {
        }

        protected virtual void BindParameters(OracleCommand command, TParameters parameters)
        { }

        protected abstract string GetCommandText(TParameters parameters);

        protected string GetCommandTextAndLog(TParameters parameters)
        {
            string query = GetCommandText(parameters);
            Context.Logger.WriteQuery(query, parameters);
            return query;
        }
    }

    public abstract class CommandBase : CommBase
    {
        protected CommandBase(ExecutionContext context)
            : base(context)
        {
        }

        protected string GetCommandTextAndLog()
        {
            string query = GetCommandText();
            Context.Logger.WriteQuery(query, null);
            return query;
        }

        protected virtual void BindParameters(OracleCommand command)
        { }

        protected abstract string GetCommandText();
    }

    public abstract class CommBase
    {
        protected CommBase(ExecutionContext context)
        {
            Context = context;
            IntegrityConstraints.Add("ORA-20101: ORA-02292", "обнаружена порожденная запись");
        }

        protected bool IsStoredProcedure { set; get; }
        protected ExecutionContext Context { get; private set; }

        protected Dictionary<string, string> IntegrityConstraints = new Dictionary<string, string>();

        protected void TryThrowIntegrityConstraintExceptions(OracleException ex)
        {
            foreach (var constr in IntegrityConstraints.Where(constr => ex.Message.Contains(constr.Key)))
            {
                throw new IntegrityConstraintException(constr.Value, ex, constr.Key);
            }
        }
    }
}
