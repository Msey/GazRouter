using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GazRouter.DAL.Core
{
    public class MultipleCommand : CommBase
    {
        private readonly IEnumerable<Action<ExecutionContext>> _actions;

        public MultipleCommand(ExecutionContext context, IEnumerable<Action<ExecutionContext>> actions) : base(context)
        {
            _actions = actions;
        }

        public void Execute()
        {
            foreach (var action in _actions)
            {
                try
                {
                    action(Context);
                    Context.Transaction.Commit();
                }
                catch (Exception e)
                {
                    Context.Logger?.WriteException(e, e.Message);
                    OnException(e);
                }
                finally
                {
                    Context.Transaction = Context.Connection.BeginTransaction();
                }
            }
        }

        public Action<Exception> OnException { get; set; }
    }
}
