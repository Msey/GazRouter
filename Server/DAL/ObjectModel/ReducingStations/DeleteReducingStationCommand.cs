using GazRouter.DAL.Core;

namespace GazRouter.DAL.ObjectModel.ReducingStations
{
    public class DeleteReducingStationCommand : DeleteEntityCommand
    {
        public DeleteReducingStationCommand(ExecutionContext context)
            : base(context)
        {
			
        }

        protected override string Package
        {
            get { return "P_REDUCING_STATION"; }
        }
    }
}
