using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using GazRouter.DTO.GasCosts;

namespace GazRouter.Modes.GasCosts.Summary
{
    public abstract class GasCostsSummaryGroup : GasCostsSummaryItemBase
    {
        public GasCostsSummaryGroup(string name ) : base(name)
        {

        }

        private readonly Dictionary<CostType, GasCostsSummaryItem> _dictionary = new Dictionary<CostType, GasCostsSummaryItem>();

        [Display(AutoGenerateField = false)]
        public virtual List<GasCostsSummaryItemBase> Items
        {
            get { return _dictionary.Values.ToList<GasCostsSummaryItemBase>(); }
        }

        public void FillChilds(List<GasCostTypeDTO> costTypeList)
        {
            foreach (var costTypeDTO in costTypeList.Where(costTypeDTO => CostTypes.Contains(costTypeDTO.CostType)))                        
                _dictionary.Add(costTypeDTO.CostType, new GasCostsSummaryItem(costTypeDTO.CostTypeName));                        
        }

        public abstract IEnumerable<CostType> CostTypes { get;}

        public override void AddGasCost(GasCostDTO costDTO)
        {
            if (_dictionary.ContainsKey(costDTO.CostType))
            {
                _dictionary[costDTO.CostType].AddGasCost(costDTO);
                base.AddGasCost(costDTO);
            }
        }

        public override void AddFactTotalToDateGasCost(GasCostDTO costDTO)
        {
            if (_dictionary.ContainsKey(costDTO.CostType))
            {
                _dictionary[costDTO.CostType].AddFactTotalToDateGasCost(costDTO);
                base.AddFactTotalToDateGasCost(costDTO);
            }
        }
    }
}         