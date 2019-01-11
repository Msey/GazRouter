using System.Collections.Generic;
using GazRouter.DAL.Core;
using GazRouter.DTO.ASDU;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.ASDU
{
    public class GetAsduOutboundContentsCommand : QueryReader<int, List<OutboundContents>>
    {
        public GetAsduOutboundContentsCommand(ExecutionContext context)
            : base(context)
        { }

        protected override void BindParameters(OracleCommand command, int parameters)
        {
            command.AddInputParameter("p1", parameters);
        }

        protected override string GetCommandText(int parameters)
        {
            return
                @"select * from table(integro.p_md_tree.load_asdu_outboundcontents(:p1))";
        }

        protected override List<OutboundContents> GetResult(OracleDataReader reader, int parameters)
        {
            var result = new List<OutboundContents>();
            while (reader.Read())
            {
                var entry =
                    new OutboundContents
                    {
                        Key = reader.GetValue<int>("nkey"),
                        Id = reader.GetValue<string>("cid"),
                        Nodetype = reader.GetValue<string>("cnodetype"),
                        Type = reader.GetValue<string>("ctype"),
                        Name = reader.GetValue<string>("cname"),
                        Changestate = reader.GetValue<string>("cchangestate"),
                        Value = reader.GetValue<string>("cvalue"),
                        ValueAsdu = reader.GetValue<string>("cvalueasdu"),
                    };
                result.Add(entry);
            }
            return result;
        }
    }
}