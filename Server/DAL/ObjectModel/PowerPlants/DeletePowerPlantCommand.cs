using GazRouter.DAL.Core;

namespace GazRouter.DAL.ObjectModel.PowerPlants
{
    public class DeletePowerPlantCommand : DeleteEntityCommand
    {
        public DeletePowerPlantCommand(ExecutionContext context)
            : base(context)
        {
        }

        protected override string Package
        {
            get { return "P_POWER_PLANT"; }
        }
    }
}