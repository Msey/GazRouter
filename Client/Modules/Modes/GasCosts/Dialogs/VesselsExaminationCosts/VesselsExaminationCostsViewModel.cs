using System;
using System.Collections.Generic;
using GazRouter.DTO.GasCosts;
using GazRouter.Modes.GasCosts.DefaultDataDialog;
using GazRouter.Modes.GasCosts.Dialogs.PurgeCosts;
namespace GazRouter.Modes.GasCosts.Dialogs.VesselsExaminationCosts
{
    public class VesselsExaminationCostsViewModel : PurgeCostsViewModel
    {
        public VesselsExaminationCostsViewModel(GasCostDTO gasCost, Action<GasCostDTO> callback, List<DefaultParamValues> defaultParamValues, bool ShowDayly) : base(gasCost, callback, defaultParamValues, ShowDayly)            
        {

            this.ShowDayly = ShowDayly;
            Header = "На освидетельствование сосудов, работающих под давлением";
        }
    }
}
