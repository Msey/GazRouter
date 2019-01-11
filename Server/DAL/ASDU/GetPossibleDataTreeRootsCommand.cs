using System.Collections.Generic;
using GazRouter.DAL.Core;
using GazRouter.DTO.ASDU;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.ASDU
{
    public class GetPossibleDataTreeRootsCommand : QueryReader<List<NodeBinding>>
    {
        public GetPossibleDataTreeRootsCommand(ExecutionContext context)
            : base(context)
        { }

        protected override string GetCommandText()
        {
            return
                @"select * from table(integro.p_md_bindingdata.get_possible_roots)";
        }

        protected override List<NodeBinding> GetResult(OracleDataReader reader)
        {
            var result = new List<NodeBinding>();
            while (reader.Read())
            {
                var entry =
                    new NodeBinding
                    {
                        IusId = reader.GetValue<string>("ciuskey"),
                        AsduId = reader.GetValue<string>("casdukey"),
                        MiscInfo = reader.GetValue<string>("cmiscinfo")
                    };
                result.Add(entry);
            }
            return result;
        }
    }
}