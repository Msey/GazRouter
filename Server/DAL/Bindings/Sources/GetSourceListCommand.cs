using System.Collections.Generic;
using GazRouter.DAL.Core;
using GazRouter.DTO.Bindings.Sources;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.Bindings.Sources
{
    public class GetSourceListQuery : QueryReader< List<Source>>
    {
        public GetSourceListQuery(ExecutionContext context)
            : base(context)
            {
            }

           
             
            protected override string GetCommandText()
            {
                return @"SELECT SOURCE_ID,
  SOURCE_NAME,
  SYSTEM_NAME,
  DESCRIPTION,
  IS_HIDDEN,
  IS_READONLY
FROM V_SOURCES
where IS_HIDDEN = 0";
            }

            protected override List<Source> GetResult(OracleDataReader reader)
            {
                var result = new List<Source>();
                while (reader.Read())
                {
                    result.Add(new Source
                    {
                        SourceId = reader.GetValue<int>("SOURCE_ID"),
                        SourceName = reader.GetValue<string>("SOURCE_NAME"),
                        SystemName = reader.GetValue<string>("SYSTEM_NAME"),
                        IsHidden = reader.GetValue<bool>("IS_HIDDEN"),
                        IsReadonly = reader.GetValue<bool>("IS_READONLY") ,
                        Description = reader.GetValue<string>("DESCRIPTION") 
                    });
                }
                return result;
            }
        
    }
}
