using System;

namespace GazRouter.DTO.GasLeaks
{
    public class GetLeaksParameterSet
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}