using System.Collections.Generic;
using GazRouter.DTO.Calculations.Parameter;

namespace GazRouter.DTO.Calculations.Calculation
{
    public class TestCalcResultDTO
    {
        public TestCalcResultDTO()
        {
            Parameters = new List<CalculationParameterDTO>();
            IsInvalid = false;
        }

        public List<CalculationParameterDTO> Parameters { get; set; }
        public string Error { get; set; }
        public bool IsInvalid { get; set; }
    }
}