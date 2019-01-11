using System;
using System.Collections.Generic;
using GazRouter.DAL.Core;
using GazRouter.DTO.Dashboards.DashboardGrants;
using Oracle.ManagedDataAccess.Client;
namespace GazRouter.DAL.Dashboards.DashboardGrants
{
    public class GetFolderPermissionListQuery : QueryReader<List<DashboardGrantDTO2>>
    {
        public GetFolderPermissionListQuery(ExecutionContext context) : base(context){ }

        protected override string GetCommandText()
        {
            return @"SELECT   t.folder_id,
                              t.site_id, 
                              t.permission   
                     FROM     v_folders_permissions t";
        }
        protected override void BindParameters(OracleCommand command)
        {
        }
        protected override List<DashboardGrantDTO2> GetResult(OracleDataReader reader)
        {
            var result = new List<DashboardGrantDTO2>();
            while (reader.Read())
            {
                result.Add(new DashboardGrantDTO2
                {
                    ItemId = reader.GetValue<int>("folder_id"),
                    SiteId = reader.GetValue<Guid>("site_id"),
                    Permission = reader.GetValue<int>("permission")
                });
            }
            return result;
        }
    }
}
