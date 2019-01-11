using GazRouter.DAL.Core;

namespace GazRouter.DAL.ObjectModel.PowerUnits
{
    public class DeletePowerUnitCommand : DeleteEntityCommand
    {
        public DeletePowerUnitCommand(ExecutionContext context)
            : base(context)
        {

        }

    protected override string Package
        {
			get { return "p_Power_Unit"; }
        }
    }
}