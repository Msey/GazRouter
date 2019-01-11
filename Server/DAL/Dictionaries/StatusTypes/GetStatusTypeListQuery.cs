using System.Collections.Generic;
using GazRouter.DAL.Core;
using GazRouter.DTO.Dictionaries.StatusTypes;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.Dictionaries.StatusTypes
{
	public class GetStatusTypeListQuery : QueryReader<List<StatusTypeDTO>>
	{
		public GetStatusTypeListQuery(ExecutionContext context)
			: base(context)
		{
		}

		protected override string GetCommandText()
		{
		    return @"   SELECT      s.status_type_id,
                                    s.status_type_name,
                                    s.status_type_code,
                                    sa.status_type_id_new
                        FROM	    v_status_types s
                        LEFT JOIN   v_allow_statuses sa ON sa.status_type_id_old = s.status_type_id
                        ORDER BY    s.status_type_id";
		}

		protected override List<StatusTypeDTO> GetResult(OracleDataReader reader)
		{
			var result = new List<StatusTypeDTO>();

            StatusTypeDTO statusDto = null;

			while (reader.Read())
			{
			    var statusId = reader.GetValue<int>("status_type_id");
			    if (statusId != statusDto?.Id)
			    {
                    statusDto = 
                        new StatusTypeDTO
                        {
                            Id = reader.GetValue<int>("status_type_id"),
                            Name = reader.GetValue<string>("status_type_name"),
                            Code = reader.GetValue<string>("status_type_code")
                        };
                    result.Add(statusDto);
                }

			    var allowedStatus = reader.GetValue<StatusType?>("status_type_id_new");
                if (allowedStatus.HasValue)
                    statusDto.AllowedStatusList.Add(allowedStatus.Value);

                
			}
			return result;
		}
	}
}