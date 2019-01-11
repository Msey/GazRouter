using GazRouter.DAL.Core;
using GazRouter.DTO.Repairs.Agreed;
using GazRouter.DTO.Repairs.Plan;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GazRouter.DAL.Repairs.Agreement
{
    public class AddAgreementCommand : CommandScalar<AddEditAgreedRepairRecordParameterSet, int>
    {
        public AddAgreementCommand(ExecutionContext context)
            : base(context)
        {
            IsStoredProcedure = true;
        }

        protected override void BindParameters(OracleCommand command, AddEditAgreedRepairRecordParameterSet parameters)
        {
            OutputParameter = command.AddReturnParameter<int>("p_agreed_repair_record_id");
            command.AddInputParameter("p_repair_id", parameters.RepairID);
            command.AddInputParameter("p_creation_date", DateTime.Now);
            command.AddInputParameter("p_agreed_user_id", parameters.AgreedUserId);
            command.AddInputParameter("p_real_agreed_user_id", parameters.RealAgreedUserId);
            command.AddInputParameter("p_agreed_date", parameters.AgreedDate);
            command.AddInputParameter("p_description", parameters.Comment);
            command.AddInputParameter("p_agreed_result", parameters.AgreedResult);
            command.AddInputParameter("p_user_name", Context.UserIdentifier);
        }

        protected override string GetCommandText(AddEditAgreedRepairRecordParameterSet parameters)
        {
            return "P_AGREED_REPAIR_RECORD.AddF";
        }
    }
}
