using GazRouter.DAL.Core;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.ObjectModel.PipelineConns
{

	public class DeletePipelineConnCommand : CommandNonQuery<int>
	{
		public DeletePipelineConnCommand(ExecutionContext context) : base(context) {}
		
	    protected override void BindParameters(OracleCommand command, int parameters)
	    {
            command.AddInputParameter("p1", parameters);
            command.AddInputParameter("user", Context.UserIdentifier);
	    }
		
		protected override string GetCommandText(int parameters)
	    {
            return @"Begin    
 rd.P_PIPELINE_CONNS.Remove
 (
                         p_pipeline_conn_id => :p1                         
                         ,p_user_name   => :user
 );  
  end;";
	    }
	}

}