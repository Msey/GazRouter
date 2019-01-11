using GazRouter.DAL.Core;

namespace GazRouter.DAL.ObjectModel.DistrStationOutlets
{
    public class DeleteDistrStationOutletCommand : DeleteEntityCommand
    {
        public DeleteDistrStationOutletCommand(ExecutionContext context)
            : base(context)
        {

        }

        protected override string Package
        {
            get { return "P_DISTR_STATION_OUTLET"; }
        }
    }
}