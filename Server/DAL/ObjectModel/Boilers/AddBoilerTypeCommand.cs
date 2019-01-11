using GazRouter.DAL.Core;
using GazRouter.DTO.ObjectModel.Boilers;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.ObjectModel.Boilers
{
    public class AddBoilerTypeCommand : CommandScalar<AddBoilerTypeParameterSet, int>
    {
        public AddBoilerTypeCommand(ExecutionContext context)
            : base(context)
        {
            IsStoredProcedure = true;
        }

        protected override void BindParameters(OracleCommand command, AddBoilerTypeParameterSet parameters)
        {
            OutputParameter = command.AddReturnParameter<int>("p_boiler_type_id");
            command.AddInputParameter("p_boiler_type_name", parameters.Name);
            command.AddInputParameter("p_description", parameters.Description);
            command.AddInputParameter("p_rated_heating_efficiency", parameters.RatedHeatingEfficiency);
            command.AddInputParameter("p_rated_efficiency_factor", parameters.RatedEfficiencyFactor);
            command.AddInputParameter("p_is_small", parameters.IsSmall);
            command.AddInputParameter("p_group_name", parameters.GroupName);
            command.AddInputParameter("p_heating_area", parameters.HeatingArea);
            command.AddInputParameter("p_user_name", Context.UserIdentifier);
        }

        protected override string GetCommandText(AddBoilerTypeParameterSet parameters)
        {
            return "rd.p_Boiler_Type.AddF";
        }
    }
}
