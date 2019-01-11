using System.Collections.Generic;
using GazRouter.DAL.Core;
using GazRouter.DTO.Dictionaries.Regions;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.Dictionaries.Regions
{
    public class GetRegionListQuery : QueryReader<List<RegionDTO>>
    {
        public GetRegionListQuery(ExecutionContext context) : base(context)
        {
        }

        protected override string GetCommandText()
        {
            return @"   SELECT      region_id, 
                                    region_name
                        FROM        v_regions
                        ORDER BY    region_name";
        }

        protected override List<RegionDTO> GetResult(OracleDataReader reader)
        {
            var result = new List<RegionDTO>();
            while (reader.Read())
            {
                result.Add(
                    new RegionDTO
                    {
                        Id = reader.GetValue<int>("region_id"),
                        Name = reader.GetValue<string>("region_name"),
                        IsFrigid = false //todo: добавить признак климатической зоны в БД
                    });
            }
            return result;
        }
    }
}