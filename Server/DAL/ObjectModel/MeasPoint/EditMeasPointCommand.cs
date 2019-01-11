using GazRouter.DAL.Core;
using GazRouter.DTO.ObjectModel.MeasPoint;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.ObjectModel.MeasPoint
{
    public class EditMeasPointCommand : CommandNonQuery<EditMeasPointParameterSet>
    {
        public EditMeasPointCommand(ExecutionContext context)
            : base(context)
        {
            IsStoredProcedure = true;
        }

		protected override void BindParameters(OracleCommand command, EditMeasPointParameterSet parameters)
        {
            command.AddInputParameter("p_entity_id", parameters.Id);
            command.AddInputParameter("p_chromatogr_consumption_rate", parameters.ChromatographConsumptionRate);
            command.AddInputParameter("p_chromatogr_test_time", parameters.ChromatographTestTime);
            command.AddInputParameter("p_user_name", Context.UserIdentifier);
        }

        protected override string GetCommandText(EditMeasPointParameterSet parameters)
        {
            return "rd.P_MEAS_POINT.Edit";
        }
    }
}

