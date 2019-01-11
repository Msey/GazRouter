using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using GazRouter.Common.ViewModel;
using GazRouter.DataProviders.GasCosts;
using GazRouter.DTO.Dictionaries.EntityTypes;
using GazRouter.DTO.Dictionaries.Targets;
using GazRouter.DTO.GasCosts;
using GazRouter.DTO.ObjectModel;
using GazRouter.Modes.GasCosts.Dialogs.ViewModel;
using JetBrains.Annotations;
using Telerik.Windows.Controls;
using Utils.Extensions;
using DelegateCommand = Microsoft.Practices.Prism.Commands.DelegateCommand;
using Microsoft.Practices.ObjectBuilder2;
using GazRouter.Common;
namespace GazRouter.Modes.GasCosts
{
    public abstract class ConsumptionViewModelBase : LockableViewModel, IConsumptionTab
    {
#region constructor
        protected ConsumptionViewModelBase([NotNull] GasCostsMainViewModel mainViewModel)
        {
            if (mainViewModel == null)
                throw new ArgumentNullException(nameof(mainViewModel));
            _costSummaryCellToValueCoverter = new CostSummaryCellToValueConverter(mainViewModel);
            MainViewModel = mainViewModel;
            CellStyleSelecter = new CostsCellStyleSelector(MainViewModel);
            UpdateColumns();
            GasCosts = new ObservableCollection<EntitySummaryRow>();
        }
#endregion

#region variables
        public bool EditPermission { get; set; }
        public abstract string Header { get; }
        public ObservableCollection<EntitySummaryRow> GasCosts { get; }
        private EntitySummaryRow _totalSummaryRow;
        protected internal EntitySummaryRow SelectedRow { get; set; }
        private GasCostDTO _selectedCost;
        public GasCostDTO SelectedCost
        {
            get
            {
                return _selectedCost;
            }
            set
            {
                SetProperty(ref _selectedCost, value);
                DeleteCostCommand.RaiseCanExecuteChanged();
                EditCostCommand.RaiseCanExecuteChanged();
            }
        }
        private List<GasCostDTO> _currentCosts;
        public List<GasCostDTO> CurrentCosts
            => _currentCosts?.Where(c => c.Target == Target).OrderBy(c => c.Date).ToList();
        public Target Target => MainViewModel.SelectedTarget.Target;
        protected internal CostType SelectedCostType { get; set; }
        public IList<GridViewCellInfo> SelectedCells { get; set; } = new List<GridViewCellInfo>();
        public List<EntityDTO> Entities { get; protected set; }
        public List<GasCostDTO> Data { get; private set; }
        public GasCostsMainViewModel MainViewModel { get; }
        private List<GasCostColumnDefinition> _columnCollection = new List<GasCostColumnDefinition>();
        public IEnumerable<GasCostColumnDefinition> ColumnCollection => _columnCollection;

        private List<GasCostColumnDefinition> _columnCollectionDuplicate = new List<GasCostColumnDefinition>();
        public IEnumerable<GasCostColumnDefinition> ColumnCollectionDuplicate => _columnCollectionDuplicate;


        private Visibility _duplicateTreeListVisibility = Visibility.Collapsed;
        public Visibility DuplicateTreeListVisible
        {
            get { return _duplicateTreeListVisibility; }
            set
            {
                if (SetProperty(ref _duplicateTreeListVisibility, value))
                    OnPropertyChanged(() => DuplicateTreeListVisible);                
            }
        }
        //Visibility="{Binding Path=BorderVisible, Converter={StaticResource BoolToVisConverter}
        public List<GasCostDTO> ChartData
        {
            get
            {
                if (CurrentCosts == null) return null;
                var result = new List<GasCostDTO>();
                for (var d = MainViewModel.SelectedMonth; 
                    d <= MainViewModel.SelectedMonth.MonthEnd(); 
                    d = d.AddDays(1))
                {
                    result.Add(new GasCostDTO
                    {
                        Date = d,
                        CalculatedVolume =
                            CurrentCosts.Any(c => c.Date.Date == d)
                                ? CurrentCosts.Where(c => c.Date.Date == d).Sum(c => c.Volume)
                                : (double?) null
                    });
                }
                return result;
            }
        }

 

        public StyleSelector CellStyleSelecter { get; }
        public GridViewCellInfo CurrentCellInfo
        {
            get { return _currentCellInfo; }
            set
            {
                _currentCellInfo = value;
                OnPropertyChanged(() => CurrentCellInfo);
            }
        }
        public DateTime Month => MainViewModel.SelectedMonth;
        public bool IsAccessAllowed => MainViewModel.IsAccessAllowed();

        public void RefreshAccessAllowed() {
            OnPropertyChanged(() => IsAccessAllowed);
            AddFactCostCommand.RaiseCanExecuteChanged();
            AddNormCostCommand.RaiseCanExecuteChanged();
            AddPlanCostCommand.RaiseCanExecuteChanged();
            EditCostCommand.RaiseCanExecuteChanged();
            DeleteCostCommand.RaiseCanExecuteChanged();
        }
        public Guid SiteId => MainViewModel.SelectedSiteId.Value;
        private readonly IValueConverter _costSummaryCellToValueCoverter;
        private GridViewCellInfo _currentCellInfo;
        private readonly CalcDialogHelper _calcDialogHelper = new CalcDialogHelper();
#endregion

#region commands
        private DelegateCommand _addCostCommand;
        private DelegateCommand _addNormCostCommand;
        private DelegateCommand _addPlanCostCommand;
        private DelegateCommand _addFactCostCommand;
        private DelegateCommand _editCostCommand;
        private DelegateCommand _deleteCostCommand;
        public DelegateCommand AddNormCostCommand
        {
            get
            {
                return _addNormCostCommand ??
                       (_addNormCostCommand = new DelegateCommand(() => Add(Target),
                       () => Target == Target.Norm && CanAdd() && EditPermission && !MainViewModel.ShowDayly));
            }
        }
        public DelegateCommand AddPlanCostCommand
        {
            get
            {
                return _addPlanCostCommand ??
                       (_addPlanCostCommand = new DelegateCommand(() => Add(Target),
                       () => Target == Target.Plan && CanAdd() && EditPermission && !MainViewModel.ShowDayly));
            }
        }
        public DelegateCommand AddFactCostCommand
        {
            get
            {
                return _addFactCostCommand ??
                       (_addFactCostCommand = new DelegateCommand(() => Add(Target),
                       () => Target == Target.Fact && CanAdd() && EditPermission && MainViewModel.SelectedSiteId.HasValue && MainViewModel.ShowDayly && MainViewModel.IsAccessAllowed()));
            }
        }
        public DelegateCommand AddCostCommand
        {
            get
            {
                return _addCostCommand ??
                       (_addCostCommand = new DelegateCommand(() => Add(Target), CanAddCostCommand));
            }
        }
        public DelegateCommand EditCostCommand
        {
            get
            {
                return _editCostCommand ??
                       (_editCostCommand =
                           new DelegateCommand(() => Edit(SelectedCost),
                               () => SelectedCost != null && EditPermission&& MainViewModel.IsAccessAllowed()
                               && ((MainViewModel.ShowDayly && Target == Target.Fact)
                                    || (!MainViewModel.ShowDayly && (Target == Target.Norm || Target == Target.Plan)))));
            }
        }
        public DelegateCommand DeleteCostCommand
        {
            get
            {
                return _deleteCostCommand ??
                       (_deleteCostCommand =
                           new DelegateCommand(() => Delete(SelectedCost),
                               () => SelectedCost != null && EditPermission&& MainViewModel.IsAccessAllowed() 
                               && ((MainViewModel.ShowDayly && Target == Target.Fact)
                                    ||(!MainViewModel.ShowDayly&& (Target == Target.Norm || Target == Target.Plan)))));
            }
        }
        private bool CanAddCostCommand()
        {
            return CanAdd() && EditPermission;
        }
        protected bool CanAdd()
        {
            var b1 = MainViewModel.CostTypeEntityTipeLinkList != null;
            var b2 = SelectedRow != null;
            if (!b2) return false;
            //
            var b3 = !SelectedRow?.IsFolder ?? false;
#region b4
            var b4 = false;
            var b41 = SelectedRow.Entity != null;
            if (b41)
            {
                var selectedCostType = SelectedCostType;
                var entityType = SelectedRow.Entity.EntityType;
                var b42 = MainViewModel.CostTypeEntityTipeLinkList
                            .Any(c => c.CostType == SelectedCostType && c.EntityType == SelectedRow.Entity.EntityType);
                b4 = b42;
            }
            //            var b4 = SelectedRow.Entity != null && MainViewModel.CostTypeEntityTipeLinkList
            //                .Any(c => c.CostType == SelectedCostType &&
            //                          c.EntityType == SelectedRow.Entity.EntityType);
#endregion
            var b5 = _calcDialogHelper.CanShow(SelectedCostType);
            return b1 && b2 && b3 && b4 && b5 && IsAccessAllowed;
        }
        protected bool CanAddFact()
        {
            return CanAdd() && MainViewModel.SelectedMonth <= DateTime.Today;
        }
        #endregion

        #region methods

        public int VolumeMultiplyer = 1;
        public void LoadGasCost(List<GasCostDTO> data)
        {
            _currentCosts = null;
            GasCosts.Clear();
            Data = data;
            _totalSummaryRow = new EntitySummaryRow("Всего по ЛПУ", this);
            FillGasCostsSummaries(_totalSummaryRow);
            foreach (var gasCostDTO in data)
            {
                _totalSummaryRow.AddGasCost(gasCostDTO);
            }
            GasCosts.Add(_totalSummaryRow);
            OnPropertyChanged(() => CurrentCosts);
            OnPropertyChanged(() => IsAccessAllowed);
            OnPropertyChanged(() => ChartData);
            if (SelectedRow != null)
                OnSelectedCellChanged(SelectedRow, ((int) SelectedCostType).ToString());            
        }

        private bool isFirstClick;
        private bool firstEntry =true;
        public void OnSelectedCellChanged(EntitySummaryRow row, string index)
        {
            if (row == null) return;

            if (firstEntry) isFirstClick = true;
            firstEntry = false;
            SelectedRow = row;
            SelectedCostType = (CostType) int.Parse(index);
            FillCurrentCosts(row.Id, row);
        }
        protected virtual void RefreshCosts(Target target)
        {
            MainViewModel.SetTarget(target);
            MainViewModel.LoadGasCosts();
        }
        protected void Add(Target target)
        {
            if (target == Target.None)
            {
                throw new ArgumentException(string.Empty, nameof(target));
            }
            _calcDialogHelper.Show(
                new GasCostDTO
                {
                    Date = MainViewModel.SelectedMonth,
                    CostType = SelectedCostType,
                    Entity = SelectedRow.Entity,
                    Target = target, 
                    SiteId = MainViewModel.SelectedSiteId.Value
                }, OnCostAdded, MainViewModel.DefaultParamValues,MainViewModel.ShowDayly);
        }
        protected void Edit(GasCostDTO selectedCost)
        {
            new CalcDialogHelper().Show(selectedCost, OnCostAdded, MainViewModel.DefaultParamValues, MainViewModel.ShowDayly);
        }
        protected abstract List<ColumnDescription> GetColumnCollection();
        protected abstract void FillGasCostsSummaries(EntitySummaryRow totalSummaryRow);
        protected void UpdateColumnsVisibility(TreeDataDTO data)
        {
            var dict = MainViewModel.CostTypeEntityTipeLinkList.ToDictionary(c => c.CostType, e => e.EntityType);
            foreach (var column in ColumnCollection.Union(ColumnCollectionDuplicate))
            {
                if (column.CostType == CostType.None) continue;
                bool needColumn;
                switch (dict[column.CostType])
                {
                    case EntityType.DistrStation:
                        needColumn = data.DistrStations.Count > 0;
                        break;
                    case EntityType.CompShop:
                        needColumn = data.CompShops.Count > 0;
                        break;
                    case EntityType.CompUnit:
                        needColumn = data.CompUnits.Count > 0;
                        break;
                    case EntityType.Boiler:
                        needColumn = data.Boilers.Count > 0;
                        break;
                    case EntityType.MeasStation:
                        needColumn = data.MeasStations.Count > 0;
                        break;
                    case EntityType.PowerUnit:
                        needColumn = data.PowerUnits.Count > 0;
                        break;
                    case EntityType.Pipeline:
                        needColumn = data.Pipelines.Count > 0;
                        break;
                    case EntityType.CoolingUnit:
                        needColumn = data.CoolingUnits.Count > 0;
                        break;
                    case EntityType.ReducingStation:
                        needColumn = data.ReducingStations.Count > 0;
                    break;
                    default:
                        needColumn = false;
                        break;
                }
                column.IsVisible = needColumn;
            }
        }
        protected void HideColumn(EntityType entityType)
        {
            var dict = MainViewModel.CostTypeEntityTipeLinkList.Where(e => e.EntityType == entityType)
                .ToDictionary(key => key.CostType, value => value.EntityType);
            ColumnCollection.ForEach(column =>
            {
                if (dict.ContainsKey(column.CostType)) column.IsVisible = false;
            });

            ColumnCollectionDuplicate.ForEach(column =>
            {
                if (dict.ContainsKey(column.CostType)) column.IsVisible = false;
            });
        }
        private void InitColumnCollection()
        {
            _columnCollection.Clear();
            _columnCollection.Add(new GasCostColumnDefinition
            {
                Header = "Итого",
                UniqueName = "Summary",
                DataMemberBinding =
                    new Binding("Summary") {StringFormat = "n3", Converter = _costSummaryCellToValueCoverter},
                Width = 80,
                TextAligment = TextAlignment.Right,
                IsResizable = true
            });

            var list = new List<GasCostColumnDefinition>();
            foreach (var columnDescription in GetColumnCollection())
            {
                var headerCellStyle = (Style) System.Windows.Application.Current.Resources["GasConstHeaderCellStyle"];
                var headerlongCellStyle =
                    (Style) System.Windows.Application.Current.Resources["GasConstLongHeaderCellStyle"];
                const int minLongHeaderLength = 100;
               list.Add(
                    new GasCostColumnDefinition
                    {
                        CostType = columnDescription.CostType,
                        Header = columnDescription.FullName,
                        DataMemberBinding =
                            new Binding($"[{(int) columnDescription.CostType}]")
                            {
                                StringFormat = "n3",
                                Converter = _costSummaryCellToValueCoverter
                            },
                        UniqueName = ((int) columnDescription.CostType).ToString(CultureInfo.InvariantCulture),
                        CellStyleSelector = CellStyleSelecter,
                        HeaderToolTip = columnDescription.FullName,
                        HeaderCellStyle =
                            Header != null && (Header.Length >= minLongHeaderLength)
                                ? headerlongCellStyle
                                : headerCellStyle,
                        Width = columnDescription.Width,
                        TextAligment = TextAlignment.Right
                    });
            }
            var v = IsolatedStorageManager.Get<string>($"consumption_columns_{Header}");
            if (v != null) 
            {
                foreach (var c in v.Split(','))
                {
                    if (c == _columnCollection[0].UniqueName) continue;
                    var col = list.FirstOrDefault(x => x.UniqueName == c);
                    if (col != null) { _columnCollection.Add(col); list.Remove(col); }
                }
            }
            _columnCollection.AddRange(list);
            OnPropertyChanged(() => ColumnCollection);


            _columnCollectionDuplicate.Clear();
            _columnCollectionDuplicate.Add(new GasCostColumnDefinition
            {
                Header = "Итого",
                UniqueName = "Summary",
                DataMemberBinding =
                    new Binding("Summary") { StringFormat = "n0", Converter = _costSummaryCellToValueCoverter },
                Width = 80,
                TextAligment = TextAlignment.Right,
                IsResizable = true
            });

            list = new List<GasCostColumnDefinition>();
            foreach (var columnDescription in GetColumnCollection())
            {
                var headerCellStyle = (Style)System.Windows.Application.Current.Resources["GasConstHeaderCellStyle"];
                var headerlongCellStyle =
                    (Style)System.Windows.Application.Current.Resources["GasConstLongHeaderCellStyle"];
                const int minLongHeaderLength = 100;
                list.Add(
                     new GasCostColumnDefinition
                     {
                         CostType = columnDescription.CostType,
                         Header = columnDescription.FullName,
                         DataMemberBinding =
                             new Binding($"[{(int)columnDescription.CostType}]")
                             {
                                 StringFormat = "n0",
                                 Converter = _costSummaryCellToValueCoverter
                             },
                         UniqueName = ((int)columnDescription.CostType).ToString(CultureInfo.InvariantCulture),
                         CellStyleSelector = CellStyleSelecter,
                         HeaderToolTip = columnDescription.FullName,
                         HeaderCellStyle =
                             Header != null && (Header.Length >= minLongHeaderLength)
                                 ? headerlongCellStyle
                                 : headerCellStyle,
                         Width = columnDescription.Width,
                         TextAligment = TextAlignment.Right
                     });
            }
            v = IsolatedStorageManager.Get<string>($"consumption_columns_{Header}");
            if (v != null)
            {
                foreach (var c in v.Split(','))
                {
                    if (c == _columnCollectionDuplicate[0].UniqueName) continue;
                    var col = list.FirstOrDefault(x => x.UniqueName == c);
                    if (col != null) { _columnCollectionDuplicate.Add(col); list.Remove(col); }
                }
            }
            _columnCollectionDuplicate.AddRange(list);
            OnPropertyChanged(() => ColumnCollectionDuplicate);
        }

        public void OnColumnReordered(GridViewColumnCollection columns)
        {
            var list = _columnCollection.Select(x => x).ToList();
            var v = _columnCollection[0];
            _columnCollection.Clear();
            _columnCollection.Add(v);
            foreach (var c in columns.Cast<GridViewColumn>().OrderBy(x=>x.DisplayIndex))
            {
                if (c.UniqueName == _columnCollection[0].UniqueName) continue;
                var col = list.FirstOrDefault(x => x.UniqueName == c.UniqueName);
                if(col!= null) _columnCollection.Add(col);
            }

            IsolatedStorageManager.Set($"consumption_columns_{Header}", string.Join(",",_columnCollection.Select(x => x.UniqueName).ToArray()));


            list = _columnCollectionDuplicate.Select(x => x).ToList();
            v = _columnCollectionDuplicate[0];
            _columnCollectionDuplicate.Clear();
            _columnCollectionDuplicate.Add(v);
            foreach (var c in columns.Cast<GridViewColumn>().OrderBy(x => x.DisplayIndex))
            {
                if (c.UniqueName == _columnCollectionDuplicate[0].UniqueName) continue;
                var col = list.FirstOrDefault(x => x.UniqueName == c.UniqueName);
                if (col != null) _columnCollectionDuplicate.Add(col);
            }

            IsolatedStorageManager.Set($"consumption_columns_{Header}", string.Join(",", _columnCollectionDuplicate.Select(x => x.UniqueName).ToArray()));
        }
        //
        private void OnCostAdded(GasCostDTO dto)
        {
            UpdateGasCosts(dto);
        }


        public Visibility GetVisibilityByCoef(int Coef)
        {
            return ((Coef > 1) ? Visibility.Visible : Visibility.Collapsed);
        }

        public void UpdateColumns(int Coef=1)
        {
            changedCostsHistory?.Clear();
            VolumeMultiplyer = Coef;
            firstChartData = true;
            DuplicateTreeListVisible = GetVisibilityByCoef(Coef);
            InitColumnCollection();
        }
        public bool firstChartData { get; set; }
        public async void UpdateGasCosts(GasCostDTO dto)
        {
            var dataSaved = await new GasCostsServiceProxy().GetGasCostListAsync(new GetGasCostListParameterSet
            {
                StartDate = MainViewModel.SelectedMonth.ToLocal().MonthStart(),
                EndDate = MainViewModel.SelectedMonth.ToLocal().MonthEnd(),
                SiteId = MainViewModel.SelectedSiteId
            });
            var cost = Data.SingleOrDefault(item => item.Id == dto.Id);
            var costSaved = dataSaved.SingleOrDefault(item => item.Id == dto.Id);
            //
            dto.ChangeDate = costSaved?.ChangeDate;
            dto.ChangeUserName = costSaved?.ChangeUserName;
            dto.ChangeUserSiteName = costSaved?.ChangeUserSiteName;
            //
            if (cost != null)
            {
                Data.Remove(cost);
                _totalSummaryRow.RemoveGasCost(cost);
            }
            Data.Add(dto);
            FillCurrentCosts(dto.Entity.Id);
            _totalSummaryRow.AddGasCost(dto);
        }

        private List<GasCostDTO> changedCostsHistory = new List<GasCostDTO>();
        private void FillCurrentCosts(Guid entityId, EntitySummaryRow row = null)
        {
            _currentCosts = SelectedCostType != CostType.None
                ? Data.Where(dto => dto.Entity.Id == entityId && dto.CostType == SelectedCostType).ToList()
                : null;
            if(_currentCosts?.Count == 0 && row!= null) //если это суммирующий элемент
            {
                var data = Data.Where(dto => dto.CostType == SelectedCostType).ToList();
                if (data.Count > 0)
                {
                    var v = new List<GasCostDTO>();
                    FillDataRow(row, data, v);
                    _currentCosts = v;
                }
            }
            if (firstChartData)
            {

                OnPropertyChanged(() => ChartData);

                if (isFirstClick)
                {
                    if (_currentCosts?.Count > 0)
                    {
                        foreach (var cost in _currentCosts)
                        {
                            if (!changedCostsHistory.Contains(cost))
                            {
                                changedCostsHistory.Add(cost);
                                cost.MeasuredVolume *= VolumeMultiplyer;
                                cost.CalculatedVolume *= VolumeMultiplyer;
                            }
                        }
                        OnPropertyChanged(() => ChartData);
                        foreach (var cost in _currentCosts)
                        {
                            if (!changedCostsHistory.Contains(cost))
                            {
                                changedCostsHistory.Add(cost);
                                cost.MeasuredVolume /= VolumeMultiplyer;
                                cost.CalculatedVolume /= VolumeMultiplyer;
                            }
                        }
                        isFirstClick = false;
                    }                   

                }
            }

            if (_currentCosts?.Count > 0)
            {
                foreach (var cost in _currentCosts)
                {
                    if (!changedCostsHistory.Contains(cost))
                    {
                        changedCostsHistory.Add(cost);
                        cost.MeasuredVolume *= VolumeMultiplyer;
                        cost.CalculatedVolume *= VolumeMultiplyer;
                    }
                }
            }

            SelectedCost = CurrentCosts?.FirstOrDefault();

            OnPropertyChanged(() => CurrentCosts);
            if (!firstChartData)
            {
                OnPropertyChanged(() => ChartData);
            }

            firstChartData = false;

            AddNormCostCommand.RaiseCanExecuteChanged();
            AddPlanCostCommand.RaiseCanExecuteChanged();
            AddFactCostCommand.RaiseCanExecuteChanged();
            AddCostCommand.RaiseCanExecuteChanged();
            EditCostCommand.RaiseCanExecuteChanged();
            DeleteCostCommand.RaiseCanExecuteChanged();
        }

        private void FillDataRow(EntitySummaryRow parentRow, List<GasCostDTO> data, List<GasCostDTO> result)
        {
            foreach (var childRow in parentRow.Items)
            {
                data.Where(dto => dto.Entity.Id == childRow.Id).ForEach(x => result.Add(x));
                FillDataRow(childRow, data, result);
            }
        }

        private void Delete(GasCostDTO cost)
        {
            if (cost.Id == 0) return;
            MessageBoxProvider.Confirm("Удалить расход?", async b =>
            {
                if (!b) return;
                await new GasCostsServiceProxy().DeleteGasCostAsync(cost.Id);
                OnCostDeleted(cost.Id);
                // RefreshCosts(cost.Target);
            }, "Удаление расхода", "Удалить", "Отмена");
        }
        private void OnCostDeleted(int id)
        {
            var cost = Data.SingleOrDefault(item => item.Id == id);
            if (cost != null)
            {
                Data.Remove(cost);
                _totalSummaryRow.RemoveGasCost(cost);
                FillCurrentCosts(cost.Entity.Id);
            }
        }
#endregion

#region helpers
        public void BindStnToColumnsHeader(IEnumerable<ColumnDescription> columns)
        {
            columns.ForEach(e =>{ e.FullName = $"{e.FullName} [{e.CostType.ToString()}]";});
        }

        public virtual List<CostType> GetCostTypeCollection()
        {
            return new List<CostType>();
        }
        #endregion
    }
}