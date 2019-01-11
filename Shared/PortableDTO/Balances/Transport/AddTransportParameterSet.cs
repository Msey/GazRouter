using System;
using GazRouter.DTO.Dictionaries.BalanceItems;
using GazRouter.DTO.Dictionaries.EntityTypes;

namespace GazRouter.DTO.Balances.Transport
{
    public class AddTransportParameterSet
    {
        public int ContractId { get; set; }

        public int OwnerId { get; set; }

        public Guid InletId { get; set; }

        public Guid OutletId { get; set; }

        public BalanceItem BalanceItem { get; set; }

        public double Volume { get; set; }

        public double Length { get; set; }

        public int? RouteId { get; set; }
    }
}
