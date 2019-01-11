using System;
using System.Collections.Generic;
using System.Linq;
using GazRouter.Common.ViewModel;
using GazRouter.DTO.Dictionaries.EntityTypes;
using GazRouter.DTO.GasCosts;
using GazRouter.Modes.GasCosts.Summary;
using Utils.Extensions;
using System.Windows;
namespace GazRouter.Modes.GasCosts
{
    public class ConsumptionSummaryViewModel : LockableViewModel, IConsumptionTab
    {
        private readonly GasCostsMainViewModel _gasCostsMainViewModel;
        
        private List<GasCostsSummaryGroup> _items;
        public List<GasCostsSummaryGroup> GasCostsGroups => _items;

        public bool IsDayly => _gasCostsMainViewModel.ShowDayly;

        private string _FactColumnHeader;
        public string FactColumnHeader
        {
            get
            {
                return _FactColumnHeader;
        }
            set
        {
                SetProperty(ref _FactColumnHeader, value);
        }

        }

        //protected Guid SiteId { get; set; }
        //public DateTime Month { get; set; }

        public string Header
        {
            get { return "Сводка"; }
        }
        public ConsumptionSummaryViewModel(GasCostsMainViewModel gasCostsMainViewModel)
        {
            _gasCostsMainViewModel = gasCostsMainViewModel;
        }

        public void UpdateFormat(int Coef)
        {
            _coef = Coef;
            OnPropertyChanged(() => VisibleDuplicate);
        }

        private int _coef = 1;
        public Visibility VisibleDuplicate => _coef > 1 ? 
            Visibility.Visible : 
            Visibility.Collapsed;


        public void LoadGasCost(List<GasCostTypeDTO> data, List<GasCostDTO> costs)
        {
            bool _IsDayly = IsDayly;
            _items = new List<GasCostsSummaryGroup>();
            var lpu = new LPUCostsSummaryGroup();
            foreach (var group in lpu._items.Values)
            {
                group.FillChilds(data);
                foreach (var costDTO in costs)
                {
                    group.AddGasCost(costDTO);
                }
            }
            lpu.AddGasCost();
            _items.Add(lpu);

            if (_IsDayly)
            {
                FactColumnHeader = "Факт тек. сутки";
                LoadFactTotalToDateGasCosts(lpu);
            }
            else
            {
                FactColumnHeader = "Факт";
                OnPropertyChanged(() => GasCostsGroups);
            }

            OnPropertyChanged(() => IsDayly);
        }
        
        private async void LoadFactTotalToDateGasCosts(LPUCostsSummaryGroup LPU)
        {
            List<GasCostDTO> FactTotalToDateGasCosts = new List<GasCostDTO>();
            FactTotalToDateGasCosts =
                    await
                    new DataProviders.GasCosts.GasCostsServiceProxy().GetGasCostListAsync(new GetGasCostListParameterSet
                    {
                        StartDate = _gasCostsMainViewModel.SelectedMonth.ToLocalTime().MonthStart(),
                        EndDate = _gasCostsMainViewModel.SelectedMonth,
                        Target = DTO.Dictionaries.Targets.Target.Fact,
                        SiteId = _gasCostsMainViewModel.SelectedSiteId,
                    });

            if(_coef!=1)
                foreach (var GasCost in FactTotalToDateGasCosts)
                {
                    GasCost.MeasuredVolume = GasCost.MeasuredVolume * _coef;
                    GasCost.CalculatedVolume = GasCost.CalculatedVolume * _coef;
                }

            foreach (var group in LPU._items.Values)
            {
                    foreach (var costDTO in FactTotalToDateGasCosts)
                    {
                        group.AddFactTotalToDateGasCost(costDTO);
                    }
            }
            LPU.AddFactTotalToDateGasCost();
            OnPropertyChanged(() => GasCostsGroups);
        }

        public List<CostType> GetCostTypeCollection()
        {
            throw new NotImplementedException();
        }
    }

    public class LPUCostsSummaryGroup : GasCostsSummaryGroup
    {
        public readonly Dictionary<EntityType, GasCostsSummaryGroup> _items = 
            new Dictionary<EntityType, GasCostsSummaryGroup>();
        public LPUCostsSummaryGroup()
            : base("Всего по ЛПУ")
        {
            _items.Add(EntityType.CompStation, new CompStationCostsSummaryGroup());
            _items.Add(EntityType.Pipeline, new PipelineCostsSummaryGroup());
            _items.Add(EntityType.DistrStation, new DistrStationCostsSummaryGroup());
            _items.Add(EntityType.MeasStation, new MeasStationCostsSummaryGroup());
            
        }
      
        public override List<GasCostsSummaryItemBase> Items
        {
            get { return _items.Values.ToList<GasCostsSummaryItemBase>(); }
        }

        public void AddGasCost()
        {
           foreach(var v in _items.Values)
           {
                Fact += v.Fact;
                Plan += v.Plan;
                Norm += v.Norm;
           }
        }

        public void AddFactTotalToDateGasCost()
        {
            foreach (var v in _items.Values)
            {
                FactTotalToDate += v.FactTotalToDate;
            }
        }

        public override IEnumerable<CostType> CostTypes
        {
            get
            {
                IEnumerable<CostType> _CostTypes = new List<CostType>();
                foreach (var Group in _items)
                    _CostTypes = _CostTypes.Union(Group.Value.CostTypes);
                return _CostTypes.ToList();
                //05.03.2018
                //return CompStationConsumptionViewModel.GetStaticColumnCollection().Select(e => e.CostType);
            }
        }
    }
}