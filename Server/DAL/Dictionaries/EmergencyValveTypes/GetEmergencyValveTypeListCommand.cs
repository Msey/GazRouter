using System.Collections.Generic;
using GazRouter.DAL.Core;
using GazRouter.DTO.Dictionaries.EmergencyValveTypes;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.Dictionaries.EmergencyValveTypes
{
    public class GetEmergencyValveTypeListQuery : QueryReader<List<EmergencyValveTypeDTO>>
    {
        public GetEmergencyValveTypeListQuery(ExecutionContext context)
            : base(context)
        {}

        protected override string GetCommandText()
        {
            return @"select * from v_emergency_valve_types";
        }

        protected override List<EmergencyValveTypeDTO> GetResult(OracleDataReader reader)
        {
            var complexList = new List<EmergencyValveTypeDTO>();
            while (reader.Read())
            {
                complexList.Add(
                    new EmergencyValveTypeDTO
                    {
                        Id = reader.GetValue<int>("emergency_valve_types_id"),
                        Name = reader.GetValue<string>("emergency_valve_name"),
                        Description = reader.GetValue<string>("DESCRIPTION"),
                        SortOrder = reader.GetValue<int>("SORT_ORDER"),
                        InnerDiameter = reader.GetValue<double>("inner_diameter"),
                    });
            }
            return complexList;
        }
    }
}