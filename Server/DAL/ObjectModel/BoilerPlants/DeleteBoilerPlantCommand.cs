using GazRouter.DAL.Core;

namespace GazRouter.DAL.ObjectModel.BoilerPlants
{
    public class DeleteBoilerPlantCommand : DeleteEntityCommand
    {
        public DeleteBoilerPlantCommand(ExecutionContext context)
            : base(context)
        {
        }

        protected override string Package
        {
            get { return "P_BOILER_PLANT"; }
        }
    }
}