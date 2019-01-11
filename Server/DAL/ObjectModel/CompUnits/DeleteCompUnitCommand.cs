using GazRouter.DAL.Core;

namespace GazRouter.DAL.ObjectModel.CompUnits
{
    public class DeleteCompUnitCommand : DeleteEntityCommand
    {
        public DeleteCompUnitCommand(ExecutionContext context)
            : base(context)
        {

        }

        protected override string Package
        {
            get { return "P_COMP_UNIT"; }
        }
    }
}