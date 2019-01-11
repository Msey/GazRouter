using GazRouter.DAL.Core;

namespace GazRouter.DAL.ObjectModel.MeasLine
{

    public class DeleteMeasLineCommand : DeleteEntityCommand
	{
		public DeleteMeasLineCommand(ExecutionContext context) 
            : base(context) {}
		
	    
        protected override string Package
        {
            get { return "P_MEAS_LINE"; }
        }
	}

}