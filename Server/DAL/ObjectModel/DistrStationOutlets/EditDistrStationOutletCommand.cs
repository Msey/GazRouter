using GazRouter.DAL.Core;
using GazRouter.DTO.ObjectModel.DistrStationOutlets;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.ObjectModel.DistrStationOutlets
{
    public class EditDistrStationOutletCommand : CommandNonQuery<EditDistrStationOutletParameterSet>
    {
        public EditDistrStationOutletCommand(ExecutionContext context) : base(context)
        {
            IsStoredProcedure = true;
        }

        protected override void BindParameters(OracleCommand command, EditDistrStationOutletParameterSet parameters)
        {
            command.AddInputParameter("p_entity_id", parameters.Id);
            command.AddInputParameter("p_distr_station_id", parameters.ParentId);
            command.AddInputParameter("p_entity_name", parameters.Name);
            command.AddInputParameter("p_hidden", parameters.IsHidden);
            command.AddInputParameter("p_is_virtual", parameters.IsVirtual);
            command.AddInputParameter("p_capacity_rated", parameters.CapacityRated);
            command.AddInputParameter("p_pressure_rated", parameters.PressureRated);
            command.AddInputParameter("p_gas_consumer_id", parameters.ConsumerId);
            command.AddInputParameter("p_sort_order", parameters.SortOrder);
            command.AddInputParameter("p_user_name", Context.UserIdentifier);
        }

        protected override string GetCommandText(EditDistrStationOutletParameterSet parameters)
        {
            return "rd.P_DISTR_STATION_OUTLET.Edit";
        }

    }
}