using GazRouter.DAL.Core;

namespace GazRouter.DAL.ObjectModel
{
    public sealed class AfterValidateCommand : CommandNonQuery
    {
        public AfterValidateCommand(ExecutionContext context) : base(context)
        {
            IsStoredProcedure = true;
        }

        protected override string GetCommandText()
        {
            return "P_ENTITY.Validation_AFTER";
        }
    }
}