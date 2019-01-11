using System;
using System.Collections.Generic;
using System.Text;
using GazRouter.DAL.Core;
using GazRouter.DTO.Dictionaries.EntityTypes;
using GazRouter.DTO.ManualInput.ChemicalTests;
using Oracle.ManagedDataAccess.Client;
using Utils.Units;

namespace GazRouter.DAL.ManualInput.ChemicalTests
{
    public class GetChemicalTestListQuery : QueryReader<GetChemicalTestListParameterSet, List<ChemicalTestDTO>>
    {
        public GetChemicalTestListQuery(ExecutionContext context)
            : base(context)
        {
        }

        protected override string GetCommandText(GetChemicalTestListParameterSet parameters)
        {
            string q =
                @"  SELECT      ct.chemical_test_id,
                                mp.meas_point_id,
                                ct.test_date,
                                ct.dew_point,
                                ct.dew_point_hydrocarbon,
                                ct.content_nitrogen,
                                ct.concentr_sour_sulfur,
                                ct.concentr_hydrogen_sulfide,
                                ct.content_carbon_dioxid,
                                ct.density,
                                ct.combustion_heat_low,
                                e.entity_id AS parent_id,
                                e.entity_name AS parent_name,
                                sn.entity_name AS parent_short_path, 
                                e.entity_type_id
                                
                    FROM        v_meas_points mp
                    LEFT JOIN   v_entities e
                          ON    e.entity_id = mp.comp_shop_id 
                          OR    e.entity_id = mp.distr_station_id 
                          OR    e.entity_id = mp.meas_line_id
                    LEFT JOIN   v_nm_short_all sn 
                          ON    sn.entity_id = e.entity_id                    
                    LEFT JOIN   v_chemical_tests ct 
                          ON    ct.meas_point_id = mp.meas_point_id 
                          AND   (ct.meas_point_id, ct.test_date) IN 
                                    ( SELECT    meas_point_id, 
                                                MAX(test_date) 
                                      FROM      v_chemical_tests
                                      WHERE test_date <= :timestamp
                                      GROUP BY meas_point_id)
                    WHERE       1=1";

            var sb = new StringBuilder(q);

            if (parameters != null)
            {
                if(parameters.SiteId.HasValue)
                    sb.Append(" AND mp.site_id = :siteid");

                if (parameters.MeasPointId.HasValue)
                    sb.Append(" AND mp.meas_point_id = :mpid");

                if (parameters.ParentId.HasValue)
                    sb.Append(" AND e.entity_id = :parentid");
            }
                
            
            sb.Append(" ORDER BY e.entity_type_id, e.sort_order, sn.entity_name");
            
            return sb.ToString();


        }

        protected override void BindParameters(OracleCommand command, GetChemicalTestListParameterSet parameters)
        {
            DateTime? timestamp = null;
            if (parameters != null)
            {
                if (parameters.SiteId.HasValue)
                    command.AddInputParameter("siteid", parameters.SiteId);

                timestamp = parameters.Timestamp;
    


                if (parameters.MeasPointId.HasValue)
                    command.AddInputParameter("mpid", parameters.MeasPointId);

                if (parameters.ParentId.HasValue)
                    command.AddInputParameter("parentid", parameters.ParentId);
            }
            command.AddInputParameter("timestamp", timestamp ?? DateTime.Now);
        }

        protected override List<ChemicalTestDTO> GetResult(OracleDataReader reader, GetChemicalTestListParameterSet parameters)
        {
            var result = new List<ChemicalTestDTO>();
            while (reader.Read())
            {
                result.Add(new ChemicalTestDTO
                {
                    ChemicalTestId = reader.GetValue<int?>("chemical_test_id"),
                    MeasPointId = reader.GetValue<Guid>("meas_point_id"),
                    TestDate = reader.GetValue<DateTime?>("test_date"),
                    DewPoint = reader.GetValue<double?>("dew_point"),
                    DewPointHydrocarbon = reader.GetValue<double?>("dew_point_hydrocarbon"),
                    ContentNitrogen = reader.GetValue<double?>("content_nitrogen"),
                    ConcentrSourSulfur = reader.GetValue<double?>("concentr_sour_sulfur"),
                    ConcentrHydrogenSulfide = reader.GetValue<double?>("concentr_hydrogen_sulfide"),
                    ContentCarbonDioxid = reader.GetValue<double?>("content_carbon_dioxid"),
                    Density = reader.GetValue<double?>("density"),
                    CombHeatLow = reader.GetValue<double?>("combustion_heat_low"),
                    ParentId = reader.GetValue<Guid>("parent_id"),
                    ParentName = reader.GetValue<string>("parent_name"),
                    ParentShortPath = reader.GetValue<string>("parent_short_path"),
                    ParentEntityType = reader.GetValue<EntityType>("entity_type_id"),
                    IsFrigid = false //todo: прокинуть этот признак во вьюху meas_points
                });
            }
            return result;
        }
    }
}
