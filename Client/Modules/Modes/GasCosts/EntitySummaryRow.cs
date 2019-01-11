using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using GazRouter.Common.ViewModel;
using GazRouter.DTO.GasCosts;
using GazRouter.DTO.ObjectModel;
using JetBrains.Annotations;
namespace GazRouter.Modes.GasCosts
{
    public class EntitySummaryRow : PropertyChangedBase
    {
#region constructors
        private EntitySummaryRow(ConsumptionViewModelBase consumptionViewModel)
        {
           
            if (consumptionViewModel == null) throw new ArgumentNullException(nameof(consumptionViewModel));
            ConsumptionViewModel = consumptionViewModel;
            Summary = new TotalCostSummarySell(this);
        }
        public EntitySummaryRow(string name, 
                                [NotNull] ConsumptionViewModelBase consumptionViewModel)
            : this(consumptionViewModel)
        {
          
            IsFolder = true;
            Name = name;
            Id = Guid.NewGuid();
        }
        public EntitySummaryRow(EntityDTO entity, 
                                [NotNull] ConsumptionViewModelBase consumptionViewModel)
            : this(consumptionViewModel)
        {
       
            Entity = entity;
            Id = entity.Id;
            Name = entity.Name;

//            FillGasCosts(consumptionViewModel.Data);
//            EntityType = entity.EntityType;
        }
#endregion
#region properties
        private bool _isExpanded = true;
        public bool IsExpanded
        {
            get { return _isExpanded; }
            set { SetProperty(ref _isExpanded, value); }
        }
        public Guid Id { get; set; }
        [Display(Name = "Объект")]
        public string Name { get; set; }
        /// <summary> Дочерние строки </summary>
        public bool IsFolder { get; }
        public int SubItemsCount
        {
            get { return _items.Count; }
        }
        public TotalCostSummarySell Summary { get; }
        public CommonEntityDTO Entity { get; set; }
        private readonly Dictionary<CostType, CostSummaryCell> 
            _cells = new Dictionary<CostType, CostSummaryCell>();       
        /// <summary> Ячейки с суммами по опредленнуму расходу </summary>
        internal IEnumerable<CostSummaryCell> Cells => _cells.Values;
        private readonly List<EntitySummaryRow> _items = new List<EntitySummaryRow>();
        public List<EntitySummaryRow> Items => _items;
        public ConsumptionViewModelBase ConsumptionViewModel { get; }
#endregion

#region public indexers
        private CostSummaryCell this[CostType costType] => GetCell(costType);
        [UsedImplicitly]
        public CostSummaryCell this[int costTypeId] => this[(CostType)costTypeId];
#endregion

#region methods    
        public bool AddGasCost(GasCostDTO gasCostDTO)
        {
            if ((Entity == null || gasCostDTO.Entity.Id != Entity.Id) &&
                !Items.Any(entitySummaryRow => entitySummaryRow.AddGasCost(gasCostDTO)))
                return false;
            GetCell(gasCostDTO.CostType)[gasCostDTO.Target] += gasCostDTO.Volume;
            OnPropertyChanged("Item[]");
            OnPropertyChanged(() => Summary);
            return true;
        }
        public bool RemoveGasCost(GasCostDTO gasCostDTO)
        {
            if ((Entity == null || gasCostDTO.Entity.Id != Entity.Id) &&
              !Items.Any(entitySummaryRow => entitySummaryRow.RemoveGasCost(gasCostDTO)))
                return false;
            GetCell(gasCostDTO.CostType)[gasCostDTO.Target] -= gasCostDTO.Volume;
            OnPropertyChanged("Item[]");
            OnPropertyChanged(() => Summary);
            return true;
        }
        public CostSummaryCell GetCell(CostType costType)
        {
            CostSummaryCell cell;
            if (_cells.TryGetValue(costType, out cell)) return cell;
            //
            var isEditable = !IsFolder &&
                             ConsumptionViewModel.MainViewModel.CostTypeEntityTipeLinkList
                                 .Any(c => c.EntityType == Entity.EntityType && c.CostType == costType);
            cell = new CostSummaryCell(isEditable);
            var mayContainValue = cell.MayContainValue ||                                                   
                                  Items.Any(entitySummaryRow => entitySummaryRow[costType].MayContainValue);
            cell.MayContainValue = mayContainValue;
            _cells.Add(costType, cell);
            return cell;
        }
        public bool CanHasValue(CostType costType)
        {
            return GetCell(costType).IsEditable || Items.Any(row => row.CanHasValue(costType));
        }
        public void AddSubItem(EntitySummaryRow entitySummaryRow)
        {
            _items.Add(entitySummaryRow);
        }
        protected bool Equals(EntitySummaryRow other)
        {
            return Id.Equals(other.Id);
        }
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((EntitySummaryRow) obj);
        }
        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
        #endregion
    }
}
#region trash
//        public bool IsEditable(CostType costType)
//        {
//            if (IsFolder)
//                return false;
//
//            return ConsumptionViewModel.MainViewModel.CostTypeEntityTipeLinkList.Any(
//                c => c.EntityType == Entity.EntityType && c.CostType == costType);
//        }

//        public EntityType EntityType { get; private set; }
#endregion
