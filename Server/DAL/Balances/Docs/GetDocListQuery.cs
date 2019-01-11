using System;
using System.Collections.Generic;
using System.Text;
using GazRouter.DAL.Core;
using GazRouter.DTO.Balances.Docs;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.Balances.Docs
{
    public class GetDocListQuery : QueryReader<int?, List<DocDTO>>
    {
        public GetDocListQuery(ExecutionContext context)
            : base(context)
        {
        }

        protected override string GetCommandText(int? parameters)
        {
            var sql = @"SELECT      d.doc_id, 
                                    d.contract_id, 
                                    d.description, 
                                    d.blob_id, 
                                    d.file_name,
                                    d.data_length, 
                                    d.date_create
                        FROM        v_bl_docs d
                        WHERE       1=1";

            var sb = new StringBuilder(sql);

            if (parameters.HasValue)
                sb.Append(" AND d.contract_id = :contractId");

            sb.Append(" ORDER BY d.date_create ASC");

            return sb.ToString();
        }

        protected override void BindParameters(OracleCommand command, int? parameters)
        {
            if (parameters.HasValue)
                command.AddInputParameter("contractId", parameters);
        }

        protected override List<DocDTO> GetResult(OracleDataReader reader, int? parameters)
        {
            var result = new List<DocDTO>();
            while (reader.Read())
            {
                result.Add(
                    new DocDTO
                    {
                        Id = reader.GetValue<int>("doc_id"),
                        ExternalId = reader.GetValue<int>("contract_id"),
                        Description = reader.GetValue<string>("description"),
                        BlobId = reader.GetValue<Guid>("blob_id"),
                        FileName = reader.GetValue<string>("file_name"),
                        DataLength = reader.GetValue<int>("data_length"),
                        CreateDate = reader.GetValue<DateTime>("date_create")
                    });
            }
            return result;
        }
    }
}