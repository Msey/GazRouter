using System;
using GazRouter.DAL.Core;
using GazRouter.DTO.ObjectModel.MeasPoint;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.ObjectModel.MeasPoint
{
    public class AddMeasPointCommand : CommandScalar<AddMeasPointParameterSet, Guid>
    {
        public AddMeasPointCommand(ExecutionContext context) : base(context)
        {
            IsStoredProcedure = true;
        }

		protected override void BindParameters(OracleCommand command, AddMeasPointParameterSet parameters)
        {
            OutputParameter = command.AddReturnParameter<Guid>("entity_id");
            command.AddInputParameter("p_entity_name", parameters.Name);
            command.AddInputParameter("p_meas_line_id", parameters.MeasLineId);
            command.AddInputParameter("p_comp_shop_id", parameters.CompShopId);
            command.AddInputParameter("p_distr_station_id", parameters.DistrStationId);
            command.AddInputParameter("p_chromatogr_consumption_rate", parameters.ChromatographConsumptionRate);
            command.AddInputParameter("p_chromatogr_test_time", parameters.ChromatographTestTime);
            command.AddInputParameter("p_user_name", Context.UserIdentifier);
        }

		protected override string GetCommandText(AddMeasPointParameterSet parameters)
        {
            return "rd.P_MEAS_POINT.AddF";
        }
    }
}

