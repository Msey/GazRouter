using System;
using System.Collections.Generic;
using GazRouter.DAL.Core;
using GazRouter.DTO.Attachments;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.ManualInput.CompUnitTests.Attachment
{
    public class GetCompUnitTestAttachmentsByTestIdQuery : QueryReader<int, List<AttachmentDTO<int, int>>>
    {
        public GetCompUnitTestAttachmentsByTestIdQuery(ExecutionContext context)
            : base(context)
        { }


        protected override List<AttachmentDTO<int, int>> GetResult(OracleDataReader reader, int parameters)
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

        protected override string GetCommandText(int parameters)
        {
            return @"select t1.comp_unit_test_att_id
                        ,t1.comp_unit_test_id
                        ,t1.file_name
                        ,t1.file_data
                        ,t1.file_blob_id
                        ,t1.description
                        ,t1.data_length
                from v_comp_units_tests_att t1
                where t1.comp_unit_test_id = :p1";
        }

        protected override void BindParameters(OracleCommand command, int parameters)
        {
            command.AddInputParameter("p1", parameters);
        }

    }
}
