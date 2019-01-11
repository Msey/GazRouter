using GazRouter.DAL.Core;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.ObjectModel.Entities.Urls
{

	public class RemoveEntityUrlCommand : CommandNonQuery<int>
	{
        public RemoveEntityUrlCommand(ExecutionContext context)
			: base(context)
        {
            IsStoredProcedure = true;
        }

        protected override void BindParameters(OracleCommand command, int parameters)
        {
            command.AddInputParameter("p_entity_ext_urls_id", parameters);
        }

        protected override string GetCommandText(int parameters)
        {
            return "rd.P_ENTITY_EXT_URL.Remove";
        }

        
    }

}

