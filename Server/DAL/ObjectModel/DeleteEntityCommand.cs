using GazRouter.DAL.Core;
using GazRouter.DTO.ObjectModel;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.ObjectModel
{
	public abstract class DeleteEntityCommand : CommandNonQuery<DeleteEntityParameterSet>
    {
        protected DeleteEntityCommand(ExecutionContext context)
            : base(context)
        {
            IsStoredProcedure = true;
            IntegrityConstraints.Add("(RDI.FK_REPAIRS_ENTITY)", "Сущность не может быть удалена, так как используется в модуле ремонтов");
        }

		protected override void BindParameters(OracleCommand command, DeleteEntityParameterSet parameters)
        {
            command.AddInputParameter("P_ENTITY_ID", parameters.Id);
            command.AddInputParameter("p_user_name", Context.UserIdentifier);
        }

		protected override string GetCommandText(DeleteEntityParameterSet parameters)
        {
            return string.Format("rd.{0}.Remove", Package);

        }

        protected abstract string Package { get; }
    }
}