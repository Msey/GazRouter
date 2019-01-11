using GazRouter.DAL.Core;

namespace GazRouter.DAL.ObjectModel.CoolingStations
{
    public class DeleteCoolingStationCommand : DeleteEntityCommand
    {
        public DeleteCoolingStationCommand(ExecutionContext context)
            : base(context)
        {}

        protected override string Package
        {
            get { return "P_COOLING_STATION"; }
        }
    }
}