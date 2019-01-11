using GazRouter.DAL.Core;

namespace GazRouter.DAL.ObjectModel.MeasPoint
{

    public class DeleteMeasPointCommand : DeleteEntityCommand
	{
		public DeleteMeasPointCommand(ExecutionContext context) 
            : base(context) {}
		
	    
        protected override string Package
        {
            get { return "P_MEAS_Point"; }
        }
	}

}