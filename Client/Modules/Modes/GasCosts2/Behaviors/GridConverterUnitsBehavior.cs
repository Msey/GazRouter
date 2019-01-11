using System.Collections.Generic;
using System.Windows;
using System.Windows.Interactivity;
using GazRouter.Modes.GasCosts2.Converters;
using Telerik.Windows.Controls;
namespace GazRouter.Modes.GasCosts2.Behaviors
{
    public class GridConverterUnitsBehavior : Behavior<UIElement>
    {
        private RadGridView _gridControl;
        private List<UnitConverter> _converters;

        protected override void OnAttached()
        {
            base.OnAttached();
            _gridControl = (RadGridView)AssociatedObject;
            _gridControl.Loaded += ControlOnLoaded;
        }
        protected override void OnDetaching()
        {
            base.OnDetaching();
            _gridControl.Loaded -= ControlOnLoaded;
        }
        private void ControlOnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            _converters = new List<UnitConverter>();
            foreach (var column in _gridControl.Columns)
            {
                if (column is GridViewDataColumn) {
                    var columnBase = column as GridViewDataColumn;
                    if (columnBase.DataMemberBinding.Converter == null) continue;
                    //
                    var converter = (UnitConverter)columnBase.DataMemberBinding.Converter;
                    _converters.Add(converter);
                }
            }
            var unitsConverter = _gridControl.DataContext as IUnitsConverter;
            unitsConverter.UnitsChanged = SetUnits;
        }
        private void SetUnits(int units)
        {
            _converters.ForEach(e => e.SetUnits(units));
        }
    }
}
