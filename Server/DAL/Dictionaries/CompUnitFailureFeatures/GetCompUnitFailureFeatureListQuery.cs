using System.Collections.Generic;
using GazRouter.DAL.Core;
using GazRouter.DTO.Dictionaries.CompUnitFailureFeatures;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.Dictionaries.CompUnitFailureFeatures
{
    public class GetCompUnitFailureFeatureListQuery : QueryReader<List<CompUnitFailureFeatureDTO>>
    {
        public GetCompUnitFailureFeatureListQuery(ExecutionContext context)
            : base(context)
        {
        }


        protected override List<CompUnitFailureFeatureDTO> GetResult(OracleDataReader reader)
        {
            var features = new List<CompUnitFailureFeatureDTO>();
            while (reader.Read())
            {
                features.Add(new CompUnitFailureFeatureDTO
                {
                    Id = reader.GetValue<int>("failure_feature_id"),
                    Name = reader.GetValue<string>("failure_feature_name"),
                    Description = reader.GetValue<string>("description")
                });
                
            }
            return features;
        }

        protected override string GetCommandText()
        {
            return @"   SELECT  failure_feature_id,
                                failure_feature_name,
                                description
                        FROM    v_failure_features";
        }
    }
}