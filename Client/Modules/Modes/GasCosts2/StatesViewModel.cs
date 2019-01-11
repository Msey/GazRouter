using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using GazRouter.DTO.Dictionaries.Targets;
using GazRouter.DTO.GasCosts;
using Microsoft.Practices.ObjectBuilder2;
using Microsoft.Practices.Prism;
namespace GazRouter.Modes.GasCosts2
{
    internal class CostValues
    {
        public double FactTotalToDate { get; set; }
        public double Fact { get; set; }    
        public double Norm { get; set; }    
        public double Plan { get; set; }
    }
    /// <summary>
    /// todo: 1. назначить parenta
    ///       2. сделать проход по Статьям и вызов метода
    /// 
    /// </summary>
    public class StateItem : CostModelBase
    {
#region constructor
        public StateItem(string name) 
        {
            Items = new ObservableCollection<StateItem>();
            Name = name;
        }
        public StateItem(string name, Action<StateItem> act)
        {
            _act = act;
            Items = new ObservableCollection<StateItem>();
            Name = name;
        }
#endregion
#region variables
        private readonly Action<StateItem> _act;
#endregion
#region property
        public CostType CostType { get; set; }
        public int TabNum { get; set; }
        public int GroupId { get; set; }
        public string CostTypeDescription { get; set; }
        public int? Regular { get; set; }

        private bool _visibility;
        public bool Visibility
        {
            get { return _visibility; }
            set
            {
                SetProperty(ref _visibility, value);
                _act?.Invoke(this);
            }
        }

        private bool _isEnabled;
        public bool IsEnabled
        {
            get { return _isEnabled; }
            set { SetProperty(ref _isEnabled, value); }
        }

        private bool _isSelected;
        public bool IsSelected
        {
            get { return _isSelected; }
            set { SetProperty(ref _isSelected, value); }
        }

        private ObservableCollection<StateItem> _items;
        public ObservableCollection<StateItem> Items
        {
            get { return _items; }
            set { SetProperty(ref _items, value); }
        }
#endregion
#region methods
        public void Update()
        {
            FactTotalToDate = SumFactTotalToDate();
            Fact            = SumFact();
            Plan            = SumPlan();
            Norm            = SumNorm();
        }
        public double SumFactTotalToDate()
        {
            if (Items == null || Items.Count == 0) return FactTotalToDate;
            //
            double sum = 0;
            Items.ForEach(e => sum += e.SumFactTotalToDate());
            FactTotalToDate = sum;
            return sum;
        }
        public double SumFact()
        {
            if (Items == null || Items.Count == 0) return Fact;
            //
            double sum = 0;
            Items.ForEach(e => sum += e.SumFact());
            Fact = sum;
            return sum;
        }
        public double SumPlan()
        {
            if (Items == null || Items.Count == 0) return Plan;
            //
            double sum = 0;
            Items.ForEach(e => sum += e.SumPlan());
            Plan = sum;
            return sum;
        }
        public double SumNorm()
        {
            if (Items == null || Items.Count == 0) return Norm;
            //
            double sum = 0;
            Items.ForEach(e => sum += e.SumNorm());
            Norm = sum;
            return sum;
        }
#endregion
    }
    public class StateGroup : StateItem
    {
        public StateGroup(string name) : base(name)
        {
        }
        public StateGroup(string name, Action<StateItem> act) : base(name, act)
        {
        }
    }
    public class StatesViewModel : GasCostViewModelBase
    {
#region constructor
        public StatesViewModel(Action<StateItem> selectCostAction)             
        {
            _selectCostAction = selectCostAction;
            States            = new ObservableCollection<StateItem>();
        }
#endregion
#region variables
        private readonly Action<StateItem> _selectCostAction;
#endregion
#region property
        private StateItem _selectedState;
        public StateItem SelectedState
        {
            get
            {
                return _selectedState;
            }
            set
            {
                if(_selectedState != null)
                    _selectedState.IsSelected = false;
                //
                SetProperty(ref _selectedState, value);
                OnSelectedItemChanged();
                //
                if (_selectedState != null)
                    _selectedState.IsSelected = true;
            }
        }

        private ObservableCollection<StateItem> _states;
        public ObservableCollection<StateItem> States
        {
            get { return _states; }
            set { SetProperty(ref _states, value); }
        }
#endregion
#region events
        public void OnSelectedItemChanged()
        {
            _selectCostAction.Invoke(_selectedState);
        }
#endregion
#region fill
        /// <symmary>
        /// 
        /// // заполнение значений статей
        /// 
        /// var kskc = new StateItem("Линейная часть");
        /// var kskc = new StateItem("ГРС");
        /// var kskc = new StateItem("ГИС");
        /// Всего по ЛПУ   - root
        /// КС, КЦ         -                     TAB_NUM
        /// 1 Топливный газ                      AUX_ITEM_GROUP_ID
        ///      Расход топливного газа СТ12   
        /// 2 Собственные технологичекие нужды 
        /// 3 - 
        /// 4 -
        /// 
        /// </symmary>
        public void BuildStates(StateItem root, 
                                List<GasCostDTO> costs,
                                DateTime selectedDate)
        {
            States.Clear();
            States.Add(root);
            FillStatesTree(costs, selectedDate);
        }
        private void FillStatesTree(ICollection<GasCostDTO> costs, 
                                    DateTime selectedDate)
        {
            if (costs.Count == 0) return;
            //
            var costsLookSum = costs.ToLookup(e => e.CostType);
            if (costsLookSum.Count == 0) return;
            //
            var costsSum = costsLookSum.ToDictionary(k => k.Key, v =>
            {
                double factTotalToDate = 0, fact = 0, norm = 0, plan = 0;
                var factTotalList = v.Where(e => e.Target == Target.Fact && 
                    (e.Date.Day == selectedDate.Day)).ToArray();
                var factList = v.Where(e => e.Target == Target.Fact).ToArray();
                var planList = v.Where(e => e.Target == Target.Plan).ToArray();
                var normList = v.Where(e => e.Target == Target.Norm).ToArray();
                // 
                if (factTotalList.Length > 0)
                    factTotalToDate = factTotalList.Sum(e => e.Volume);
                if (factList.Length > 0)
                    fact = factList.Sum(e => e.Volume);
                if (planList.Length > 0)
                    plan = planList.Sum(e => e.Volume);
                if (normList.Length > 0)
                    norm = normList.Sum(e => e.Volume);
                return new CostValues
                {
                    FactTotalToDate = factTotalToDate,
                    Fact            = fact,
                    Plan            = plan,
                    Norm            = norm,
                };
            });
            //
            Traversal(States.First(), item =>
            {
                if (item.CostType == CostType.None || !costsSum.ContainsKey(item.CostType)) return;                                
                //
                var cost = costsSum[item.CostType];
                item.FactTotalToDate = cost.FactTotalToDate;
                item.Fact            = cost.Fact;
                item.Norm            = cost.Norm;
                item.Plan            = cost.Plan;
            });
        }
#endregion
#region api
        private void UpdateStateData(List<EntityRowBase> rows)
        {
            SelectedState.FactTotalToDate = rows.Sum(e => e.FactTotalToDate);
            SelectedState.Fact            = rows.Sum(e => e.Fact);
            SelectedState.Norm            = rows.Sum(e => e.Norm);
            SelectedState.Plan            = rows.Sum(e => e.Plan);
        }
        public void Update(List<EntityRowBase> rows)
        {
            UpdateStateData(rows);
            States.First().Update();            
        }
        public void Update()
        {
            States.First().Update();
        }
        public void ClearSelection()
        {
            SelectedState = null;
        }
        public override void UpdateUnits(int units)
        {
            UnitsChanged?.Invoke(units);
            Traversal(GetRoot(), item => { item.UpdateProperty(); });
        }
        private StateItem GetRoot()
        {
            var root = new StateItem("root");
            root.Items.AddRange(States);
            return root;
        }
        public void GetStateItem(CostType costType, out StateItem stateItem)
        {
            const string emptyStateName ="state";
            var state = new StateItem(emptyStateName);
            Traversal(GetRoot(), item =>
            {
                if (item.CostType == costType) state = item;                
            });
            if (state.Name.Equals(emptyStateName)) throw new Exception($"Список не содержит статью c номером: {costType}");
            stateItem = state;
        }
#endregion
    }
}
#region trash

// Items = new ObservableCollection<StateItem>(States.ToList()) 

//        public virtual void AddGasCost(GasCostDTO costDTO)
//        {
//            switch (costDTO.Target)
//            {
//                case Target.Fact:
//                    Fact += costDTO.Volume;
//                    break;
//                case Target.Plan:
//                    Plan += costDTO.Volume;
//                    break;
//                case Target.Norm:
//                    Norm += costDTO.Volume;
//                    break;
//            }
//        }
//        public virtual void AddFactTotalToDateGasCost(GasCostDTO costDTO)
//        {
//            //if (costDTO.Target == Target.Fact)
//            FactTotalToDate += costDTO.Volume;
//        }

//        public StateItem(GasCostTypeDTO gasCostType)
//        {
//            Items = new ObservableCollection<StateItem>();
//            Name = gasCostType.CostTypeName;
//            GroupId = gasCostType.GroupId;
//            CostType = gasCostType.CostType;
//        }

//            _items = new List<GasCostsSummaryGroup>();
//            var lpu = new LPUCostsSummaryGroup();
//
//            foreach (var group in lpu._items.Values)
//            {
//                group.FillChilds(data);
//                foreach (var costDTO in costs)
//                    group.AddGasCost(costDTO);
//            }
//            lpu.AddGasCost();
//            _items.Add(lpu);
// LoadFactTotalToDateGasCosts(lpu);



// var groupId = summaryItem.GroupId;

//        public void LoadGasCost2(List<GasCostTypeDTO> data, List<GasCostDTO> costs)
//        {
//            _items = new List<GasCostsSummaryGroup>();
//            var lpu = new LPUCostsSummaryGroup();
//
//            foreach (var group in lpu._items.Values)
//            { 
//                group.FillChilds(data);
//                foreach (var costDTO in costs)
//                    group.AddGasCost(costDTO);
//            }
//            lpu.AddGasCost();
//            _items.Add(lpu); // LoadFactTotalToDateGasCosts(lpu);
//        }

//        public void FillChilds(List<GasCostTypeDTO> costTypeList)
//        {
//            foreach (var costTypeDTO in costTypeList.Where(costTypeDTO => 
//                CostTypes.Contains(costTypeDTO.CostType)))
//                    _dictionary.Add(costTypeDTO.CostType, 
//                        new GasCostsSummaryItem(costTypeDTO));
//        }
//        public void WrapSummaryData()
//        {
//            States.Clear();
//            //
//            var itemGroups = ClientCache.DictionaryRepository.GasCostItemGroups;
//            var allSummary = _items[0];
//            var root = GetHierarchicalItem(allSummary);
//            allSummary.Items.ForEach(items =>
//            {
//                var groupItems = ((GasCostsSummaryGroup)items).Items;
//                var groupHierarchicalItems = GetHierarchicalItem(items);
//                AddGroups(itemGroups, groupHierarchicalItems, groupItems);
//                root.Items.Add(groupHierarchicalItems);
//            });
//            States.Add(root);
//        }
//        private static StateItem GetHierarchicalItem(GasCostsSummaryItemBase itemBase)
//        {
//            var name = itemBase.Name;
//            var item = new StateItem(name)
//            {
//                CostType = itemBase.CostType,
//                CostTypeDescription = itemBase.CostType == CostType.None ? "" : itemBase.CostType.ToString(),
//                Norm = itemBase.Norm,
//                Plan = itemBase.Plan,
//                Fact = itemBase.Fact,
//                FactTotalToDate = itemBase.FactTotalToDate
//            };
//            return item;
//        }

//        private static void AddGroups(IEnumerable<GasCostItemGroupDTO> groups, 
//                                      StateItem summaryGroup,
//                                      List<GasCostsSummaryItemBase> items)
//        {
//            foreach (var group in groups)
//            {
//                var groupItems = items.Where(e => e.GroupId == group.Id).ToArray();
//                if (groupItems.Length <= 0) continue;
//                // add group
//                var gr = new StateItem(@group.Name);
//                summaryGroup.Items.Add(gr);
//                var hierarchicalItems = groupItems.Select(GetHierarchicalItem);
//                gr.Items.AddRange(hierarchicalItems);
//            }
//        }


//        public List<GasCostsSummaryGroup> GasCostsGroups => _items;

//        IGasCostsParameters costsParameters

//        public void AddItem(StateItem item)
//        {
//            if (Items == null) Items = new ObservableCollection<StateItem>();
//            //
//            Items.Add(item);
//        }
#endregion
