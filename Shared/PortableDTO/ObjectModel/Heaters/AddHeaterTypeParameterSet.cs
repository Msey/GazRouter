using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GazRouter.DTO.ObjectModel.Heaters
{
    public class AddHeaterTypeParameterSet
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public double GasConsumptionRate { get; set; }
        public double? EfficiencyFactorRated { get; set; }
    }
}
