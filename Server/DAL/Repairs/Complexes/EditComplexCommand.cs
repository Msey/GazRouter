using GazRouter.DAL.Core;
using GazRouter.DTO.Repairs.Complexes;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.Repairs.Complexes
{
    public class EditComplexCommand : CommandNonQuery<EditComplexParameterSet>
    {
        public EditComplexCommand(ExecutionContext context)
            : base(context)
        {
            IsStoredProcedure = true;
        }

        protected override void BindParameters(OracleCommand command, EditComplexParameterSet parameters)
        {
            command.AddInputParameter("p_complex_id", parameters.Id);
            command.AddInputParameter("p_complex_type", parameters.ComplexName);
            command.AddInputParameter("p_start_date", parameters.StartDate);
            command.AddInputParameter("p_end_date", parameters.EndDate);
            command.AddInputParameter("p_user_name", Context.UserIdentifier);
        }

        protected override string GetCommandText(EditComplexParameterSet parameters)
        {
            return "P_PPW_COMPLEX.Edit";
        }
    }
}
