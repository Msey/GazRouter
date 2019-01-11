using System;
using GazRouter.DAL.Core;
using GazRouter.DTO.Dictionaries.AggregatorTypes;
using GazRouter.DTO.Dictionaries.AlarmTypes;
using GazRouter.DTO.ObjectModel.Aggregators;
using GazRouter.DTO.ObjectModel.BoilerPlants;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.ObjectModel.Aggregators
{
    public class GetAggregatorByIdQuery : QueryReader<Guid, AggregatorDTO>
	{
        public GetAggregatorByIdQuery(ExecutionContext context)
            : base(context)
		{
		}
                
        protected override string GetCommandText(Guid parameters)
        {
            return @"   SELECT      aggr_id, 
                                    aggr_name, 
                                    description, 
                                    aggr_type_id, 
                                    aggr_type_name
                        FROM        v_aggregators
                        WHERE       aggr_id = :id";
        }

        protected override void BindParameters(OracleCommand command, Guid parameters)
        {
            command.AddInputParameter("id", parameters);
        }

        protected override AggregatorDTO GetResult(OracleDataReader reader, Guid parameters)
		{
            AggregatorDTO result = null;
			while (reader.Read())
			{
				result = new AggregatorDTO
                {
                    Id = reader.GetValue<Guid>("aggr_id"),
                    Name = reader.GetValue<string>("aggr_name"),
                    Description = reader.GetValue<string>("description"),
                    AggregatorType = reader.GetValue<AggregatorType>("aggr_type_id"),
                    AggregatorTypeName = reader.GetValue<string>("aggr_type_name")
                };
			}
			return result;
		}
	}
}