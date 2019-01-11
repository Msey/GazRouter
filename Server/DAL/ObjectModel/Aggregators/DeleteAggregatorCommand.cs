using GazRouter.DAL.Core;

namespace GazRouter.DAL.ObjectModel.Aggregators
{
    public class DeleteAggregatorCommand : DeleteEntityCommand
    {
        public DeleteAggregatorCommand(ExecutionContext context)
            : base(context)
        {
        }

        protected override string Package => "P_AGGREGATOR";
    }
}