using System;
using System.Collections.Generic;
using System.Text;
using GazRouter.DAL.Core;
using GazRouter.DTO.Dictionaries.Enterprises;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.Dictionaries.Enterprises
{
	public class GetEnterpriseListQuery : QueryReader<Guid?, List<EnterpriseDTO>>
	{
		public GetEnterpriseListQuery(ExecutionContext context) : base(context)
		{
		}

		protected override string GetCommandText(Guid? parameters)
		{
			var sql = @"SELECT      e.enterprise_id, 
                                    e.enterprise_name, 
                                    e.is_gr, 
                                    e.code, 
                                    e.sort_order,
                                    e.asdu_code
                                     
                        FROM        v_enterprises e";

            var sb = new StringBuilder(sql);
		    if (parameters.HasValue)
		    {
		        sb.Append(@" WHERE e.enterprise_id IN 
                            (SELECT     neighbour_enterprise_id 
                                FROM    v_enterprise_neighbours 
                                WHERE   enterprise_id = :id)  
                            OR      enterprise_id = :id");
		    }
		    sb.Append(@" ORDER BY e.sort_order, e.enterprise_name");

            return sb.ToString();
		}


        protected override void BindParameters(OracleCommand command, Guid? parameters)
        {
            if (parameters.HasValue)
                command.AddInputParameter("id", parameters);
        }

        protected override List<EnterpriseDTO> GetResult(OracleDataReader reader, Guid? parameters)
		{
			var result = new List<EnterpriseDTO>();
			while (reader.Read())
			{
				result.Add(new EnterpriseDTO
				{
					Id = reader.GetValue<Guid>("enterprise_id"),
					Name = reader.GetValue<string>("enterprise_name"),
                    Code = reader.GetValue<string>("code"),
					SortOrder = reader.GetValue<int>("sort_order"),
					IsGr = reader.GetValue<bool?>("is_gr"),
                    AsduCode = reader.GetValue<string>("asdu_code")
                });

			}
			return result;
		}
	}
}