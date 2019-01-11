using System;
using System.Runtime.Serialization;

namespace GazRouter.DTO.Balances.Routes.Exceptions
{
    public class EditRouteExceptionParameterSet : AddRouteExceptionParameterSet
    {
        public int RouteExceptionId { get; set; }
	}
}