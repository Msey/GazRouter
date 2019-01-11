using GazRouter.DAL.Core;

namespace GazRouter.DAL.ObjectModel.Pipelines
{

    public class DeletePipelineCommand : DeleteEntityCommand
	{
		public  DeletePipelineCommand(ExecutionContext context) : base(context)
		{
			IntegrityConstraints.Add("(RDI.FK_PIPELINE_CONN_PIPELINE)", "Газопровод не может быть удален, т.к. содержит подключения. Для удаления газопровода удалите все подключения этого газопровода.");
            IntegrityConstraints.Add("(RDI.FK_VALVES_PIPELINE)", "Газопровод не может быть удален, т.к. содержит крановые узлы. Для удаления газопровода удалите все крановые узлы на этом газопроводе.");
            IntegrityConstraints.Add("(RDI.FK_MEAS_LINES_PIPELINE)", "Газопровод не может быть удален, т.к. содержит измерительные линии ГИС. Для удаления газопровода удалите замерные линии или перенесите их на другой газопровод.");
            IntegrityConstraints.Add("(RDI.FK_COMP_SHOPS_PIPELINE)", "Газопровод не может быть удален, т.к. существуют КЦ подключенные к этому газопроводу. Для удаления газопровода удалите КЦ или перенесите их на другой газопровод.");
            IntegrityConstraints.Add("(RDI.FK_REDUCING_STATIONS_PIPELINE)", "Газопровод не может быть удален, т.к. содержит ПРГ. Для удаления газопровода удалите ПРГ или перенесите их на другой газопровод.");
            IntegrityConstraints.Add("(RDI.FK_SEGMENTS_BY_SITES_PIPELINE)", "Газопровод не может быть удален, т.к. содержит сегменты по ЛПУ. Для удаления газопровода удалите сегменты этого газопровода.");
            IntegrityConstraints.Add("(RDI.FK_SEGMENTS_BY_PRESS_PIPELINE)", "Газопровод не может быть удален, т.к. содержит сегменты по давлению. Для удаления газопровода удалите сегменты этого газопровода.");
            IntegrityConstraints.Add("(RDI.FK_SEG_BY_DIAMETR_PIPELINE)", "Газопровод не может быть удален, т.к. содержит сегменты по диаметру. Для удаления газопровода удалите сегменты этого газопровода.");
            IntegrityConstraints.Add("(RDI.FK_SEGMENTS_BY_GROUPS_PIPELINE)", "Газопровод не может быть удален, т.к. содержит сегменты по группам. Для удаления газопровода удалите сегменты этого газопровода.");
        }
		

        protected override string Package
        {
            get { return "P_PIPELINE"; }
        }
	}

}

