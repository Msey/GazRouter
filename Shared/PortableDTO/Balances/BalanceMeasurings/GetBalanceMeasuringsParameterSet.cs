using System;
using GazRouter.DTO.ObjectModel;

namespace GazRouter.DTO.Balances.BalanceMeasurings
{
    public class GetBalanceMeasuringsParameterSet
    {
        public int Month { get; set; }

        public int Year { get; set; }

        public int Day { get; set; }
    }
}
