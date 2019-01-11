using System;

namespace GazRouter.DTO.Calculations.Log
{
    public class GetLogListParameterSet
    {
        public int? CalculationId { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}