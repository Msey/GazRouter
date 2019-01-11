using GazRouter.DAL.Core;

namespace GazRouter.DAL.ObjectModel.Boilers
{
    public class DeleteBoilerCommand : DeleteEntityCommand
    {
		public DeleteBoilerCommand(ExecutionContext context)
            : base(context)
        {

        }

    protected override string Package
        {
			get { return "p_boiler"; }
        }
    }
}