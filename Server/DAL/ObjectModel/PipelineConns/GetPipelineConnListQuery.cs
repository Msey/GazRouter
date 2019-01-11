using System;
using System.Collections.Generic;
using System.Text;
using GazRouter.DAL.Core;
using GazRouter.DTO.Dictionaries.PipelineEndType;
using GazRouter.DTO.ObjectModel.PipelineConns;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.ObjectModel.PipelineConns
{

    public class GetPipelineConnListQuery : QueryReader<GetPipelineConnListParameterSet, List<PipelineConnDTO>>
    {
        public GetPipelineConnListQuery(ExecutionContext context)
            : base(context)
        { }

        protected override string GetCommandText(GetPipelineConnListParameterSet parameters)
        {
            var sb = new StringBuilder(
               @"select pc.PIPELINE_CONN_ID ID, pc.END_TYPE_ID,
pc.PIPELINE_ID, 
pc.DEST_PIPELINE_ID, pc.KILOMETER, pc.END_TYPE_NAME, pc.distr_station_id, 
NVL(NVL(pc.DEST_PIPELINE_NAME,pc.DISTR_STATION_NAME),pc.COMP_SHOP_NAME) as NAME, 
NVL(NVL(pt.Pipeline_Type_NAme, n1.Entity_name), n.Entity_name) as Description,
pc.comp_shop_id  
from V_PIPELINE_CONNS pc 
left join V_Pipelines p on p.pipeline_id = pc.DEST_PIPELINE_ID
left join Rd.V_Pipeline_Types pt on pt.Pipeline_Type_Id = p.Pipeline_Type_Id
left join V_NM_ALL n on n.entity_id = pc.comp_shop_id
left join V_NM_ALL n1 on n1.entity_id = pc.distr_station_id where 1=1 ");

            if (parameters != null)
            {
                if (parameters.PipelineId.HasValue)
                {
                    sb.Append(" and pc.PIPELINE_ID = :pipelineId");
                }

                if (parameters.GasTrasportSystemId.HasValue)
                {
                    sb.Append(" and pc.system_ID = :systemId");
                }

            }
            var res = sb.ToString();
            return res;
         

        }

        protected override void BindParameters(OracleCommand command, GetPipelineConnListParameterSet parameters)
        {
            if (parameters == null) return;

            if (parameters.PipelineId.HasValue)
                command.AddInputParameter("pipelineId", parameters.PipelineId);
            if (parameters.GasTrasportSystemId.HasValue)
                command.AddInputParameter("systemId", parameters.GasTrasportSystemId);

        }

        protected override List<PipelineConnDTO> GetResult(OracleDataReader reader, GetPipelineConnListParameterSet parameters)
        {
            var conns = new List<PipelineConnDTO>();
            while (reader.Read())
            {
                var distrStationId = reader.GetValue<Guid?>("distr_station_id");
                var compShopId = reader.GetValue<Guid?>("comp_shop_id");
                conns.Add(new PipelineConnDTO
                {
                    Id = reader.GetValue<int>("ID"),
					EndTypeId = reader.GetValue<PipelineEndType>("END_TYPE_ID"),
                    EndTypeName = reader.GetValue<string>("END_TYPE_NAME"),
                    DestPipelineId = reader.GetValue<Guid?>("DEST_PIPELINE_ID"),
                    CompShopId = compShopId,
                    Name = reader.GetValue<string>("NAME"),
					DistrStationId = distrStationId,
                    PipelineId = reader.GetValue<Guid>("PIPELINE_ID"),
                    Kilometr = reader.GetValue<double?>("KILOMETER"),
                    Description = reader.GetValue<string>("Description")
                });
            }
            return conns;
        }
    }
}
