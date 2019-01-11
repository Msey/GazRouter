using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GazRouter.DTO.ObjectModel.Boilers
{
    public class AddBoilerTypeParameterSet
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public double RatedHeatingEfficiency { get; set; }

        public double RatedEfficiencyFactor { get; set; }

        public bool IsSmall { get; set; }

        public string GroupName { get; set; }

        public double HeatingArea { get; set; }
    }
}
