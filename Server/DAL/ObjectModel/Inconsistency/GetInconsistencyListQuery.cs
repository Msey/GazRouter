using System;
using System.Collections.Generic;
using System.Text;
using GazRouter.DAL.Core;
using GazRouter.DTO.Dictionaries.InconsistencyTypes;
using GazRouter.DTO.ObjectModel.Inconsistency;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.ObjectModel.Inconsistency
{
	public class GetInconsistencyListQuery : QueryReader<Guid?, List<InconsistencyDTO>>
	{
		public GetInconsistencyListQuery(ExecutionContext context)
			: base(context)
		{
		}

        protected override string GetCommandText(Guid? parameters)
        {
            var q = @"  SELECT      t1.inconsistency_id,
                                    t1.inconsistency_type_id,
                                    t1.entity_id,
                                    n1.entity_name 
                        FROM        v_inconsistencies t1
                        LEFT JOIN   v_nm_short_all n1 ON t1.entity_id = n1.entity_id
                        WHERE       1=1";

            var sb = new StringBuilder(q);

            if (parameters.HasValue)
                sb.Append(" AND t1.entity_id = :entityId");

            return sb.ToString();
        }

	    protected override void BindParameters(OracleCommand command, Guid? parameters)
	    {
            if(parameters.HasValue)
                command.AddInputParameter("entityId", parameters.Value);
	    }

        protected override List<InconsistencyDTO> GetResult(OracleDataReader reader, Guid? parameters)
	    {
	        var result = new List<InconsistencyDTO>();
	        while (reader.Read())
	        {
				var inconsistency = new InconsistencyDTO
	            {
	                Id = reader.GetValue<Guid>("inconsistency_id"),
	                EntityId = reader.GetValue<Guid>("entity_id"),
	                InconsistencyTypeId = reader.GetValue<InconsistencyType>("inconsistency_type_id"),
					EntityName = reader.GetValue<string>("entity_name")
	            };
				result.Add(inconsistency);
	        }
	        return result;
	    }
	}
}