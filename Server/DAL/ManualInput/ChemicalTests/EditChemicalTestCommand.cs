using GazRouter.DAL.Core;
using GazRouter.DTO.ManualInput.ChemicalTests;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.ManualInput.ChemicalTests
{
    public class EditChemicalTestCommand: CommandNonQuery<EditChemicalTestParameterSet>
    {
        public EditChemicalTestCommand(ExecutionContext context)
            : base(context)
        {
            IsStoredProcedure = true;
        }

        protected override void BindParameters(OracleCommand command, EditChemicalTestParameterSet parameters)
		{
            command.AddInputParameter("p_chemical_test_id", parameters.ChemicalTestId);
            command.AddInputParameter("p_test_date", parameters.TestDate);
            command.AddInputParameter("p_dew_point", parameters.DewPoint);
            command.AddInputParameter("p_dew_point_hydrocarbon", parameters.DewPointHydrocarbon);
            command.AddInputParameter("p_content_nitrogen", parameters.ContentNitrogen);
            command.AddInputParameter("p_concentr_sour_sulfur", parameters.ConcentrSourSulfur);
            command.AddInputParameter("p_concentr_hydrogen_sulfide", parameters.ConcentrHydrogenSulfide);
            command.AddInputParameter("p_content_carbon_dioxid", parameters.ContentCarbonDioxid);
            command.AddInputParameter("p_density", parameters.Density);
            command.AddInputParameter("p_combustion_heat_low", parameters.CombHeatLow);
            command.AddInputParameter("p_user_name ", Context.UserIdentifier);
		}

        protected override string GetCommandText(EditChemicalTestParameterSet parameters)
        {
            return "P_CHEMICAL_TEST.Edit";
        }
    }
}