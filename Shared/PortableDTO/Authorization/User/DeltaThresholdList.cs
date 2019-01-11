using System;
using System.Collections.Generic;
using System.Linq;
using GazRouter.DTO.Dictionaries.PhisicalTypes;

namespace GazRouter.DTO.Authorization.User
{
    public class DeltaThresholdList : List<DeltaThreshold>
    {
        public DeltaThresholdList()
        {
             
        }

        public DeltaThresholdList(List<DeltaThreshold> list)
        {
            AddRange(list);
        }

        public DeltaThreshold GetThreshold(PhysicalType type)
        {
            return this.SingleOrDefault(t => t.PhysicalType == type);
        }

        public ValueDeltaType CheckDelta(PhysicalType type, double value, double prevValue)
        {
            var th = this.SingleOrDefault(t => t.PhysicalType == type);
            if (th == null) return ValueDeltaType.None;

            var delta = Math.Abs(th.IsPercentage ? value/prevValue*100.0 - 100 : prevValue - value); 

            if (delta > th.WarnThreshold) return ValueDeltaType.Warn;
            if (delta > th.ShowThreshold) return ValueDeltaType.Show;
            return ValueDeltaType.None;
        }
    }
}