using System.Collections.Generic;
using GazRouter.DAL.Core;
using GazRouter.DTO.ExcelReports;
using Oracle.ManagedDataAccess.Client;
namespace GazRouter.DAL.ExcelReport
{
    public class GetDashboardGrantListQuery : QueryReader<int, List<DashboardGrantDTO>>
    {
        public GetDashboardGrantListQuery(ExecutionContext context)
            : base(context)
        { }
        protected override string GetCommandText(int parameter)
        {
            return @"Select 
						DASHBOARD_ID,
						usr.USER_ID,
						IS_EDITABLE,
						IS_GRANTABLE, 
						usr.Name
					From V_Users usr 
                    left join V_dashBoards_grants vf 
                    on usr.user_id = vf.User_Id 
                    and (vf.DASHBOARD_ID = :DASHBOARDID OR vf.DASHBOARD_ID IS NULL)";
        }
        protected override void BindParameters(OracleCommand command, int parameters)
        {
            command.AddInputParameter(":DASHBOARDID", parameters);
        }
        protected override List<DashboardGrantDTO> GetResult(OracleDataReader reader, int parameters)
        {
            var DashboardGrant = new List<DashboardGrantDTO>();
            while (reader.Read())
            {
                DashboardGrant.Add(new DashboardGrantDTO
                {
                    DashboardId = reader.GetValue<int>("DASHBOARD_ID"),
                    UserName = reader.GetValue<string>("Name"),
                    UserId = reader.GetValue<int>("USER_ID"),
                    IsEdit = reader.GetValue<bool>("IS_EDITABLE"),
                    IsGrantable = reader.GetValue<bool>("IS_GRANTABLE"),
                });
            }
            return DashboardGrant;
        }
    }
}