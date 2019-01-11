using System.Collections.Generic;
using GazRouter.DAL.Core;
using GazRouter.DTO.Dictionaries.CompUnitRepairTypes;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.Dictionaries.CompUnitRepairTypes
{
    public class GetCompUnitRepairTypeListQuery : QueryReader<List<CompUnitRepairTypeDTO>>
    {
        public GetCompUnitRepairTypeListQuery(ExecutionContext context)
            : base(context)
        {
        }

        protected override List<CompUnitRepairTypeDTO> GetResult(OracleDataReader reader)
        {
            var result = new List<CompUnitRepairTypeDTO>();
            while (reader.Read())
            {
                result.Add(
                    new CompUnitRepairTypeDTO
                    {
                        Id = reader.GetValue<int>("comp_unit_repair_type_id"),
                        Name = reader.GetValue<string>("comp_unit_repair_type_name")
                    });
                
            }
            return result;
        }

        protected override string GetCommandText()
        {
            return @"   SELECT  comp_unit_repair_type_id,
                                comp_unit_repair_type_name
                        FROM    v_comp_unit_repair_types";
        }
    }
}