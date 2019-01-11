using System;

namespace GazRouter.DTO.ObjectModel.MeasLine
{
	public class AddMeasLineParameterSet : AddEntityParameterSet
    {
		public double KmOfConn { get; set; }
		public Guid PipelineId { get; set; }
		public bool Status { get; set; }
	    public Guid? Id { get; set; }
    }
}
