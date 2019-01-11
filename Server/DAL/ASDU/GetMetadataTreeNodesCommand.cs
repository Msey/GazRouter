using System.Collections.Generic;
using GazRouter.DAL.Core;
using GazRouter.DTO.ASDU;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.ASDU
{
    public class GetMetadataTreeNodesCommand : QueryReader<MetadataTreeParams, List<MetadataTreeNode>>
    {
        public GetMetadataTreeNodesCommand(ExecutionContext context)
            : base(context)
        { }

        protected override void BindParameters(OracleCommand command, MetadataTreeParams parameters)
        {

        }

        protected override string GetCommandText(MetadataTreeParams parameters)
        {
            return parameters.IsIus
                ? "select * from table(integro.P_MD_BINDING.get_ius_md_tree)"
                : "select * from table(integro.P_MD_BINDING.get_asdu_md_tree)";
        }

        protected override List<MetadataTreeNode> GetResult(OracleDataReader reader, MetadataTreeParams parameters)
        {
            var result = new List<MetadataTreeNode>();
            while (reader.Read())
            {
                var node =
                    new MetadataTreeNode
                    {
                        Nodetype = (MetadataTreeNodeType)reader.GetValue<int>("nnodetype"),
                        Id = reader.GetValue<string>("cid"),
                        ParentId = reader.GetValue<string>("cparentid"),
                        RawId = reader.GetValue<string>("crawid"),
                        RawType = reader.GetValue<string>("crawtype"),
                        Name = reader.GetValue<string>("cname"),
                        LinkedId = reader.GetValue<string>("clinkedobjid"),
                        ErrorCount = reader.GetValue<int>("nerrorcount"),
                        MiscInfo = reader.GetValue<string>("cmiscinfo"),
                    };
                result.Add(node);
            }
            return result;
        }
    }
}