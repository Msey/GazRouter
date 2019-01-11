using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using GazRouter.Common.ViewModel;
using GazRouter.DTO.Dictionaries.BalanceSigns;


namespace GazRouter.Balances.Commercial.Plan.Corrections
{
    public class CorrectionsSummaryViewModel : DialogViewModel
    {
        public CorrectionsSummaryViewModel(List<PlanOwnerItem> items, int docId)
            : base(null)
        {
            Items = new List<DocCorrectionItem>();

            var inItem = new DocCorrectionItem {Name = "РЕСУРСЫ:"};
            Items.Add(inItem);
            var outItem = new DocCorrectionItem {Name = "РАСПРЕДЕЛЕНИЕ:"};
            Items.Add(outItem);

            foreach (var balItem in ClientCache.DictionaryRepository.BalanceItems)
            {
                var bItem = new DocCorrectionItem {Name = balItem.Name};
                foreach (var entity in items.Where(i => i.BalItem == balItem.BalanceItem).Select(i => i.Entity).Distinct())
                {
                    var entityItem = new DocCorrectionItem {Name = entity.Name};
                    foreach (var owner in items.Where(i => i.Entity == entity))
                    {
                        var oItem = new DocCorrectionItem
                        {
                            Name = owner.Name,
                            Value = owner.CorrectionList.SingleOrDefault(c => c.DocId == docId)?.Value
                        };
                        entityItem.Childs.Add(oItem);
                    }
                    bItem.Childs.Add(entityItem);
                }
                
                if (bItem.Childs.Count == 0) continue;
                if (balItem.BalanceSign == Sign.In) inItem.Childs.Add(bItem);
                if (balItem.BalanceSign == Sign.Out) outItem.Childs.Add(bItem);
            }
        }


        public List<DocCorrectionItem> Items { get; set; }
    }

    public class DocCorrectionItem
    {
        private double? _value;

        public DocCorrectionItem()
        {
            Childs = new List<DocCorrectionItem>();
        }

        public string Name { get; set; }

        public double? Value
        {
            get { return _value.HasValue ? _value : Childs.Sum(c => c.Value); }
            set { _value = value; }
        }

        public List<DocCorrectionItem> Childs { get; set; } 
    }
    
}