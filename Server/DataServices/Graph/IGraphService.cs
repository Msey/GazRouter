using System.ServiceModel;
using GazRouter.DataServices.Infrastructure.Attributes;

namespace GazRouter.DataServices.Graph
{
    [Service("Граф")]
    [ServiceContract]
    public interface IGraphService
    {

        //[ServiceAction("Получение списка маршрутов")]
        //[OperationContract]
        //RouteDTO GetRoute(GetRouteListParameterSet parameters);
    }
}
