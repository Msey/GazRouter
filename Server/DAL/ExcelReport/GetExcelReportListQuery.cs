using System;
using System.Collections.Generic;
using GazRouter.DAL.Core;
using GazRouter.DTO.Dashboards.Dashboard;
using GazRouter.DTO.ExcelReports;
using Oracle.ManagedDataAccess.Client;
namespace GazRouter.DAL.ExcelReport
{
	public class GetExcelReportListQuery : QueryReader<int, List<DashboardDTO>>
	{
		public GetExcelReportListQuery(ExecutionContext context)
			: base(context)
		{ }
        // todo: (select * from V_Folder_Reports union select * from v_folders) f
        // todo: V_Folder_Reports
        protected override string GetCommandText(int parameters)
		{
			return @"select db.*,dg.is_editable,dg.is_grantable,fold.FOLDER_ID,usr.name user_Name ,fold.Sort_order 
				from
				V_DASHBOARD_REPORTS db 
				join V_DASHBOARDS_GRANTS dg on DB.DASHBOARD_ID = DG.DASHBOARD_ID 
				join V_Users usr on usr.User_id = dg.User_id
				left join
				(
				  select df.folder_id, df.dashboard_id,df.Sort_order
				  from (select * from V_Folder_Reports union select * from v_folders) f
				  join V_DASHBOARDS_FOLDERS df on Df.folder_id = f.folder_id
				  where f.user_id = :UserId
				) fold on db.dashboard_id = fold.dashboard_id
				where dg.user_id = :UserId ";
		}
        
		protected override void BindParameters(OracleCommand command, int parameters)
		{
			command.AddInputParameter(":UserId", parameters);
		}
        protected override List<DashboardDTO> GetResult(OracleDataReader reader, int parameters)
		{
			var dashBoardGrant = new List<DashboardDTO>();
			while (reader.Read())
			{
				dashBoardGrant.Add(new DashboardDTO
                {
					Id = reader.GetValue<int>("DASHBOARD_ID"),
					DashboardName = reader.GetValue<string>("DASHBOARD_Name"),
					CreateDate = reader.GetValue<DateTime>("CREATEDATE"),
					CreatorUserId = reader.GetValue<string>("User_ID"),
					CreatorUserName = reader.GetValue<string>("USER_Name"),
					//IsEditable = reader.GetValue<bool>("IS_EDITABLE"),
					IsGrantable = reader.GetValue<bool>("IS_GRANTABLE"),
					FolderId = reader.GetValue<int?>("FOLDER_ID"),
					SortOrder = reader.GetValue<int>("Sort_order"),
                    //PeriodTypeId = reader.GetValue<PeriodType>("PERIOD_TYPE_ID"),
				});
			}
			return dashBoardGrant;
		}
	}
}

