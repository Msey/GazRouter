using System.Collections.Generic;
using GazRouter.DAL.Core;
using GazRouter.DTO.Dashboards.Folders;
using Oracle.ManagedDataAccess.Client;
namespace GazRouter.DAL.ExcelReport.Folders
{
    public class GetAllExcelReportFolderListQuery : QueryReader<List<FolderDTO>>
    {
        public GetAllExcelReportFolderListQuery(ExecutionContext context) : base(context) { }
        protected override string GetCommandText()
        {
            return @"SELECT     vf.folder_id,
						        vf.folder_name,
						        vf.sort_order,
						        vf.cre_user_login,
						        vf.parent_folder_id
					 FROM       v_folder_reports vf";
        }
        protected override void BindParameters(OracleCommand command)
        {
        }
        protected override List<FolderDTO> GetResult(OracleDataReader reader)
        {
            var result = new List<FolderDTO>();
            while (reader.Read())
            {
                result.Add(new FolderDTO
                {
                    Id            = reader.GetValue<int>("folder_id"),
                    Name          = reader.GetValue<string>("folder_name"),
                    ParentId      = reader.GetValue<int?>("parent_folder_id"),
                    CreatorUserId = reader.GetValue<string>("cre_user_login"),
                    SortOrder     = reader.GetValue<int>("sort_order"),
                });
            }
            return result;
        }
    }
}
#region trash
//UserName = reader.GetValue<string>("user_name"),
//JOIN v_users usr on usr.user_id = vf.user_Id
#endregion
