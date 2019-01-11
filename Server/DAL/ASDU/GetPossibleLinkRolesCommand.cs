using System.Collections.Generic;
using GazRouter.DAL.Core;
using GazRouter.DTO.ASDU;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.ASDU
{
    public class GetPossibleLinkRolesCommand : QueryReader<LinkParams, List<DictionaryEntry>>
    {
        public GetPossibleLinkRolesCommand(ExecutionContext context)
            : base(context)
        { }

        protected override void BindParameters(OracleCommand command, LinkParams parameters)
        {
            command.AddInputParameter("p_iustype", parameters.IusType);
            command.AddInputParameter("p_iusid", parameters.IusId);
            command.AddInputParameter("p_asduid", parameters.AsduId);
        }

        protected override string GetCommandText(LinkParams parameters)
        {
            return $"select * from table(integro.p_md_tree.get_poss_roles(:p_iustype, :p_iusid, :p_asduid))";
        }

        protected override List<DictionaryEntry> GetResult(OracleDataReader reader, LinkParams parameters)
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