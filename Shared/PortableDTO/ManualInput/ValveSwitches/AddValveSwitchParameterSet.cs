using System;
using GazRouter.DTO.Dictionaries.StatesModel;

namespace GazRouter.DTO.ManualInput.ValveSwitches
{
    public class AddValveSwitchParameterSet
    {
        public Guid ValveId { get; set; }
        public DateTime SwitchingDate { get; set; }
        public ValveSwitchType ValveSwitchType { get; set; }
        public ValveState State { get; set; }
    }
}