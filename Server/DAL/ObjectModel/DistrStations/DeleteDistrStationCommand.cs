using GazRouter.DAL.Core;

namespace GazRouter.DAL.ObjectModel.DistrStations
{
    public class DeleteDistrStationCommand : DeleteEntityCommand
    {
        public DeleteDistrStationCommand(ExecutionContext context)
            : base(context)
        {
			IntegrityConstraints.Add("(RDI.FK_OUTLETS_STATION)", "ГРС не может быть удалена так как содержит выходы");
			IntegrityConstraints.Add("(RDI.FK_GAS_CONSUMERS_DISTR_STATION)", "ГРС не может быть удалена так как содержит потребителей");
        }

    protected override string Package
        {
            get { return "P_DISTR_STATION"; }
        }
    }
}