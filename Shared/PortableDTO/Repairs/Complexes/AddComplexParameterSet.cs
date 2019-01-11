using System;

namespace GazRouter.DTO.Repairs.Complexes
{

    public class AddComplexParameterSet
    {
        public string ComplexName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsLocal { get; set; }
        public int SystemId { get; set; }

    }
}