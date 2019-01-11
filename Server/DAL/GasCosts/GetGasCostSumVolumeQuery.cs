using System;
using System.Collections.Generic;
using System.Text;
using GazRouter.DAL.Core;
using GazRouter.DTO.Dictionaries.EntityTypes;
using GazRouter.DTO.Dictionaries.Targets;
using GazRouter.DTO.GasCosts;
using GazRouter.DTO.ObjectModel;
using Oracle.ManagedDataAccess.Client;
using Utils.Extensions;

namespace GazRouter.DAL.GasCosts
{
    public class GetGasCostSumVolumeQuery : QueryReader<GetGasCostSumVolumeParameterSet, List<GasCostDTO>>
    {
        public GetGasCostSumVolumeQuery(ExecutionContext context) : base(context)
        {
        }

        protected override string GetCommandText(GetGasCostSumVolumeParameterSet parameters)
        {
            return $@" SELECT   sum(ac.volume) vol
                    FROM        v_aux_costs ac       
                    WHERE       1=1
                    {GetWhereClause(parameters)}
                    {GetGroupByClause(parameters)}";
        }

        private string GetGroupByClause(GetGasCostSumVolumeParameterSet parameters)
        {
            return parameters.SiteId.HasValue ? " group by ac.site_id " 
                : parameters.EntityId.HasValue ? " group by ac.entity_id " : " ";
        }

        private string GetWhereClause(GetGasCostSumVolumeParameterSet parameters)
        {
            var sb = new StringBuilder();
            if (parameters.Target.HasValue)
                sb.Append(" AND ac.target_id = :targetId ");
            if (parameters.EntityId.HasValue)
                sb.Append(" AND ac.entity_id = :entityId ");
            if (parameters.SiteId.HasValue)
                sb.Append(" AND ac.site_id = :siteId ");
            if (parameters.CostType.HasValue)
                sb.Append(" AND ac.aux_item_id = :costTypeId ");

            sb.Append(@" and AUX_DATE between :begin and :end ");
            return sb.ToString();
        }

        protected override void BindParameters(OracleCommand command, GetGasCostSumVolumeParameterSet parameters)
        {
            if (parameters.Target.HasValue)
                command.AddInputParameter("targetId", parameters.Target.Value);

            if (parameters.EntityId.HasValue)
                command.AddInputParameter("entityId", parameters.EntityId.Value);

            if (parameters.SiteId.HasValue)
                command.AddInputParameter("siteId", parameters.SiteId.Value);

            if (parameters.CostType.HasValue)
                command.AddInputParameter("costTypeId", parameters.CostType.Value);


            command.AddInputParameter("begin", parameters.BeginDate);
            command.AddInputParameter("end", parameters.EndDate);
        }

        protected override List<GasCostDTO> GetResult(OracleDataReader reader, GetGasCostSumVolumeParameterSet parameters)
        {
            var result = new List<GasCostDTO>();
            while (reader.Read())
            {
                result.Add(new GasCostDTO
                {
                    MeasuredVolume = reader.GetValue<double?>("vol"), 
                });
            }
            return result;
        }
    }
}