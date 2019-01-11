using GazRouter.DataServices.Infrastructure.Attributes.Behaviors;
using GazRouter.DataServices.Infrastructure.Services;

namespace GazRouter.DataServices.Graph
{
    [ErrorHandlerLogger("mainLogger")]
    [Authorization]
    public class GraphService : ServiceBase, IGraphService
    {
        //public RouteDTO GetRoute(GetRouteListParameterSet parameters)
        //{
            
        //}
    }
}
