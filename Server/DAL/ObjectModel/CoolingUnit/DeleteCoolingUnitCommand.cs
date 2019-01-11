using GazRouter.DAL.Core;

namespace GazRouter.DAL.ObjectModel.CoolingUnit
{
    public class DeleteCoolingUnitCommand : DeleteEntityCommand
    {
		public DeleteCoolingUnitCommand(ExecutionContext context)
            : base(context)
        {}

        protected override string Package
        {
			get { return "p_Cooling_Unit"; }
        }
    }
}