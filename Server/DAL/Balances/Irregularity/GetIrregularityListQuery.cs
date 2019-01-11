using System.Collections.Generic;
using GazRouter.DAL.Core;
using GazRouter.DTO.Balances.Irregularity;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.Balances.Irregularity
{

	public class GetIrregularityListQuery : QueryReader<int, List<IrregularityDTO>>
	{
		public GetIrregularityListQuery(ExecutionContext context)
			: base(context)
		{ }

        protected override string GetCommandText(int parameters)
		{

            return @"   SELECT      i.value_id,
                                    i.start_day,
                                    i.end_day,
                                    i.value  
                        FROM        rd.v_bl_irregularity i
                        INNER JOIN  v_bl_values v ON v.value_id = i.value_id
                        WHERE       v.contract_id = :contractid";
		}

        protected override void BindParameters(OracleCommand command, int parameters)
        {
            command.AddInputParameter("contractid", parameters);
        }

	    protected override List<IrregularityDTO> GetResult(OracleDataReader reader, int parameters)
	    {
	        var result = new List<IrregularityDTO>();

	        while (reader.Read())
	        {
	            result.Add(
	                new IrregularityDTO
                    {
                        BalanceValueId = reader.GetValue<int>("value_id"),
	                    StartDayNum = reader.GetValue<int>("start_day"),
	                    EndDayNum = reader.GetValue<int>("end_day"),
	                    Value = reader.GetValue<double>("value")
                    });
	        }

            return result;
        }

	    
	}
}
