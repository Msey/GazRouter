using System.Collections.Generic;
using GazRouter.DAL.Core;
using GazRouter.DTO.Dictionaries.CompUnitStopTypes;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.Dictionaries.CompUnitStopTypes
{
    public class GetCompUnitStopTypeListQuery : QueryReader<List<CompUnitStopTypeDTO>>
    {
        public GetCompUnitStopTypeListQuery(ExecutionContext context)
            : base(context)
        {
        }

        protected override List<CompUnitStopTypeDTO> GetResult(OracleDataReader reader)
        {
            var result = new List<CompUnitStopTypeDTO>();
            while (reader.Read())
            {
                result.Add(new CompUnitStopTypeDTO
                {
                    Id = reader.GetValue<int>("comp_unit_stop_type_id"),
                    Name = reader.GetValue<string>("comp_unit_stop_type_name")
                });
                
            }
            return result;
        }

        protected override string GetCommandText()
        {
            return @"   SELECT  comp_unit_stop_type_id,
                                comp_unit_stop_type_name
                        FROM    v_comp_unit_stop_types";
        }
    }
}