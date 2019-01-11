using System;
using System.Collections.Generic;
using System.Text;
using GazRouter.DAL.Core;
using GazRouter.DTO.GasCosts;
using Oracle.ManagedDataAccess.Client;
namespace GazRouter.DAL.GasCosts
{
    public class GetGasCostVisibilityListQuery : QueryReader<Guid?, List<GasCostVisibilityDTO>>
    {
        public GetGasCostVisibilityListQuery(ExecutionContext context) : base(context)
        {
        }

        protected override string GetCommandText(Guid? parameters)
        {
            var sb = new StringBuilder(@" SELECT   
                                                       t.aux_item_id,
                                                       t.site_id,
                                                       t.visible
                                           FROM        V_AUX_ITEM_VISIBLES t
                                           WHERE       1=1");

            if (parameters.HasValue)
                sb.Append(" AND t.site_id = :siteId");

            return sb.ToString();
        }
        protected override void BindParameters(OracleCommand command, Guid? parameters)
        {
            if (parameters.HasValue)
                command.AddInputParameter("siteId", parameters.Value);
        }
        protected override List<GasCostVisibilityDTO> GetResult(OracleDataReader reader, Guid? parameters)
        {
            var result = new List<GasCostVisibilityDTO>();
            while (reader.Read())
            {
                result.Add(new GasCostVisibilityDTO
                {
                    SiteId     = reader.GetValue<Guid>("site_id"),
                    CostType   = reader.GetValue<int>("aux_item_id"),
                    Visibility = reader.GetValue<int?>("visible"),
                });
            }
            return result;
        }
    }
}
