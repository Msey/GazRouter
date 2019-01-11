using System;
using System.Collections.Generic;
using GazRouter.DTO;

namespace GazRouter.DataServices.Infrastructure.Sessions
{
    public static class ModelState
    {
        public static readonly Dictionary<Module, Guid> ModulesStates = new Dictionary<Module, Guid>();

        static ModelState()
        {
            foreach (var module in Enum.GetValues(typeof(Module)))
            {
                ModulesStates.Add((Module)module, Guid.NewGuid());
            }
        }

        public static void ChangeState(Module module)
        {
            var state = Guid.NewGuid();
            ModulesStates[module] = state;
        }
    }
}
