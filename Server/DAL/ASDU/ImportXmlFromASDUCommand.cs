using System;
using GazRouter.DAL.Core;
using GazRouter.DTO.ASDU;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.DispatcherTasks
{
    public class ImportXmlFromASDUCommand : CommandScalar<XmlFileForImport, int>
    {
        public ImportXmlFromASDUCommand(ExecutionContext context)
            : base(context)
        {
            IsStoredProcedure = true;
        }

        protected override void BindParameters(OracleCommand command, XmlFileForImport parameters)
        {
            OutputParameter = command.AddReturnParameter<int>("file_id");
            command.AddInputParameter("p_xml", parameters.Xml);
            if (!parameters.IsMetadataFile)
            {
                command.AddInputParameter("p_filename", parameters.Filename);
            }
        }

        protected override string GetCommandText(XmlFileForImport parameters)
        {
            return parameters.IsMetadataFile ? "integro.p_md_loaddata.load_metadata_helper" : "integro.p_md_loaddata.load_data_changes";
        }
    }
}
