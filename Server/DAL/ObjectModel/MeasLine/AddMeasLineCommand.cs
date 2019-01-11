using System;
using GazRouter.DAL.Core;
using GazRouter.DTO.ObjectModel.MeasLine;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.ObjectModel.MeasLine
{

    public class AddMeasLineCommand : CommandScalar<AddMeasLineParameterSet, Guid>
    {
        public AddMeasLineCommand(ExecutionContext context) : base(context)
        {
            IsStoredProcedure = true;
        }

        protected override void BindParameters(OracleCommand command, AddMeasLineParameterSet parameters)
        {
            if (parameters.Id.HasValue)
            {
                command.AddInputParameter("p_entity_id", parameters.Id.Value);
            }

            OutputParameter = command.AddReturnParameter<Guid>("entity_id");
            command.AddInputParameter("p_entity_name", parameters.Name);
            command.AddInputParameter("p_status", parameters.Status);
            command.AddInputParameter("p_hidden", parameters.IsHidden);
            command.AddInputParameter("p_is_virtual", parameters.IsVirtual);
            command.AddInputParameter("p_meas_station_id", parameters.ParentId);
            command.AddInputParameter("p_pipeline_id", parameters.PipelineId);
            command.AddInputParameter("p_kilometer_conn", parameters.KmOfConn);
			command.AddInputParameter("p_sort_order", parameters.SortOrder);
            command.AddInputParameter("p_user_name", Context.UserIdentifier);
            command.AddInputParameter("p_bal_name", parameters.BalanceName);
        }

        protected override string GetCommandText(AddMeasLineParameterSet parameters)
        {
            return "rd.P_MEAS_LINE.AddF";
        }
    }

}

