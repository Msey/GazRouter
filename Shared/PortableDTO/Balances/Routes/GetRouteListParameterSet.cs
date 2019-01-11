using System;
using GazRouter.DTO.Dictionaries.EntityTypes;

namespace GazRouter.DTO.Balances.Routes
{
    public class GetRouteListParameterSet
    {
        public Guid? InletId { get; set; }

        public int? SystemId { get; set; }
    }
}
