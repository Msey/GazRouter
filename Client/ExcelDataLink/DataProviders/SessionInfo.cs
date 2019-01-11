using System;
using System.Collections.Generic;
using GazRouter.DTO;

namespace DataProviders
{
    public static class SessionInfo
    {
        public static Dictionary<Module, Guid> ModulesStates { get; private set; }

        static SessionInfo()
        {
            ModulesStates = new Dictionary<Module, Guid>();
            foreach (var module in Enum.GetValues(typeof(Module)))
            {
                ModulesStates.Add((Module)module, Guid.Empty);
            }
        }

    }
}