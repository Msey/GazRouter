using GazRouter.DAL.Core;
using GazRouter.DTO.SystemVariables;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.SystemVariables
{
    public class EditIusVariableCommand : CommandNonQuery<IusVariableParameterSet>
    {
        public EditIusVariableCommand(ExecutionContext context) : base(context)
        {
            IsStoredProcedure = true;
        }
        
        protected override void BindParameters(OracleCommand command, IusVariableParameterSet parameters)
        {
            command.AddInputParameter("p_code", parameters.Name);
            command.AddInputParameter("p_varchar", parameters.Value);
        }

        protected override string GetCommandText(IusVariableParameterSet parameters)
        {
            return "rd.P_SYS_PARM.SetVALUE";
        }
    }
}