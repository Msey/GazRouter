using System;

namespace GazRouter.DTO.Balances.Routes
{
    public class SetRouteParameterSet
    {
        public Guid InletId { get; set; }
        public Guid OutletId { get; set; }
        public double? Length { get; set; }
    }
}
