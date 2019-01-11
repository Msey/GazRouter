using System;
using System.ServiceModel;
using GazRouter.DTO.Infrastructure.Faults;
using System.Threading.Tasks;
      
// ReSharper disable once CheckNamespace
namespace DataProviders.Graph  
{
    [ServiceContract]
    public interface IGraphService
    {                   
            }


    public class GraphServiceProxy : DataProviderBase<IGraphService>
	{
        protected override string ServiceUri
        {
            get { return "/Graph/GraphService.svc"; }
        }

            }
}
