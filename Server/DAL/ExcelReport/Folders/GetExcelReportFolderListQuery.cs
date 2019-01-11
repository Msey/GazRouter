using System.Collections.Generic;
using GazRouter.DAL.Core;
using GazRouter.DTO.Dashboards.Folders;
using Oracle.ManagedDataAccess.Client;
namespace GazRouter.DAL.ExcelReport.Folders
{
    public class GetExcelReportFolderListQuery : QueryReader<int, List<FolderDTO>>
    {
        public GetExcelReportFolderListQuery(ExecutionContext context)
            : base(context)
        { }

        protected override string GetCommandText(int parameter)
        {
            return @"SELECT     vf.folder_id,
						        vf.folder_name,
						        vf.sort_order ,
						        vf.user_id,
						        usr.name user_name,
						        vf.parent_folder_id
					 FROM       v_folder_reports vf 
                     JOIN       v_users usr on usr.user_id = vf.user_Id
                     WHERE      vf.user_id = :userId";
        }

        protected override void BindParameters(OracleCommand command, int parameters)
        {
            command.AddInputParameter(":UserId", parameters);
        }

        protected override List<FolderDTO> GetResult(OracleDataReader reader, int parameters)
        {
            var folder = new List<FolderDTO>();
            while (reader.Read())
            {
                folder.Add(new FolderDTO
                {
                    Id        = reader.GetValue<int>("folder_id"),
                    Name      = reader.GetValue<string>("folder_name"),
                    ParentId  = reader.GetValue<int?>("parent_folder_id"),
                    CreatorUserId    = reader.GetValue<string>("cre_user_id"),
                    SortOrder = reader.GetValue<int>("sort_order"),
                });
            }
            return folder;
        }
    }
}
//UserName  = reader.GetValue<string>("user_name"),