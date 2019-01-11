using System;
using System.Collections.Generic;
using System.Linq;

namespace GazRouter.Prediction
{
    public class ConsumptionValueList : List<ConsumptionValue>
    {
        public ConsumptionValueList(IEnumerable<ConsumptionValue> list)
            : base(list)
        {

        }

        public ConsumptionValueList()
        {

        }                
        
        /// <summary>
        /// Функция возвращает только те дни, в которых газопотребление не равно 0
        /// </summary>
        /// <param name="dayX"></param>
        /// <returns></returns>
        public ConsumptionValueList NoZeroValues(DateTime dayX)
        {
            ConsumptionValueList cvlstNew = new ConsumptionValueList(this.Where(cv => cv.Date >= dayX));
            ConsumptionValueList cvlist = new ConsumptionValueList(this.Where(cv => (cv.Date < dayX && cv.Volume > 0)));
            cvlist.AddRange(cvlstNew);

            return cvlist;
        }

    }
}