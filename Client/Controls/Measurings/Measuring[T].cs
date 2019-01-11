using System;
using GazRouter.DTO.Dictionaries.PeriodTypes;
using GazRouter.DTO.Dictionaries.PropertyTypes;

namespace GazRouter.Controls.Measurings
{
    public class Measuring<T> : MeasuringBase
    {
        private T _value;

        public Measuring(Guid entityId, PropertyType propType, PeriodType periodType)
            : base(entityId, propType, periodType)
        {
        }

        public T Value
        {
            get { return _value; }
            protected set
            {
                if (SetProperty(ref _value, value))
                {
                    OnPropertyChanged(() => DisplayValue);
                }
            }
        }

        /// <summary>
        ///     Текущее значение
        /// </summary>
        public virtual string DisplayValue { get; set; }
    }
}