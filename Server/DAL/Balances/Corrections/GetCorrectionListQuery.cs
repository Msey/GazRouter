using System.Collections.Generic;
using GazRouter.DAL.Core;
using GazRouter.DTO.Balances.Corrections;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.Balances.Corrections
{

	public class GetCorrectionListQuery : QueryReader<int, List<CorrectionDTO>>
	{
		public GetCorrectionListQuery(ExecutionContext context)
			: base(context)
		{ }

        protected override string GetCommandText(int parameters)
		{

            return @"   SELECT      c.value_id,
                                    c.doc_id,
                                    c.value_corrections  
                        FROM        rd.v_bl_value_corrections c
                        INNER JOIN  v_bl_values v ON v.value_id = c.value_id
                        WHERE       v.contract_id = :contractid";
		}

        protected override void BindParameters(OracleCommand command, int parameters)
        {
            command.AddInputParameter("contractid", parameters);
        }

	    protected override List<CorrectionDTO> GetResult(OracleDataReader reader, int parameters)
	    {
	        var result = new List<CorrectionDTO>();

	        while (reader.Read())
	        {
	            result.Add(
	                new CorrectionDTO
                    {
                        BalanceValueId = reader.GetValue<int>("value_id"),
	                    DocId = reader.GetValue<int>("doc_id"),
	                    Value = reader.GetValue<double>("value_corrections")
                    });
	        }

            return result;
        }

	    
	}
}
