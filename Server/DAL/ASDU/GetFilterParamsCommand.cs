using System;
using System.Collections.Generic;
using GazRouter.DAL.Core;
using GazRouter.DTO.ASDU;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.ASDU
{
    public class GetFilterParamsCommand : QueryReader<FilterType, List<DictionaryEntry>>
    {
        public GetFilterParamsCommand(ExecutionContext context)
            : base(context)
        { }

        protected override string GetCommandText(FilterType parameters)
        {
            string procName;
            switch (parameters)
            {
                case FilterType.IusObjectTypes:
                    procName = "load_ius_object_types";
                    break;
                case FilterType.AsduObjectTypes:
                    procName = "load_asdu_object_types";
                    break;
                case FilterType.LinkStates:
                    procName = "load_link_statusee";
                    break;
                case FilterType.ChangeStates:
                    procName = "load_change_statusee";
                    break;
                case FilterType.AsduOutbounds:
                    procName = "load_asdu_outbounds";
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(parameters), parameters, null);
            }
            return $"select * from table(integro.p_md_tree.{procName}())";
        }

        protected override List<DictionaryEntry> GetResult(OracleDataReader reader, FilterType parameters)
        {
            var result = new List<DictionaryEntry>();
            while (reader.Read())
            {
                var entry =
                    new DictionaryEntry
                    {
                        Key = reader.GetValue<string>("ckey"),
                        Value = reader.GetValue<string>("cvalue"),
                    };
                result.Add(entry);
            }
            return result;
        }
    }
}