using System;


namespace GazRouter.DTO.SeriesData.GasInPipes
{
    public class GetGasInPipeListParameterSet
    {
        public int? SystemId { get; set; }
        public Guid? PipelineId { get; set; }
        
        public DateTime BeginDate { get; set; }
        public DateTime EndDate { get; set; }


        public double? KmBegin { get; set; }

        public double? KmEnd { get; set; }
    }
}
