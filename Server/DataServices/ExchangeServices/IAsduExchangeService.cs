using System.ServiceModel;

namespace GazRouter.DataServices.ExchangeServices
{
    [ServiceContract]
    public interface IAsduExchangeService
    {
        [OperationContract]
        string GetTask(string data);
    }
}
