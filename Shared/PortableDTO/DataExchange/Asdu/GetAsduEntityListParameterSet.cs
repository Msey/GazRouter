using System;
using System.Collections.Generic;

namespace GazRouter.DTO.DataExchange.Asdu
{
    public class GetAsduEntityListParameterSet
    {
        public GetAsduEntityListParameterSet()
        {
            AsduIdList = new List<Guid>();
        }

        public List<Guid> AsduIdList { get; set; }
    }
}