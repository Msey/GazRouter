using System;
using System.Collections.Generic;
using GazRouter.DTO.DataLoadMonitoring;

namespace DataProviders.DataLoadMonitoring
{
    public class DataLoadMonitoringDataProvider : DataProviderBase<IDataLoadService>
    {
        protected override string ServiceUri
        {
            get { return "/DataLoadMonitoring/DataLoadService.svc"; }

        }
        public void GetDataLoadSiteStatistics(DateTime dt, Func<List<SiteDataLoadStatistics>, Exception, bool> callback, IClientBehavior behavior)
        {
            var channel = GetChannel();
            Execute(channel, channel.BeginGetDataLoadSiteStatisticsTechData, channel.EndGetDataLoadSiteStatisticsTechData, callback,dt , behavior);
        }

        public void GetTechDataBySite(EntityPropertyValueParameterSet parameters, Func<List<BaseEntityProperty>, Exception, bool> callback, IClientBehavior behavior)
        {
            var channel = GetChannel();
            Execute(channel,channel.BeginGetSiteTechData,channel.EndGetSiteTechData,callback,parameters,behavior);
        }
        public void GetChangeModeData(GasModeChangeParameterSet parameters, Func<GasModeChangeData, Exception, bool> callback, IClientBehavior behavior)
        {
            var channel = GetChannel();
            Execute(channel, channel.BeginGetGasModeChangeData, channel.EndGetGasModeChangeData, callback, parameters, behavior);
        }

        public void GetChangeModeLastData( Func<GasModeChangeData, Exception, bool> callback, IClientBehavior behavior)
        {
            var channel = GetChannel();
            Execute(channel, channel.BeginGetGasModeChangeDataLastSerie, channel.EndGetGasModeChangeDataLastSerie, callback,  behavior);
        }

        public void GetGasSupplyDataSet(int systemId, Func<GasSupplyDataSetDTO, Exception, bool> callback, IClientBehavior behavior)
        {
            var channel = GetChannel();
            Execute(channel, channel.BeginGetGasSupplyDataSet, channel.EndGetGasSupplyDataSet, callback, systemId, behavior);
        }

        public void GetGasSupplyValues(DateTime dt, Func<List<GasSupplyValue>, Exception, bool> callback, IClientBehavior behavior)
        {
            var channel = GetChannel();
            Execute(channel, channel.BeginGetGasSupplyValues, channel.EndGetGasSupplyValues, callback, dt, behavior);
        }

        public void GetSumGasSupplyValuesByEnterprise(GasSupplySumParameterSet param, Func<List<GasSupplySumValueDTO>, Exception, bool> callback, IClientBehavior behavior)
        {
            var channel = GetChannel();
            Execute(channel, channel.BeginGetSumGasSupplyValuesByEnterprise, channel.EndGetSumGasSupplyValuesByEnterprise, callback, param, behavior);
        }

    }

    
}
