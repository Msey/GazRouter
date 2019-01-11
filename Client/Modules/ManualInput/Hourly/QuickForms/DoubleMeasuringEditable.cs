using System;
using GazRouter.Application;
using GazRouter.Controls.Measurings;
using GazRouter.DTO.Dictionaries.PeriodTypes;
using GazRouter.DTO.Dictionaries.PropertyTypes;

namespace GazRouter.ManualInput.Hourly.QuickForms
{
    public class DoubleMeasuringEditable : DoubleMeasuring
    {
        private Action<Guid, PropertyType, double> _updateAction;
        public DoubleMeasuringEditable(Guid entityId, PropertyType propType,
            Action<Guid, PropertyType, double> updateAction, int coef = 1)
            : base(entityId, propType, PeriodType.Twohours, true, true, coef)
        {
            _updateAction = updateAction;
        }

        public double? EditableValue
        {
            get { return ConvertAndRound(Value) ?? ConvertAndRound(PrevValue); }
            set
            {
                if (value.HasValue && value != UserValue)
                {
                    Value = UserProfile.ToServerUnits(value.Value, PropertyType.PropertyType);
                    if (PropertyType.PhysicalType.ValueMin.HasValue && Value < PropertyType.PhysicalType.ValueMin) Value = PropertyType.PhysicalType.ValueMin;
                    if (PropertyType.PhysicalType.ValueMax.HasValue && Value > PropertyType.PhysicalType.ValueMax * Coef) Value = PropertyType.PhysicalType.ValueMax * Coef;
                   
                    if (Value.HasValue)
                        _updateAction?.Invoke(EntityId, PropertyType.PropertyType, Value.Value / Coef);
                }
                
            }
        }

        public string DefaultPrecisionFormat { get { return "n" + PropertyType.PhysicalType.DefaultPrecision; } }

    }
}
