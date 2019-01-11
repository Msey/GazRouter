using GazRouter.DAL.Core;

namespace GazRouter.DAL.ObjectModel.Valves
{

	public class DeleteValveCommand : DeleteEntityCommand
	{
        public DeleteValveCommand(ExecutionContext context) : base(context) { }
		

        protected override string Package
        {
            get { return "P_VALVE"; }
        }
	}

}