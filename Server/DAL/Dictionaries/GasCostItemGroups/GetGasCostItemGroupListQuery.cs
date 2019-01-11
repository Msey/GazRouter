using System.Collections.Generic;
using GazRouter.DAL.Core;
using GazRouter.DTO.Dictionaries.GasCostItemGroups;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.Dictionaries.GasCostItemGroups
{
	public class GetGasCostItemGroupListQuery : QueryReader<List<GasCostItemGroupDTO>>
    {
		public GetGasCostItemGroupListQuery(ExecutionContext context)
			: base(context)
        {
        }

        protected override string GetCommandText()
        {
            return @"   SELECT      aux_item_group_id, 
                                    aux_item_group_name 
                        FROM        v_aux_item_groups";
        }

		protected override List<GasCostItemGroupDTO> GetResult(OracleDataReader reader)
        {
			var result = new List<GasCostItemGroupDTO>();
            while (reader.Read())
            {
				result.Add(
                    new GasCostItemGroupDTO
                    {
				        Id = reader.GetValue<int>("aux_item_group_id"), 
                        Name = reader.GetValue<string>("aux_item_group_name") 
				    });
            }
            return result;
        }
    }
}