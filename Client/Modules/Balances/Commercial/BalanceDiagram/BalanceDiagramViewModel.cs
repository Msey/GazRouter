using System.Collections.Generic;
using GazRouter.Balances.Commercial.Summary;
using GazRouter.Common.ViewModel;
using GazRouter.DTO.Dictionaries.BalanceItems;
using GazRouter.DTO.Dictionaries.Targets;

namespace GazRouter.Balances.Commercial.BalanceDiagram
{
    public class BalanceDiagramViewModel : ViewModelBase
    {
        public void UpdateDiagram(BalanceSummary summary, Target target)
        {
            DiagramValues = new List<BalanceDiagramValue>();

            var pipeMinus = summary?.GetValue(BalanceItem.PipeMinus, target);
            var pipePlus = summary?.GetValue(BalanceItem.PipePlus, target);

            if (pipeMinus.HasValue && pipePlus.HasValue)
            {
                if (pipeMinus > pipePlus)
                {
                    pipeMinus -= pipePlus;
                    pipePlus = 0;
                }
                else
                {
                    pipePlus -= pipeMinus;
                    pipeMinus = 0;
                }
            }

            if (target == Target.Fact)
                DiagramValues.Add(new BalanceDiagramValue("РЕСУРСЫ", summary?.Resources?.FactValue));
            DiagramValues.Add(new BalanceDiagramValue("Поступл.", summary?.GetValue(BalanceItem.Intake, target)));
            if (target == Target.Fact)
                DiagramValues.Add(new BalanceDiagramValue("Труба-", pipeMinus));

            DiagramValues.Add(new BalanceDiagramValue("РАСПРЕД.",
                target == Target.Plan ? summary?.Distribution?.PlanValue : summary?.Distribution?.FactValue));
            DiagramValues.Add(new BalanceDiagramValue("Транзит", summary?.GetValue(BalanceItem.Transit, target)));
            DiagramValues.Add(new BalanceDiagramValue("Потреб.", summary?.GetValue(BalanceItem.Consumers, target)));
            DiagramValues.Add(new BalanceDiagramValue("СТН", summary?.GetValue(BalanceItem.AuxCosts, target)));
            DiagramValues.Add(new BalanceDiagramValue("ПЭН", summary?.GetValue(BalanceItem.OperConsumers, target)));

            if (target == Target.Fact)
                DiagramValues.Add(new BalanceDiagramValue("Труба+", pipePlus));
            
            OnPropertyChanged(() => DiagramValues);
        }

        public List<BalanceDiagramValue> DiagramValues { get; set; } 
        
        
    }



}
