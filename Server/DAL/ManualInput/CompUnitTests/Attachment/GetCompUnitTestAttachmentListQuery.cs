using System;
using System.Collections.Generic;
using GazRouter.DAL.Core;
using GazRouter.DTO.Attachments;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.ManualInput.CompUnitTests.Attachment
{
    public class GetCompUnitTestAttachmentListQuery : QueryReader<List<int>, List<AttachmentDTO<int, int>>>
    {
        public GetCompUnitTestAttachmentListQuery(ExecutionContext context)
            : base(context)
        {
            
        }


        protected override string GetCommandText(List<int> parameters)
        {
            var q = string.Format( 
                @"  SELECT  t1.comp_unit_test_att_id,
                            t1.comp_unit_test_id,
                            t1.file_name,
                            t1.file_blob_id,
                            t1.description,
                            t1.data_length
                    FROM    v_comp_units_tests_att t1
                    WHERE   t1.comp_unit_test_id IN {0}", CreateInClause(parameters.Count));

            return q;
        }

        protected override List<AttachmentDTO<int, int>> GetResult(OracleDataReader reader, List<int> parameters)
        {
            var attachments = new List<AttachmentDTO<int, int>>();
            while (reader.Read())
            {
                var att =
                    new AttachmentDTO<int, int>
                    {
                        Id = reader.GetValue<int>("comp_unit_test_att_id"),
                        ExternalId = reader.GetValue<int>("comp_unit_test_id"),
                        FileName = reader.GetValue<string>("file_name"),
                        BlobId =  reader.GetValue<Guid>("file_blob_id"),
                        Description = reader.GetValue<string>("description"),
                        DataLength = reader.GetValue<int>("data_length")
                    };
                attachments.Add(att);
            }
            return attachments;
        }

        

        protected override void BindParameters(OracleCommand command, List<int> parameters)
        {
            if (parameters == null) return;

            for (var i = 0; i < parameters.Count; i++)
                command.AddInputParameter(string.Format("p{0}", i), parameters[i]);
        }

    }
}
