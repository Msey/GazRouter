using System;
using GazRouter.DAL.Core;
using GazRouter.DTO.ObjectModel.Inconsistency;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.ObjectModel.Inconsistency
{
	public class AddInconsistencyCommand : CommandScalar<AddInconsistencyParameterSet, Guid>
    {
		public AddInconsistencyCommand(ExecutionContext context)
            : base(context)
        {
            IsStoredProcedure = true;
        }

		protected override void BindParameters(OracleCommand command, AddInconsistencyParameterSet parameters)
        {
			OutputParameter = command.AddReturnParameter<Guid>("P_INCONSISTENCY_ID");
			command.AddInputParameter("P_ENTITY_ID", parameters.EntityId);
			command.AddInputParameter("P_INCONSISTENCY_TYPE_ID", parameters.InconsistencyTypeId);
			command.AddInputParameter("P_USER_NAME", Context.UserIdentifier);
        }

		protected override string GetCommandText(AddInconsistencyParameterSet parameters)
        {
			return "rd.P_INCONSISTENCY.AddF";
        }

    }
    
}