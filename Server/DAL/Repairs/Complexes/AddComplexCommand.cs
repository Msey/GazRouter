using GazRouter.DAL.Core;
using GazRouter.DTO.Repairs.Complexes;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.Repairs.Complexes
{
    public class AddComplexCommand : CommandScalar<AddComplexParameterSet, int>
    {
        public AddComplexCommand(ExecutionContext context)
            : base(context)
        {
            IsStoredProcedure = true;
        }

        protected override void BindParameters(OracleCommand command, AddComplexParameterSet parameters)
        {
            OutputParameter = command.AddReturnParameter<int>("p_complex_id");
            command.AddInputParameter("p_complex_type", parameters.ComplexName);
            command.AddInputParameter("p_start_date", parameters.StartDate);
            command.AddInputParameter("p_end_date", parameters.EndDate);
            command.AddInputParameter("p_is_local", parameters.IsLocal);
            command.AddInputParameter("p_system_id", parameters.SystemId);
            command.AddInputParameter("p_user_name", Context.UserIdentifier);
        }

        protected override string GetCommandText(AddComplexParameterSet parameters)
        {
            return "P_PPW_COMPLEX.AddF";
        }
    }
}
