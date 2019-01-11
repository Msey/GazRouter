using System;
using GazRouter.DAL.Core;
using GazRouter.DTO.ASDU;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.DispatcherTasks
{
    public class ManageLinkCommand : CommandScalar<LinkParams, int>
    {
        public ManageLinkCommand(ExecutionContext context)
            : base(context)
        {
            IsStoredProcedure = true;
        }

        protected override void BindParameters(OracleCommand command, LinkParams parameters)
        {
            OutputParameter = command.AddReturnParameter<int>("dummy");
            
            command.AddInputParameter("ciusentitykey0", parameters.IusType);
            command.AddInputParameter("ciusid0", parameters.IusId);
            command.AddInputParameter("casduentitykey0", parameters.AsduType);
            command.AddInputParameter("casduid0", parameters.AsduId);
            command.AddInputParameter("iaction0", parameters.LinkAction);
        }

        protected override string GetCommandText(LinkParams parameters)
        {
            switch (parameters.LinkAction)
            {
                case LinkAction.UnLinkObject:
                case LinkAction.LinkObject:
                    return $"integro.p_md_bindingdata.set_binding";
                case LinkAction.UnLinkMetaEntity:
                case LinkAction.LinkMetaEntity:
                case LinkAction.UnLinkMetaAttr:
                case LinkAction.LinkMetaAttr:
                case LinkAction.UnLinkMetaParam:
                case LinkAction.LinkMetaParam:
                    return $"integro.p_md_binding.set_meta_binding";
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
        }
    }
}