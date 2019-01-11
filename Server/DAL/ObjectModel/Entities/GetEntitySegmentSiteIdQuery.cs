using System;
using GazRouter.DAL.Core;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.ObjectModel.Entities
{
    public sealed class GetEntitySegmentSiteIdQuery : QueryReader<Guid, Guid?>
    {
        public GetEntitySegmentSiteIdQuery(ExecutionContext context)
            : base(context)
        {
        }

        protected override void BindParameters(OracleCommand command, Guid parameters)
        {
            command.AddInputParameter("entityId", parameters);
        }

        protected override string GetCommandText(Guid parameters)
        {
            return @"   SELECT  site_id 
                        FROM    v_entity_2_site_segments 
                        WHERE   entity_id = :entityId";
        }

        protected override Guid? GetResult(OracleDataReader reader, Guid parameters)
        {
            if (reader.Read())
            {
                return reader.GetValue<Guid>("site_id");
            }
            return null;
        }
    }
}