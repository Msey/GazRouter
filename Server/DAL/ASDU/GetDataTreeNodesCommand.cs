using System.Collections.Generic;
using GazRouter.DAL.Core;
using GazRouter.DTO.ASDU;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.ASDU
{
    public class GetDataTreeNodesCommand : QueryReader<DataTreeParams, List<DataTreeNode>>
    {
        public GetDataTreeNodesCommand(ExecutionContext context)
            : base(context)
        { }

        protected override void BindParameters(OracleCommand command, DataTreeParams parameters)
        {
            command.AddInputParameter("p1", parameters.RootId);
            command.AddInputParameter("p2", parameters.SingleNodeType);
            command.AddInputParameter("p3", parameters.SingleNodeId);
        }

        protected override string GetCommandText(DataTreeParams parameters)
        {
            return parameters.IsIus
                ? "select * from table(integro.P_MD_BINDINGDATA.get_ius_data_tree(:p1, :p2, :p3))"
                : "select * from table(integro.P_MD_BINDINGDATA.get_asdu_data_tree(:p1, :p2, :p3))";
        }

        protected override List<DataTreeNode> GetResult(OracleDataReader reader, DataTreeParams parameters)
        {
            var result = new List<DataTreeNode>();
            while (reader.Read())
            {
                var node =
                    new DataTreeNode
                    {
                        NodeType = (DataTreeNodeType)reader.GetValue<int>("nnodetype"),
                        Id = reader.GetValue<string>("cid"),
                        ParentId = reader.GetValue<string>("cparentid"),
                        RawId = reader.GetValue<string>("crawid"),
                        RawType = reader.GetValue<string>("crawtype"),
                        Name = reader.GetValue<string>("cname"),
                        LinkedId = reader.GetValue<string>("clinkedobjid"),
                        ErrorCount = reader.GetValue<int>("nerrorcount"),
                        MiscInfo = reader.GetValue<string>("cmiscinfo"),
                        Value   = reader.GetValue<string>("cvalue"),
                        Equality = (DataValueEquality?)reader.GetValue<int?>("nequality")
                    };
                result.Add(node);
            }
            return result;
        }
    }
}