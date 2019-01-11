using GazRouter.DAL.Core;

namespace GazRouter.DAL.ObjectModel.CompStations
{
    public class DeleteCompStationCommand : DeleteEntityCommand
    {
        public DeleteCompStationCommand(ExecutionContext context)
            : base(context)
        {
			IntegrityConstraints.Add("(RDI.FK_COMP_SHOPS_STATION)", "Компрессорная станция не может быть удалена так как содержит Компрессорные цеха");
        }

        protected override string Package
        {
            get { return "P_COMP_STATION"; }
        }
    }
}