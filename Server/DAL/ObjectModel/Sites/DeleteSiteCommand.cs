using GazRouter.DAL.Core;

namespace GazRouter.DAL.ObjectModel.Sites
{
    public class DeleteSiteCommand : DeleteEntityCommand
    {
        public DeleteSiteCommand(ExecutionContext context)
            : base(context)
        {
			IntegrityConstraints.Add("(RDI.FK_COMP_STATIONS_SITE)", "ЛПУ не может быть удалена так как содержит компрессорные станции");
			IntegrityConstraints.Add("(RDI.FK_MEAS_STATIONS_SITE)", "ЛПУ не может быть удалена так как содержит измерительные станции");
            IntegrityConstraints.Add("(RDI.FK_REDUCING_STATIONS_SITE)", "ЛПУ не может быть удалена так как содержит пункты редуцирования");
            IntegrityConstraints.Add("(RDI.FK_SEGMENTS_BY_SITES_SITE)", "ЛПУ не может быть удалена так как содержит сегменты");
            IntegrityConstraints.Add("(RDI.FK_DISTR_STATIONS_SITE)", "ЛПУ не может быть удалена так как содержит ГРС");
        }

        protected override string Package
        {
            get { return "P_SITE"; }
        }
    }
}