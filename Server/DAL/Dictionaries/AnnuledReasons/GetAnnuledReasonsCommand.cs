using System.Collections.Generic;
using GazRouter.DAL.Core;
using GazRouter.DTO.Dictionaries.AnnuledReasons;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.Dictionaries.AnnuledReasons
{
	public class GetAnnuledReasonsListQuery : QueryReader<List<AnnuledReasonDTO>>
	{
		public GetAnnuledReasonsListQuery(ExecutionContext context)
			: base(context)
		{
		}

		protected override string GetCommandText()
		{
		    return
		        @"SELECT	    REASON_id
	                            ,text_template
                            FROM  V_ANNULED_REASONS";
		}

		protected override List<AnnuledReasonDTO> GetResult(OracleDataReader reader)
		{
			var result = new List<AnnuledReasonDTO>();
			while (reader.Read())
			{
				var periodType =
				   new AnnuledReasonDTO
				   {
					   Id = reader.GetValue<int>("REASON_id"),
					   Name = reader.GetValue<string>("text_template"),
				   };
				result.Add(periodType);
			}
			return result;
		}
	}
}