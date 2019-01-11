using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GazRouter.DAL.Core;
using GazRouter.DTO.DataStorage;
using Oracle.ManagedDataAccess.Client;
using GazRouter.DTO.ASDU;

namespace GazRouter.DAL.ASDU
{
    public class GetImportLogEntriesCommand : QueryReader<GetImportLogParam, List<AsduDataChange>>
    {
        public GetImportLogEntriesCommand(ExecutionContext context)
            : base(context)
        { }

        protected override void BindParameters(OracleCommand command, GetImportLogParam parameters)
        {
            command.AddInputParameter("p1", parameters.ImportId);
            command.AddInputParameter("p2", parameters.ChangeType);
        }

        protected override string GetCommandText(GetImportLogParam parameters)
        {
            return
                    @"select * from table(integro.p_md_loaddata.get_packet_stats(:p1, :p2))";
        }

        protected override List<AsduDataChange> GetResult(OracleDataReader reader, GetImportLogParam parameters)
        {
            var result = new List<AsduDataChange>();
            while (reader.Read())
            {
                var entry =
                   new AsduDataChange
                   {
                       Nrownum = reader.GetValue<int>("nrownum"),
                       Cchangetype = reader.GetValue<string>("cchangetype"),
                       Cclass = reader.GetValue<string>("cclass"),
                       Cobjname = reader.GetValue<string>("cobjname"),
                       Cobjid = reader.GetValue<string>("cobjid"),
                       Cparamname = reader.GetValue<string>("cparamname"),
                       Cdesc = reader.GetValue<string>("cdesc"),
                   };
                result.Add(entry);
            }
            return result;
        }
    }
}
