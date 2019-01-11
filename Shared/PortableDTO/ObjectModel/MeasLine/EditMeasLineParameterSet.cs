using System;

namespace GazRouter.DTO.ObjectModel.MeasLine
{
	public class EditMeasLineParameterSet : EditEntityParameterSet
    {
		public double KmOfConn { get; set; }
		public Guid PipelineId { get; set; }
		public bool Status { get; set; }
	}
}
