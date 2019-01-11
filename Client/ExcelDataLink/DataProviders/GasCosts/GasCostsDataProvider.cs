using System;
using System.Collections.Generic;
using GazRouter.DTO.GasCosts;

namespace DataProviders.GasCosts
{
    public class GasCostsDataProvider : DataProviderBase<IGasCostsService>
    {
        protected override string ServiceUri
        {
            get { return "/GasCosts/GasCostsService.svc"; }
        }

        public void GetDefaultParamValues(
            GetGasCostListParameterSet parameters,
            Func<List<DefaultParamValuesDTO>, Exception, bool> callback,
            IClientBehavior behavior)
        {
            var channel = GetChannel();
            Execute(channel, channel.BeginGetDefaultParamValues, channel.EndGetDefaultParamValues, callback, parameters,
                behavior);
        }

        public void SetDefaultParamValues(List<DefaultParamValuesDTO> parameter, Func<Exception, bool> callback,
            IClientBehavior behavior)
        {
            var channel = GetChannel();
            Execute(channel, channel.BeginSetDefaultParamValues, channel.EndSetDefaultParamValues, callback, parameter,
                behavior);
        }
    }
}