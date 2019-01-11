using System;
using System.Linq;

namespace GazRouter.Prediction
{
    /// <summary>
    /// Класс параметров линейной регрессионной модели
    /// </summary>
    public class ParametersLinearRegr
    {
        private readonly ConsumptionValueList _convalList;
        private readonly int _index = -1;

        public ParametersLinearRegr(ConsumptionValueList convalList, DateTime day) // constructor
        {
            _convalList = convalList;
            var consVal = _convalList.First(cv => cv.Date == day);
            if ((consVal) != null)
                _index = _convalList.IndexOf(consVal);
        }

        public ParametersLinearRegr(ConsumptionValueList connvalList, int index) // constructor
        {
            _convalList = connvalList;
            _index = index;
        }

        /// <summary>
        /// Метка времени
        /// </summary>
        public DateTime Dt
        {
            get { return _index > 0 ? _convalList[_index].Date : new DateTime(1900, 01, 01); }
        }

        /// <summary>
        /// "1"
        /// </summary>
        public double One
        {
            get { return _index > 0 ? 1 : 0; } // если _index > 0, то вернуть 1, иначе вернуть 0
        }

        /// <summary>
        /// Температура наружного воздуха за текущий день
        /// </summary>
        public double T0
        {
            get { return _index > 0 ? _convalList[_index].TemperatureAir : 0; } // если _index > 0, то вернуть _consList[_index].TemperatureAir, иначе вернуть 0
        }

        /// <summary>
        /// Объем газопотребления за текущий день
        /// </summary>
        public double Q0
        {
            get { return _index > 0 ? _convalList[_index].Volume : 0; }
        }
            
        /// <summary>
        ///  Объем газопотребления за предыдущий день
        /// </summary>
        public double Q1
        {
            get { return _index > 0 ? _convalList[_index - 1].Volume : 0; } // если _index > 0, то вернуть _consList[_index - 1].TemperatureAir, иначе вернуть 0
        }
    }
}