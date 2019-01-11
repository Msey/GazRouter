using System;
using GazRouter.DTO.Dictionaries.StatesModel;

namespace GazRouter.DTO.ManualInput.ValveSwitches
{
    public class DeleteValveSwitchParameterSet
    {
        public Guid ValveId { get; set; }
        public ValveSwitchType ValveSwitchType { get; set; }
        public DateTime SwitchingDate { get; set; }
    }
}