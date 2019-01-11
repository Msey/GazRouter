using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GazRouter.DTO.ObjectModel.EmergencyValves
{
    public class AddEmergencyValveTypeParameterSet
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public double InnerDiameter { get; set; }
    }
}
