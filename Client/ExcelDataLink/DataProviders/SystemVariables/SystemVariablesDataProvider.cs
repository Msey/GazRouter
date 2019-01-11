using System;
using System.Collections.Generic;
using GazRouter.DTO.SystemVariables;

namespace DataProviders.SystemVariables
{
    [Obsolete("используйте Proxy классы ")]
    public class SystemVariablesDataProvider : DataProviderBase<ISysVarService>
	{
        protected override string ServiceUri
        {
            get { return "/SystemVariables/SysVarService.svc"; }
        }

        public void SystemVariablesListGet(Func<List<IusVariableDTO>, Exception, bool> callback, IClientBehavior behavior)
        {
            var channel = GetChannel();
            Execute(channel, channel.BeginGetIusVariableList, channel.EndGetIusVariableList, callback, behavior);
        }

        public void SystemVariablesEdit(IusVariableParameterSet parameters, Func<Exception, bool> callback, IClientBehavior behavior)
        {
            var channel = GetChannel();
            Execute(channel, channel.BeginEditIusVariableValue, channel.EndEditIusVariableValue, callback, parameters, behavior);
        }

	}
}
