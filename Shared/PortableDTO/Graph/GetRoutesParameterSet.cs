using System;

namespace GazRouter.DTO.Graph
{
    public class GetRoutesParameterSet
    {
        public Guid StartPointId { get; set; }
        public Guid EndPointId { get; set; }
    }
}
