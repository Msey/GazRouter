using GazRouter.DAL.Core;
using GazRouter.DTO.Alarms;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.Alarms
{
    public class AddAlarmCommand: CommandScalar<AddAlarmParameterSet, int>
    {
        public AddAlarmCommand(ExecutionContext context)
            : base(context)
        {
            IsStoredProcedure = true;
        }

        protected override void BindParameters(OracleCommand command, AddAlarmParameterSet parameters)
		{
            OutputParameter = command.AddReturnParameter<int>("alarm_id");
            command.AddInputParameter("p_alarm_type_id", parameters.AlarmTypeId);
            command.AddInputParameter("p_entity_id", parameters.EntityId);
            command.AddInputParameter("p_property_type_id", parameters.PropertyTypeId);
            command.AddInputParameter("p_period_type_id", parameters.PeriodTypeId);
            command.AddInputParameter("p_setting", parameters.Setting);
            command.AddInputParameter("p_description", parameters.Description);
            command.AddInputParameter("p_activation_date", parameters.ActivationDate);
            command.AddInputParameter("p_expiration_date", parameters.ExpirationDate);

            command.AddInputParameter("p_user_name", Context.UserIdentifier);
		}

        protected override string GetCommandText(AddAlarmParameterSet parameters)
        {
            return "rd.P_ALARM.AddF";
        }
    }
}