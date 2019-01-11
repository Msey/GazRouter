using GazRouter.DAL.Core;
using GazRouter.DTO.DataExchange.ASUTPImport;
using GazRouter.DTO.DataExchange.DataSource;
using GazRouter.DTO.Dictionaries.PeriodTypes;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.DataExchange.ASUTPImport
{
    public class ASUTPImportCommand : CommandNonQuery<ASUTPImportParameterSet>
    {
        public ASUTPImportCommand(ExecutionContext context)
            : base(context)
        {
            IsStoredProcedure = true;
        }

        protected override void BindParameters(OracleCommand command, ASUTPImportParameterSet parameters)
        {
            command.AddInputParameter("p_series_date",
                parameters.PeriodType == PeriodType.Twohours ? parameters.Timestamp : parameters.Timestamp.Date);
            command.AddInputParameter("p_period_type_id", parameters.PeriodType);
            command.AddInputParameter("p_user_name", Context.UserIdentifier);
        }


        protected override string GetCommandText(ASUTPImportParameterSet parameters)
        {
            return "im.P_IMPEX.ImportSeriesFull";

        }

    }
}