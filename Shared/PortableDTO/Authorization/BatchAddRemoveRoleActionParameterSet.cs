using System.Collections.Generic;
using GazRouter.DTO.Authorization.Role;

namespace GazRouter.DTO.Authorization
{
    public class BatchAddRemoveRoleActionParameterSet
    {
        public BatchAddRemoveRoleActionParameterSet()
        {
            ActionsToAdd = new List<RoleActionParameterSet>();
            ActionsToRemove = new List<RoleActionParameterSet>();
        }

        public List<RoleActionParameterSet> ActionsToAdd { get; set; }
        public List<RoleActionParameterSet> ActionsToRemove { get; set; }
    }
}