using System;
using GazRouter.DAL.Core;
using GazRouter.DTO.SeriesData.PropertyValues;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.DataExchange.ExchangeProperty
{
    public class SetExchangePropertyValueCommand : CommandNonQuery<SetPropertyValueParameterSet>
    {
        public SetExchangePropertyValueCommand(ExecutionContext context)
            : base(context)
        {
            IsStoredProcedure = true;
        }

        protected override void BindParameters(OracleCommand command, SetPropertyValueParameterSet parameters)
        {
            command.AddInputParameter("p_property_type_id", parameters.PropertyTypeId);
            command.AddInputParameter("p_entity_id", parameters.EntityId);
            command.AddInputParameter("p_series_id", parameters.SeriesId);
            command.AddInputParameter("p_annotation", parameters.Annotation);
            command.AddInputParameter("p_source_type", (int)SourceType.CustomExchange);

            if (parameters.Value is double)
                command.AddInputParameter("p_value_numb", (double)parameters.Value);

            if (parameters.Value is int)
                command.AddInputParameter("p_value_numb", (int)parameters.Value);
            
            if (parameters.Value is DateTime)
                command.AddInputParameter("p_value_date", (DateTime)parameters.Value);

            if (parameters.Value is string)
                command.AddInputParameter("p_value_char", (string)parameters.Value);
            
            
            command.AddInputParameter("p_user_name", Context.UserIdentifier);
        }

        protected override string GetCommandText(SetPropertyValueParameterSet parameters)
        {
            return "rd.P_VALUE.Set_VALUE";
        }
    }
}