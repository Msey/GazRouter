using System;
using GazRouter.Common.ViewModel;
using GazRouter.Modes.GasCosts2.Converters;
using Microsoft.Practices.ObjectBuilder2;
namespace GazRouter.Modes.GasCosts2
{
    public class GasCostViewModelBase : LockableViewModel, IUnitsConverter
    {
        private int _units;
        public int Units
        {
            get { return _units; }
            set
            {
                SetProperty(ref _units, value);
                UpdateUnits(value);
            }
        }

        public Action<int> UnitsChanged { get; set; }
        public virtual void UpdateUnits(int units)
        {
        }
        public void Traversal<T>(T data, Action<T> action) where T : StateItem
        {
            action.Invoke(data);
            data.Items?.ForEach(item => Traversal((T)item, action));
        }
    }
}