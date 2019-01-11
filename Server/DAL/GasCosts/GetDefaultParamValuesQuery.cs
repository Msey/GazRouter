using System;
using System.Collections.Generic;
using GazRouter.DAL.Core;
using GazRouter.DTO.Dictionaries.Targets;
using GazRouter.DTO.GasCosts;
using GazRouter.DTO.Infrastructure;
using Oracle.ManagedDataAccess.Client;
using Utils.Extensions;

namespace GazRouter.DAL.GasCosts
{
    public class GetDefaultParamValuesQuery : QueryReader<GetGasCostListParameterSet, List<DefaultParamValuesDTO>>
    {
        public GetDefaultParamValuesQuery(ExecutionContext context)
            : base(context)
        {
        }

        protected override string GetCommandText(GetGasCostListParameterSet parameters)
        {
            return @"   SELECT  site_id,
                                target_id, 
                                period, 
                                p_air, 
                                density, 
                                comb_heat, 
                                n_content, 
                                cd_content
                        FROM    V_AUX_DEFAULT_DATA
                        WHERE   period = :prd
                        AND     site_id = :siteid";

            
        }

        protected override void BindParameters(OracleCommand command, GetGasCostListParameterSet parameters)
        {
            command.AddInputParameter("prd", parameters.StartDate);
            command.AddInputParameter("siteid", parameters.SiteId);
        }

        protected override List<DefaultParamValuesDTO> GetResult(OracleDataReader reader, GetGasCostListParameterSet parameters)
        {
            var result = new List<DefaultParamValuesDTO>();
            while (reader.Read())
            {
                result.Add(new DefaultParamValuesDTO
                {
                    SiteId = reader.GetValue<Guid>("site_id"),
                    Target = (Target)reader.GetValue<int>("target_id"),
                    Period = reader.GetValue<DateTime>("period"),
                    PressureAir = reader.GetValue<double>("p_air"),
                    Density = reader.GetValue<double>("density"),
                    CombHeat = reader.GetValue<double>("comb_heat"),
                    NitrogenContent = reader.GetValue<double>("n_content"),
                    CarbonDioxideContent = reader.GetValue<double>("cd_content")
                });
            }
            return result;
        }
    }
}