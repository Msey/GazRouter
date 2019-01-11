using GazRouter.DAL.Core;
using GazRouter.Log;

namespace TestBase.Infra
{
    public sealed class ExecutionContextTest : ExecutionContext
    {
        public ExecutionContextTest(string userIdentifier, string connectionString, string appHostName, MyLogger logger)
            : base(userIdentifier, connectionString, appHostName, logger)
        {
			
        }

        protected override void EndTransaction()
        {
            Transaction.Rollback();
            base.EndTransaction();
        }
    }
}