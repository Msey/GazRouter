using System;
using GazRouter.DAL.Core;
using GazRouter.DTO;
using GazRouter.DTO.Dictionaries.PeriodTypes;
using GazRouter.DTO.SeriesData.Series;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.SeriesData.Series
{
    public class AddSeriesCommand : CommandScalar<AddSeriesParameterSet, int>
    {
        public AddSeriesCommand(ExecutionContext context)
            : base(context)
        {
            IsStoredProcedure = true;
        }

        protected override void BindParameters(OracleCommand command, AddSeriesParameterSet parameters)
        {
            OutputParameter = command.AddReturnParameter<int>("p_series_id");

            if (parameters.PeriodTypeId == PeriodType.Twohours)
                command.AddInputParameter("p_key_date", parameters.KeyDate);

            if (parameters.PeriodTypeId == PeriodType.Day)
                command.AddInputParameter("p_key_date", new DateTime(parameters.Year, parameters.Month, parameters.Day));
            

            command.AddInputParameter("p_period_type_id", parameters.PeriodTypeId);
            command.AddInputParameter("p_description", parameters.Description);
            command.AddInputParameter("p_cre_user", Context.UserIdentifier);
        }

        protected override string GetCommandText(AddSeriesParameterSet parameters)
        {
            return "rd.P_VALUE.AddF_SERIES";
        }

    }

}