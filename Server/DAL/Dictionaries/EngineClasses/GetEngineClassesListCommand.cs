using System.Collections.Generic;
using GazRouter.DAL.Core;
using GazRouter.DTO.Dictionaries.EngineClasses;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.Dictionaries.EngineClasses
{
	public class GetEngineClassesListQuery : QueryReader<List<EngineClassDTO>>
	{
		public GetEngineClassesListQuery(ExecutionContext context)
			: base(context)
		{
		}

		protected override string GetCommandText()
		{
		    return
                @"SELECT	    engine_class_id
	                                    ,engine_class_name
	                                    ,description
	                                    ,sort_order
                            FROM	    V_ENGINE_CLASSES";
		}

        protected override List<EngineClassDTO> GetResult(OracleDataReader reader)
		{
			var result = new List<EngineClassDTO>();
			while (reader.Read())
			{
				var periodType =
				   new EngineClassDTO
				   {
                       Id = reader.GetValue<int>("engine_class_id"),
                       Name = reader.GetValue<string>("engine_class_name"),
					   Description = reader.GetValue<string>("DESCRIPTION"),
					   SortOrder = reader.GetValue<int>("SORT_ORDER")
				   };
				result.Add(periodType);
			}
			return result;
		}
	}
}