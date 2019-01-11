using GazRouter.DAL.Core;
using GazRouter.DTO.Balances.Routes;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.Balances.Routes
{
    public class AddRouteSectionCommand : CommandScalar<AddRouteSectionParameterSet, int>
    {
        public AddRouteSectionCommand(ExecutionContext context)
            : base(context)
		{
		    IsStoredProcedure = true;
		}

        protected override void BindParameters(OracleCommand command, AddRouteSectionParameterSet parameters)
        {
            OutputParameter = command.AddReturnParameter<int>("p_route_section_id");
            command.AddInputParameter("p_route_id", parameters.RouteId);
            command.AddInputParameter("p_pipeline_id", parameters.PipelineId);
            command.AddInputParameter("p_kilometer_end", parameters.KilometerEnd);
            command.AddInputParameter("p_kilometer_start", parameters.KilometerStart);
            command.AddInputParameter("p_sort_order", parameters.SortOrder);
            command.AddInputParameter("P_USER_NAME", Context.UserIdentifier);
        }

        protected override string GetCommandText(AddRouteSectionParameterSet parameters)
		{
            return "P_ROUTE_SECTION.AddF";
		}

    }
}