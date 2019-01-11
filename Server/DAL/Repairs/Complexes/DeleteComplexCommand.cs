using GazRouter.DAL.Core;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.Repairs.Complexes
{
    public class DeleteComplexCommand : CommandNonQuery<int>
    {
        public DeleteComplexCommand(ExecutionContext context)
            : base(context)
        {
            IsStoredProcedure = true;
			IntegrityConstraints.Add("(RDI.FK_REPAIRS_PPW_COMPEX)", "Невозможно удалить комплекс так как он используется в существующих ремонтах");
        }

        protected override void BindParameters(OracleCommand command, int parameters)
        {
            command.AddInputParameter("p_complex_id", parameters);
            command.AddInputParameter("p_user_name", Context.UserIdentifier);
        }

        protected override string GetCommandText(int parameters)
        {
            return "P_PPW_COMPLEX.Remove";
        }
    }
}
