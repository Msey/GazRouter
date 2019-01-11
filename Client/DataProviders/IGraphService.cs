using System;
using System.ServiceModel;
using GazRouter.DTO.Infrastructure.Faults;
using System.Threading.Tasks;
      
// ReSharper disable once CheckNamespace
namespace GazRouter.DataProviders.Graph  
{
    [ServiceContract]
    public interface IGraphService
    {                   
            }

	public interface IGraphServiceProxy
	{

            }

    public sealed class GraphServiceProxy : DataProviderBase<IGraphService>, IGraphServiceProxy
	{
        protected override string ServiceUri => "/Graph/GraphService.svc";
      


            }
}
