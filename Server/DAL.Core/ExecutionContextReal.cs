using GazRouter.Log;

namespace GazRouter.DAL.Core
{
    public sealed class ExecutionContextReal : ExecutionContext
    {
        public ExecutionContextReal(string userIdentifier, string connectionString, string appHostName, MyLogger logger)
            : base(userIdentifier, connectionString, appHostName, logger)
        {
        }

        protected override void EndTransaction()
        {
            Transaction.Commit();
            base.EndTransaction();
            Logger.WriteContextAction(ContextId, "commited");
        }
    }
}