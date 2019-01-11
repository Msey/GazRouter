using System;
using GazRouter.DAL.Core;
using GazRouter.DTO.DataStorage;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.DataStorage
{
    public class GetBlobByIdQuery : QueryReader<Guid, BlobDTO>
    {
        public GetBlobByIdQuery(ExecutionContext context)
            : base(context)
        {}

        protected override void BindParameters(OracleCommand command, Guid parameters)
        {
            command.AddInputParameter("p1", parameters);
        }

        protected override string GetCommandText(Guid parameters)
        {
            return
                    @"select t1.data, t1.file_name from V_BLOB_ROWS t1 where t1.blob_id = : p1";
        }

        protected override BlobDTO GetResult(OracleDataReader reader, Guid parameters)
        {
            BlobDTO leak = null;
            if (reader.Read())
            {
                leak =
                    new BlobDTO
                    {
                        FileName = reader.GetValue<string>("file_name"),
                        Data = reader.GetValue<byte[]>("data")
                    };
            }
            return leak;
        }
    }
}
