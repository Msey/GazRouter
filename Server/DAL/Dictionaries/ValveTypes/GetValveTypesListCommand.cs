using System.Collections.Generic;
using GazRouter.DAL.Core;
using GazRouter.DTO.Dictionaries.ValveTypes;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.Dictionaries.ValveTypes
{
    public class GetValveTypesListQuery : QueryReader<List<ValveTypeDTO>>
    {
        public GetValveTypesListQuery(ExecutionContext context)
            : base(context)
        {}

        protected override string GetCommandText()
        {
            return
                @"select vt.valve_type_id, vt.rated_consumption, vt.diametr_id, vt.diametr_name, vt.diameter_conv, vt.diameter_real
                        from V_VALVE_TYPES vt order by vt.sort_order";
        }

        protected override List<ValveTypeDTO> GetResult(OracleDataReader reader)
        {
            var result = new List<ValveTypeDTO>();
            while (reader.Read())
            {
                var periodType =
                   new ValveTypeDTO
                   {
                       Id = reader.GetValue<int>("valve_type_id"),
                       Name = reader.GetValue<string>("diametr_name"),
                       RatedConsumption = reader.GetValue<double>("rated_consumption"),
                       DiameterId = reader.GetValue<int>("diametr_id"),
                       DiameterConv = reader.GetValue<double>("diameter_conv"),
                       DiameterReal = reader.GetValue<double>("diameter_real")
                   };
                result.Add(periodType);
            }
            return result;
        }
    }
}
