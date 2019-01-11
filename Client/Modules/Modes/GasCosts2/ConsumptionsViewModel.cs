using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using GazRouter.DTO.Dictionaries.Targets;
using Microsoft.Practices.ObjectBuilder2;
using Telerik.Windows.Controls;
namespace GazRouter.Modes.GasCosts2
{
    public class ConsumptionsParameters
    {
        public ConsumptionsParameters(bool isEditPermission, 
                                      Action<EntityRowBase> objectChanged, 
                                      Action<EntityRowBase, double> manualInput)
        {
            IsEditPermission = isEditPermission;
            ObjectChanged = objectChanged;
            ManualInput = manualInput;
        }
        public bool IsEditPermission { get; }
        public Action<EntityRowBase> ObjectChanged { get; }
        public Action<EntityRowBase, double> ManualInput { get; }
    }
    public class ConsumptionsViewModel : GasCostViewModelBase
    {
#region constructor
        public ConsumptionsViewModel(ConsumptionsParameters parameters)
        {
            _parameters = parameters;
            Rows = new ObservableCollection<EntityRowBase>();
        }
#endregion
#region variables
        private readonly ConsumptionsParameters _parameters;
        private EntityRowBase _capturedItem;
#endregion
#region property
        private ObservableCollection<EntityRowBase> _rows;
        public ObservableCollection<EntityRowBase> Rows
        {
            get { return _rows; }
            set { SetProperty(ref _rows, value); }
        }

        private EntityRowBase _selectedItem;
        public EntityRowBase SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                SetProperty(ref _selectedItem, value);
               _parameters.ObjectChanged.Invoke(SelectedItem);
            }
        }

        private Target _selectedTarget;
        public Target SelectedTarget
        {
            get { return _selectedTarget; }
            set { SetProperty(ref _selectedTarget, value); }
        }

        private bool _manualInputAccess;
        public bool ManualInputAccess
        {
            get { return _manualInputAccess; }
            set { SetProperty(ref _manualInputAccess, value); }
        }
#endregion
#region events
        public void OnTargetChanged(int cellIndex)
        {
            SelectedTarget = GetTarget(cellIndex);
        }
        public void OnInputMeasuredValue(double value)
        {
            if (value > 0)
                _parameters.ManualInput.Invoke(_capturedItem, value / Units);
        }
#endregion
#region methods
        public void UpdateSelectedItem()
        {
            OnPropertyChanged(()=>SelectedItem);
        }
        public void UpdateRows(IEnumerable<ObjectItem> entityRows)
        {
            UpdateRows(entityRows.Cast<EntityRowBase>());
        }
        public void UpdateRows(IEnumerable<EntityRowBase> entityRows)
        {
            Rows.AddRange(entityRows);
        }
        public void UpdateRow()
        {
            var objectItem = (ObjectItem) SelectedItem;
            objectItem.UpdateAll();
        }        
        public void Clear()
        {
            if (Rows.Count > 0) Rows.Clear();
            SelectedItem = null;
        }
        private static Target GetTarget(int columnIndex)
        {
            return columnIndex == 1 ? Target.Fact : Target.None;
        }
        public void SetCellManualInputAccess(bool manualInput, int? isRegular, int costsCount)
        {
            ManualInputAccess = _parameters.IsEditPermission &&   // permission
                                manualInput &&                    // manual input
                                isRegular == 1 && costsCount <= 1 // regular
                                ;
        }
        internal void CaptureSelectedRow()
        {
            _capturedItem = SelectedItem;
        }
        public EntityRowBase GetCapturedItem()
        {
            return _capturedItem;
        }
        public override void UpdateUnits(int units)
        {
            UnitsChanged?.Invoke(units);
            Rows.ForEach(item=> item.UpdateProperty());
        }
#endregion
    }
}
#region trash
#endregion
