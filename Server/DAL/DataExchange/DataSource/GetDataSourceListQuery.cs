using System.Collections.Generic;
using System.Text;
using GazRouter.DAL.Core;
using GazRouter.DTO.DataExchange.DataSource;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.DataExchange.DataSource
{
    public class GetDataSourceListQuery : QueryReader<GetDataSourceListParameterSet, List<DataSourceDTO>>
    {
        public GetDataSourceListQuery(ExecutionContext context)
            : base(context) {}


        protected override string GetCommandText(GetDataSourceListParameterSet parameters)
        {
            var q =  @" SELECT  source_id,
                                source_name,
                                system_name,
                                description,
                                is_hidden,
                                is_readonly
                        FROM    v_sources
                        WHERE   1=1";

            var sb = new StringBuilder(q);

            
            if (parameters == null || !parameters.GetHidden) sb.Append(" AND is_hidden = 0");
            if (parameters == null || !parameters.GetReadonly) sb.Append(" AND is_readonly = 0");
            

            return sb.ToString();

        }
        
        protected override List<DataSourceDTO> GetResult(OracleDataReader reader, GetDataSourceListParameterSet parameters)
        {
            var result = new List<DataSourceDTO>();
            while (reader.Read())
            {
                result.Add(new DataSourceDTO
                {
                    Id = reader.GetValue<int>("source_id"),
                    Name = reader.GetValue<string>("source_name"),
                    SysName = reader.GetValue<string>("system_name"),
                    IsHidden = reader.GetValue<bool>("is_hidden"),
                    IsReadonly = reader.GetValue<bool>("is_readonly"),
                    Description = reader.GetValue<string>("description")
                });
            }
            return result;
        }
        
    }
}
