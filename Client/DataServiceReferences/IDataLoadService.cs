using System;
using System.ServiceModel;
using DTO.Infrastructure.Faults;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using DTO.DataLoadMonitoring;
      
namespace DataServiceReferences.DataLoadMonitoring  
{
    [ServiceContract]
    public interface IDataLoadService
    {                   
        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginGetDataLoadSiteStatisticsTechData(DateTime dt, AsyncCallback callback, Object state);
        List<SiteDataLoadStatistics> EndGetDataLoadSiteStatisticsTechData(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        [ServiceKnownType(typeof(EntityPropertyValueStringDTO))] 
        [ServiceKnownType(typeof(EntityPropertyValueDateDTO))] 
        [ServiceKnownType(typeof(EntityPropertyValueDoubleDTO))] 
        IAsyncResult BeginGetSiteTechData(EntityPropertyValueParameterSet parameters, AsyncCallback callback, Object state);
        List<BaseEntityProperty> EndGetSiteTechData(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        [ServiceKnownType(typeof(BaseGasModeChange))] 
        [ServiceKnownType(typeof(GasModeChangeStringDTO))] 
        [ServiceKnownType(typeof(GasModeChangeDateDTO))] 
        [ServiceKnownType(typeof(GasModeChangeDoubleDTO))] 
        IAsyncResult BeginGetGasModeChangeData(GasModeChangeParameterSet parameters, AsyncCallback callback, Object state);
        GasModeChangeData EndGetGasModeChangeData(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        [ServiceKnownType(typeof(BaseGasModeChange))] 
        [ServiceKnownType(typeof(GasModeChangeStringDTO))] 
        [ServiceKnownType(typeof(GasModeChangeDateDTO))] 
        [ServiceKnownType(typeof(GasModeChangeDoubleDTO))] 
        IAsyncResult BeginGetGasModeChangeDataLastSerie(object parameters, AsyncCallback callback, Object state);
        GasModeChangeData EndGetGasModeChangeDataLastSerie(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginGetGasSupplyDataSet(int systemId, AsyncCallback callback, Object state);
        GasSupplyDataSetDTO EndGetGasSupplyDataSet(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginGetGasSupplyValues(DateTime dt, AsyncCallback callback, Object state);
        List<GasSupplyValue> EndGetGasSupplyValues(IAsyncResult result);
    }
}
