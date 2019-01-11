using GazRouter.DAL.Core;

namespace GazRouter.DAL.ObjectModel.MeasStations
{
  

    public class DeleteMeasStationCommand : DeleteEntityCommand
    {
        public DeleteMeasStationCommand(ExecutionContext context)
            : base(context)
        {
			IntegrityConstraints.Add("(RDI.FK_MEAS_LINES_STATION)", "Измерительная станция не может быть удалена так как содержит измерительные линии");
        }

        protected override string Package
        {
            get { return "P_SITE"; }
        }
    }

}