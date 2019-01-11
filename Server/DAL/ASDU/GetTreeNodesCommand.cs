using System.Collections.Generic;
using GazRouter.DAL.Core;
using GazRouter.DTO.ASDU;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.ASDU
{
    public class GetTreeNodesCommand : QueryReader<MatchingTreeNodeParams, List<MatchingTreeNode>>
    {
        public GetTreeNodesCommand(ExecutionContext context)
            : base(context)
        { }

        protected override void BindParameters(OracleCommand command, MatchingTreeNodeParams parameters)
        {
            command.AddInputParameter("p_parentnodetype", parameters.ParentNode.Nodetype);
            command.AddInputParameter("p_parenttype", parameters.ParentNode.Type);
            command.AddInputParameter("p_parentid", parameters.ParentNode.Id);
            if (parameters.NodeToRefresh != null)
            {
                command.AddInputParameter("p_nodetype", parameters.NodeToRefresh.Nodetype);
                command.AddInputParameter("p_type", parameters.NodeToRefresh.Type);
                command.AddInputParameter("p_id", parameters.NodeToRefresh.Id);
            }
        }

        protected override string GetCommandText(MatchingTreeNodeParams parameters)
        {
            return parameters.NodeToRefresh == null
                ? "select * from table(integro.p_md_tree.getTreeNodes(:p_parentnodetype, :p_parenttype, :p_parentid))"
                : "select * from table(integro.p_md_tree.getTreeNode(:p_parentnodetype, :p_parenttype, :p_parentid, :p_nodetype, :p_type, :p_id))";
        }

        protected override List<MatchingTreeNode> GetResult(OracleDataReader reader, MatchingTreeNodeParams parameters)
        {
            var result = new List<MatchingTreeNode>();
            while (reader.Read())
            {
                var node =
                    new MatchingTreeNode
                    {
                        Nodetype = reader.GetValue<string>("cnodetype"),
                        Type = reader.GetValue<string>("ctype"),
                        Id = reader.GetValue<string>("cid"),
                        Name = reader.GetValue<string>("cname"),
                        Linkstate = reader.GetValue<string>("clinkstate"),
                        Changestate = reader.GetValue<string>("cchangestate"),
                        Linkedobjid = reader.GetValue<string>("clinkedobjid"),
                        Linkedrole = reader.GetValue<string>("clinkedrole"),
                        Somefile = reader.GetValue<string>("csomefile"),
                        CanLink = reader.GetValue<int>("ncanlink") == 1,
                        IsEndNode = reader.GetValue<int>("nendelement") == 1,
                        Value = reader.GetValue<string>("cvalue"),
                    };
                result.Add(node);
            }
            return result;
        }
    }
}