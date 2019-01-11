using System.Collections.Generic;
using GazRouter.DAL.Core;
using GazRouter.DTO.Dictionaries.ParameterTypes;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.Dictionaries.ParameterTypes
{
	public class GetParameterTypesListQuery : QueryReader<List<ParameterTypeDTO>>
	{
		public GetParameterTypesListQuery(ExecutionContext context)
			: base(context)
		{
		}

		protected override string GetCommandText()
		{
		    return @"   SELECT  parameter_type_id,
                                sys_name,
                                parameter_type_name
                        FROM	v_parameter_types";
		}

		protected override List<ParameterTypeDTO> GetResult(OracleDataReader reader)
		{
			var result = new List<ParameterTypeDTO>();
			while (reader.Read())
			{
                result.Add(
                    new ParameterTypeDTO
				    {
					    Id = reader.GetValue<int>("parameter_type_id"),
					    Name = reader.GetValue<string>("parameter_type_name"),
					    SysName = reader.GetValue<string>("sys_name"),
				    });
				
			}
			return result;
		}
	}
}