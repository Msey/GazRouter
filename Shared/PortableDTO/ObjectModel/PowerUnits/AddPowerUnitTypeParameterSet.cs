using GazRouter.DTO.Dictionaries.PowerUnitTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GazRouter.DTO.ObjectModel.PowerUnits
{
    public class AddPowerUnitTypeParameterSet
    {
        public string Name { get; set; }
        public string Description { get; set; }

        public double RatedPower { get; set; }

        public double FuelConsumptionRate { get; set; }

        public EngineGroup EngineGroup { get; set; }

        public string EngineTypeName { get; set; }
    }
}
