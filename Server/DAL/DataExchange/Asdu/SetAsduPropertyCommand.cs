using System;
using GazRouter.DAL.Core;
using GazRouter.DTO.DataExchange.Asdu;
using GazRouter.DTO.DataExchange.ExchangeProperty;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.DataExchange.Asdu
{
    public class SetAsduPropertyCommand : CommandNonQuery<SetAsduPropertyParameterSet>
    {
        public SetAsduPropertyCommand(ExecutionContext context)
            : base(context)
        {
            IsStoredProcedure = true;
        }

        protected override void BindParameters(OracleCommand command, SetAsduPropertyParameterSet parameters)
        {
            command.AddInputParameter("p_parameter_gid", parameters.ParameterGid);
            command.AddInputParameter("p_entity_id", parameters.EntityId);
            command.AddInputParameter("p_property_type_id", parameters.PropertyTypeId);
            
            command.AddInputParameter("p_user_name", Context.UserIdentifier);
        }


        protected override string GetCommandText(SetAsduPropertyParameterSet parameters)
        {
            return "rd.P_2_ASDU.Set_PROPERTY";

        }

    }
}