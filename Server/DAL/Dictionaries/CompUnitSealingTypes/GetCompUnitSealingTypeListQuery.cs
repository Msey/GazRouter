using System.Collections.Generic;
using GazRouter.DAL.Core;
using GazRouter.DTO.Dictionaries.CompUnitSealingTypes;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.Dictionaries.CompUnitSealingTypes
{
    public class GetCompUnitSealingTypeListQuery : QueryReader<List<CompUnitSealingTypeDTO>>
	{
        public GetCompUnitSealingTypeListQuery(ExecutionContext context)
            : base(context)
		{
		}

		protected override string GetCommandText()
		{
            return @"   SELECT  comp_unit_sealing_type_id, 
                                comp_unit_sealing_type_name                                 
                        FROM    v_comp_unit_sealing_types";
		}

        protected override List<CompUnitSealingTypeDTO> GetResult(OracleDataReader reader)
		{
            var result = new List<CompUnitSealingTypeDTO>();
            while (reader.Read())
            {
                result.Add(
                    new CompUnitSealingTypeDTO
                    {
                        Id = reader.GetValue<int>("comp_unit_sealing_type_id"),
                        Name = reader.GetValue<string>("comp_unit_sealing_type_name")
                    });
            }
            return result;
		}
	}
}