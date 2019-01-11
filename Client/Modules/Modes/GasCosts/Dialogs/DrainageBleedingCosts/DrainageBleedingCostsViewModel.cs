using System;
using System.Collections.Generic;
using GazRouter.DTO.GasCosts;
using GazRouter.Modes.GasCosts.DefaultDataDialog;
using GazRouter.Modes.GasCosts.Dialogs.PurgeCosts;
namespace GazRouter.Modes.GasCosts.Dialogs.DrainageBleedingCosts
{
    public class DrainageBleedingCostsViewModel : PurgeCostsViewModel
    {
        public DrainageBleedingCostsViewModel(GasCostDTO gasCost, Action<GasCostDTO> callback, List<DefaultParamValues> defaultParamValues,bool ShowDayly) : base(gasCost, callback, defaultParamValues,ShowDayly)
        {

            this.ShowDayly = ShowDayly;
            Header = "Стравливание газа при продувке дренажей УСБ и продувке импульсных линий";
        }
    }
}
