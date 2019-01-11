using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GazRouter.DAL.Core;
using GazRouter.DTO.Dictionaries.EntityTypes;
using GazRouter.DTO.ObjectModel.MeasLine;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.ObjectModel.MeasLine
{

    public class GetMeasLineListQuery : QueryReader<GetMeasLineListParameterSet, List<MeasLineDTO>>
    {
        public GetMeasLineListQuery(ExecutionContext context)
            : base(context)
        {}

        protected override string GetCommandText(GetMeasLineListParameterSet parameters)
        {
            var sb = new StringBuilder(
                 @" SELECT      ml.meas_line_id, 
                                ml.entity_type_id, 
                                ml.status, 
                                ml.sort_order, 
                                ml.meas_line_name, 
                                ml.is_virtual, 
                                ml.meas_station_id, 
                                ml.pipeline_id, 
                                ml.pipelinename, 
                                ml.kilometer_conn, 
                                n.entity_name AS full_name, 
                                n1.entity_name AS short_name,
                                ml.system_id,
                                ml.sort_order,
                                ml.description,
                                ml.bal_name

                    FROM        rd.v_meas_lines ml
                    LEFT JOIN   v_nm_all n ON ml.meas_line_id = n.entity_id
                    LEFT JOIN   v_nm_short_all n1 ON ml.meas_line_id = n1.entity_id
                    WHERE       1=1");

            if (parameters != null)
            {
                if (parameters.SystemId.HasValue)
                    sb.Append(" AND ml.system_id = :systemId");

                if (parameters.MeasStationId.HasValue)
                    sb.Append(" AND ml.meas_station_id = :measStationId");

                if (parameters.MeasStationList != null && parameters.MeasStationList.Any())
                    sb.Append($" AND ml.meas_station_id IN {CreateInClause(parameters.MeasStationList.Count)}");
            }

            sb.Append(" ORDER BY ml.sort_order, ml.meas_line_name");

            return sb.ToString();
		}

        protected override void BindParameters(OracleCommand command, GetMeasLineListParameterSet parameters)
		{
            if (parameters == null)
                return;

            if (parameters.SystemId.HasValue)
                command.AddInputParameter("systemId", parameters.SystemId);

            if (parameters.MeasStationId.HasValue)
                command.AddInputParameter("measStationId", parameters.MeasStationId);

            if (parameters.MeasStationList != null && parameters.MeasStationList.Any())
            {
                for (var i = 0; i < parameters.MeasStationList.Count; i++)
                {
                    command.AddInputParameter($"p{i}", parameters.MeasStationList[i]);
                }
            }
		}

        protected override List<MeasLineDTO> GetResult(OracleDataReader reader, GetMeasLineListParameterSet parameters)
        {
            var measLines = new List<MeasLineDTO>();
            while (reader.Read())
            {
                measLines.Add(
                    new MeasLineDTO
                    {
                        Id = reader.GetValue<Guid>("meas_line_id"),
                        Name = reader.GetValue<string>("meas_line_name"),
                        IsVirtual = reader.GetValue<bool>("is_virtual"),
                        ParentId = reader.GetValue<Guid>("meas_station_id"),
						PipelineName = reader.GetValue<string>("pipelinename"),
						PipelineId = reader.GetValue<Guid>("pipeline_id"),
                        KmOfConn = reader.GetValue<double>("kilometer_conn"),
                        Path = reader.GetValue<string>("full_name"),
                        ShortPath = reader.GetValue<string>("short_name") ,
						SystemId = reader.GetValue<int>("system_id"),
						SortOrder = reader.GetValue<int>("sort_order"),
                        Status =  reader.GetValue<EntityStatus?>("status"),
                        Description = reader.GetValue<string>("description"),
                        BalanceName = reader.GetValue<string>("bal_name")
                    });
            }
            return measLines;
        }
    }
}
