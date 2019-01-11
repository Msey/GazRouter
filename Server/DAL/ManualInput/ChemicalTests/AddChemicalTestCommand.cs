using GazRouter.DAL.Core;
using GazRouter.DTO.ManualInput.ChemicalTests;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.ManualInput.ChemicalTests
{
    public class AddChemicalTestCommand: CommandScalar<AddChemicalTestParameterSet, int>
    {
        public AddChemicalTestCommand(ExecutionContext context)
            : base(context)
        {
            IsStoredProcedure = true;
        }

        protected override void BindParameters(OracleCommand command, AddChemicalTestParameterSet parameters)
        {
            OutputParameter = command.AddReturnParameter<int>("p_chemical_test_id");

            command.AddInputParameter("p_meas_point_id", parameters.MeasPointId);
            command.AddInputParameter("p_test_date", parameters.TestDate);

            if (parameters.DewPoint.HasValue)
                command.AddInputParameter("p_dew_point", parameters.DewPoint);

            if (parameters.DewPointHydrocarbon.HasValue)
                command.AddInputParameter("p_dew_point_hydrocarbon", parameters.DewPointHydrocarbon);

            if (parameters.ContentNitrogen.HasValue)
                command.AddInputParameter("p_content_nitrogen", parameters.ContentNitrogen);

            if (parameters.ConcentrSourSulfur.HasValue)
                command.AddInputParameter("p_concentr_sour_sulfur", parameters.ConcentrSourSulfur);

            if (parameters.ConcentrHydrogenSulfide.HasValue)
                command.AddInputParameter("p_concentr_hydrogen_sulfide", parameters.ConcentrHydrogenSulfide);

            if (parameters.ContentCarbonDioxid.HasValue)
                command.AddInputParameter("p_content_carbon_dioxid", parameters.ContentCarbonDioxid);

            if (parameters.Density.HasValue)
                command.AddInputParameter("p_density", parameters.Density);

            if (parameters.CombHeatLow.HasValue)
                command.AddInputParameter("p_combustion_heat_low", parameters.CombHeatLow);

            command.AddInputParameter("p_user_name", Context.UserIdentifier);
		}

        protected override string GetCommandText(AddChemicalTestParameterSet parameters)
        {
            return "P_CHEMICAL_TEST.AddF";
        }
    }
}