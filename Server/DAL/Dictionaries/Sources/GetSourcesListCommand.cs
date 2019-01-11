using System.Collections.Generic;
using GazRouter.DAL.Core;
using GazRouter.DTO.Dictionaries.Sources;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.Dictionaries.Sources
{
    public class GetSourcesListQuery : QueryReader<List<SourceDTO>>
	{
        public GetSourcesListQuery(ExecutionContext context)
			: base(context)
		{
		}

		protected override string GetCommandText()
		{
		    return @"select source_id, source_name, system_name, description, 
is_readonly, is_hidden from v_sources where is_hidden = 0";
		}

        protected override List<SourceDTO> GetResult(OracleDataReader reader)
		{
            var result = new List<SourceDTO>();
			while (reader.Read())
			{
                var source =
                    new SourceDTO
						{
                            Id = reader.GetValue<int>("source_id"),
                            Name = reader.GetValue<string>("source_name"),
                            SysName = reader.GetValue<string>("system_name"),
                            Description = reader.GetValue<string>("description"),
                            IsReadonly = reader.GetValue<bool>("is_readonly"),
                            IsHidden = reader.GetValue<bool>("is_hidden")
						};
                result.Add(source);
			}
			return result;
		}
	}
}