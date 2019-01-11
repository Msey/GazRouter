using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using GazRouter.Common.ViewModel;
using GazRouter.DTO.GasCosts;
using GazRouter.DTO.ManualInput.InputStates;
using Microsoft.Practices.ObjectBuilder2;
using Microsoft.Practices.Prism;
using Microsoft.Practices.Prism.Commands;
using Utils.Extensions;
namespace GazRouter.Modes.GasCosts2
{
    public class ChartItem : PropertyChangedBase
    {
        private DateTime _date;
        public DateTime Date
        {
            get { return _date; }
            set { SetProperty(ref _date, value); }
        }

        private double? _calculatedVolume;
        public double? CalculatedVolume
        {
            get { return _calculatedVolume; }
            set { SetProperty(ref _calculatedVolume, value); }
        }
    }
    public class CostItem : PropertyChangedBase
    {
        public CostItem(GasCostDTO costDTO)
        {
            GasCost = costDTO;
        }

        private GasCostDTO _gasCost;
        public GasCostDTO GasCost
        {
            get { return _gasCost; }
            set { SetProperty(ref _gasCost, value); }
        }
        public double? CalculatedVolume
        {
            get { return _gasCost.CalculatedVolume; }
            set {
                _gasCost.CalculatedVolume = value;
                OnPropertyChanged(()=> CalculatedVolume);
            }
        }
        public double? MeasuredVolume
        {
            get { return _gasCost.MeasuredVolume; }
            set {
                _gasCost.MeasuredVolume = value;
                OnPropertyChanged(() => MeasuredVolume);
            }
        }
        public void UpdateProperty()
        {
            OnPropertyChanged(() => CalculatedVolume);
            OnPropertyChanged(() => MeasuredVolume);
        }
    }
    public class CostsParameters
    {
        public CostsParameters(bool isEditPermission, 
                               Action addCostCommand, 
                               Action<CostItem> editCostCommand, 
                               Action<CostItem> deleteCostCommand)
        {
            IsEditPermission  = isEditPermission;
            AddCostCommand    = addCostCommand;
            EditCostCommand   = editCostCommand;
            DeleteCostCommand = deleteCostCommand;
        }
        public bool IsEditPermission { get; }
        public Action AddCostCommand { get; }
        public Action<CostItem> EditCostCommand { get; }
        public Action<CostItem> DeleteCostCommand { get; }
    }
    public class CostsViewModel : GasCostViewModelBase
    {
#region constructor
        public CostsViewModel(CostsParameters costsParameters)
        {
            _costsParameters = costsParameters;
            DayCosts         = new ObservableCollection<CostItem>();
            ChartData        = new ObservableCollection<ChartItem>();
            InitCommands();
        }
#endregion
#region variables
        private readonly CostsParameters _costsParameters;
        private List<ChartItem> _result;
        private int? _currentRegular;
        private bool _objectSelected;
        private List<GasCostDTO> _monthCosts;
        private ObjectItem _objectItem;
#endregion
#region property
        private ObservableCollection<CostItem> _dayCosts;
        public ObservableCollection<CostItem> DayCosts
        {
            get { return _dayCosts; }
            set
            {
                SetProperty(ref _dayCosts, value);
            }
        }

        private CostItem _selectedCost;
        public CostItem SelectedCost
        {
            get { return _selectedCost; }
            set
            {
                SetProperty(ref _selectedCost, value);
                EditCostCommand.RaiseCanExecuteChanged();
                DeleteCostCommand.RaiseCanExecuteChanged();
            }
        }

        private bool _isAccessAllowed;
        public bool IsAccessAllowed
        {
            get { return _isAccessAllowed; }
            set { SetProperty(ref _isAccessAllowed, value); }
        }

        private ObservableCollection<ChartItem> _chartData;
        public ObservableCollection<ChartItem> ChartData
        {
            get { return _chartData; }
            set { SetProperty(ref _chartData, value); }
        }
#endregion
#region events
        /// <summary> вызывается при выборе ячейки строки объекта 
        /// 
        /// значение ввода должно проверяться при каждом переключении на новый объект!
        /// 
        /// Is_regular = 1 
        /// 
        /// a) может быть введено только одно значение для объекта
        /// б) возможен ввод вручную - измеренного значения для единчтвенной позиции объекта
        /// 
        /// Is_regular != 1 
        /// 
        /// a) ввод вручную не доступен
        /// b) можно вводить несколько значений для объекта
        /// 
        /// </summary>
        /// <param name="regular"></param>
        /// <param name="objectSelected"></param>
        /// <param name="month"></param>
        public void UpdateAddCommandCanExecute(int? regular, bool objectSelected)
        {
            _currentRegular = regular;
            _objectSelected = objectSelected;
            AddCostCommand.RaiseCanExecuteChanged();
        }
#endregion
#region commands
        public DelegateCommand AddCostCommand { get; set; }
        public DelegateCommand EditCostCommand
        {
            get; set;
        }
        public DelegateCommand DeleteCostCommand { get; set; }

        private void InitCommands()
        {
            AddCostCommand    = new DelegateCommand(() => 
                _costsParameters.AddCostCommand.Invoke(), CanAddCostCommand);
            EditCostCommand   = new DelegateCommand(() => 
                _costsParameters.EditCostCommand.Invoke(SelectedCost), CanEditCostCommand);
            DeleteCostCommand = new DelegateCommand(() => 
                _costsParameters.DeleteCostCommand.Invoke(SelectedCost), CanDeleteCostCommand);
        }
        /// <summary>
        /// добавление нового расхода происходит при:
        ///     выделении любой ячейки строки объекта 
        ///         и
        ///     соблюдении условий 
        /// 
        /// </summary>
        /// <returns></returns>
        private bool CanAddCostCommand()
        {
            if (!_objectSelected) return false;
            if (_currentRegular == 1 && DayCosts.Count == 0)     return true;
            if (_currentRegular == null || _currentRegular == 0) return true;
            return false;
        }
        private bool CanEditCostCommand()
        {
            return SelectedCost != null;
        }
        private bool CanDeleteCostCommand()
        {
            return SelectedCost != null;
        }
#endregion
 #region methods
        public void SetAccessAllowed(ManualInputState inputState)
        {
            IsAccessAllowed = inputState == ManualInputState.Input && _costsParameters.IsEditPermission;
        }
        public void UpdateCosts(EntityRowBase entityRowBase, DateTime month)
        {
            if (entityRowBase == null)
            {
                _objectSelected = false;
                return;
            }
            Clear();
            var objectItem = entityRowBase as ObjectItem;
            if (objectItem == null) throw new Exception();
            //
            _objectItem = objectItem;
            UpdateCosts();
            _objectSelected = true;
            // chart              
            UpdateChart(month);
        }
        private void UpdateCosts()
        {
            var dayCosts = _objectItem.GetDayCosts();
            _monthCosts  = _objectItem.GetMonthCosts();
            if (DayCosts != null && DayCosts.Count > 0) DayCosts.Clear();
            DayCosts.AddRange(dayCosts.Select(e => new CostItem(e)));
        }
        public void Clear()
        {
            _objectSelected = false;
            SelectedCost    = null;
            if (ChartData.Count > 0) ChartData.Clear();
            if (DayCosts != null && DayCosts.Count > 0) DayCosts.Clear();
            AddCostCommand.RaiseCanExecuteChanged();
        }
        public void RemoveCost(int id)
        {
            _objectItem.RemoveCost(id);
            UpdateCosts();
            AddCostCommand.RaiseCanExecuteChanged();
        }
        public override void UpdateUnits(int units)
        {
            // table
            UnitsChanged?.Invoke(units);
            DayCosts.ForEach(item => item.UpdateProperty());
            // chart
            if(_result == null) return;
            UpdateChartData(_result);
        }
        public void UpdateChart(DateTime selectedMonth)
        {
            if (_monthCosts == null) return;
            //
            if (_monthCosts.Count == 0)
            {
                if (ChartData.Count > 0) ChartData.Clear();
                return;
            }
            //
            _result = new List<ChartItem>();
            var month = selectedMonth;
            for (var day = month.MonthStart(); day <= month.MonthEnd(); day = day.AddDays(1))
            {
                _result.Add(new ChartItem
                {
                    Date = day,
                    CalculatedVolume = _monthCosts.Any(c => c.Date.Date == day) ?
                          _monthCosts.Where(c => c.Date.Date == day)
                                     .Sum(c => c.Volume) : (double?) null
                });
            }
            UpdateChartData(_result);
        }
        private void UpdateChartData(IEnumerable<ChartItem> result)
        {
            var data  = result.Select(e => new ChartItem
            {
                Date = e.Date,
                CalculatedVolume = e.CalculatedVolume * Units
            });
            if (ChartData.Count > 0) ChartData.Clear();
            ChartData.AddRange(data);
        }
#endregion
    }
}
#region trash

//        private DelegateCommand _addCostCommand;
//        private DelegateCommand _editCostCommand;
//        private DelegateCommand _deleteCostCommand;


//            DayCosts.Remove(dayCost);//            _monthCosts.Remove(monthCost);
//            var dayCost    = DayCosts.Single(e=>e.GasCost.Id == id);

//            _monthCosts      = new List<GasCostDTO>();
//            DayCosts         = new ObservableCollection<CostItem>();

//private IEnumerable<double> GetChartCalculatedVolume(int day)
//{
//
//    return DayCosts.Any(c => c.GasCost.Date.Date == day) ?
//                  DayCosts.Where(c => c.GasCost.Date.Date == day)
//                       .Sum(c => c.GasCost.Volume) * Units :
//                  (double?)null;
//}

//            _result.ForEach(e => { e.CalculatedVolume = e.CalculatedVolume*Units; });

//        private bool _manualInputAccess;
//        public bool ManualInputAccess
//        {
//            get { return _manualInputAccess; }
//            set { SetProperty(ref _manualInputAccess, value); }
//        }

//            get
//            {
//                return _deleteCostCommand ;
//                    ??
//                       (_deleteCostCommand =
//                           new DelegateCommand(() => Delete(SelectedCost),
//                               () => SelectedCost != null && EditPermission && _mainViewModel.IsAccessAllowed()
//                               && ((_mainViewModel.ShowDayly() && Target == Target.Fact)
//                                    || (!_mainViewModel.ShowDayly() && (Target == Target.Norm || Target == Target.Plan)))));
//            }

//        public void AddNewCost()
//        {
//            
//        }
//        public void EditCost(GasCostDTO cost)
//        {
//            var c = DayCosts.Single(e => e.Id == id);
//            DayCosts.Remove(cost);
//        }

//            {
//                return _editCostCommand ;
//                ??
//                       (_editCostCommand =
//                           new DelegateCommand(() => Edit(SelectedCost),
//                               () => SelectedCost != null && EditPermission && _mainViewModel.IsAccessAllowed()
//                               && ((_mainViewModel.ShowDayly() && Target == Target.Fact)
//                                    || (!_mainViewModel.ShowDayly() && (Target == Target.Norm || Target == Target.Plan)))));
// }{

//                return _addCostCommand ??
//                       (_addCostCommand = new DelegateCommand(() => 
//                            AddNewCost(_costsParameters.GetSelectedCellTarget()), 
//                            CanAddCostCommand));
//            }

//_costsParameters.IsRowSelected();
/*Target target*/

//            if (target == Target.None)
//            {
//                throw new ArgumentException(string.Empty, nameof(target));
//            }

// public List<GasCostDTO> DayCosts => _currentCosts?.Where(c => c.Target == Target).OrderBy(c => c.Date).ToList();
#endregion