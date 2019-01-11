using System;
using System.Collections.Generic;
using DataProviders.Dashboards;
using GazRouter.DTO.Dashboards.Dashboard;
using GazRouter.DTO.Dashboards.DashboardContent;
using GazRouter.DTO.Dashboards.DashboardFolder;
using GazRouter.DTO.Dashboards.DashboardGrants;
using GazRouter.DTO.Dashboards.Folders;

namespace DataProviders.Dashboard
{
    public class DashboardDataProvider : DataProviderBase<IDashboardService>
    {
	    protected override string ServiceUri
	    {
			get { return "/Dashboards/DashboardService.svc"; }
	    }

		
		public void GetFolderList(int parameter,Func<List<FolderDTO>, Exception, bool> callback, IClientBehavior behavior)
		{
			var channel = GetChannel();
            Execute(channel, channel.BeginGetFolderList, channel.EndGetFolderList, callback,parameter, behavior);
		}

		public void DeleteFolder(int parameters, Func<Exception, bool> callback, IClientBehavior behavior)
		{
			var channel = GetChannel();
			Execute(channel, channel.BeginDeleteFolder, channel.EndDeleteFolder, callback, parameters, behavior);
		}

		public void GetDashboardGrantList(int parameters, Func<List<DashboardGrantDTO>, Exception, bool> callback, IClientBehavior behavior)
		{
			var channel = GetChannel();
			Execute(channel, channel.BeginGetDashboardGrantList, channel.EndGetDashboardGrantList, callback,parameters, behavior);
		}

        public void AddDashboard(AddDashboardParameterSet parameters, Func<int, Exception, bool> callback, IClientBehavior behavior)
        {
            var channel = GetChannel();
			Execute(channel, channel.BeginAddDashboard, channel.EndAddDashboard, callback, parameters, behavior);
        }

		public void DeleteDashboard(int parameters, Func<Exception, bool> callback, IClientBehavior behavior)
        {
            var channel = GetChannel();
			Execute(channel, channel.BeginDeleteDashboard, channel.EndDeleteDashboard, callback, parameters, behavior);
        }

		public void GetDashboardList(int parameters, Func<List<DashboardDTO>, Exception, bool> callback, IClientBehavior behavior)
        {
            var channel = GetChannel();
			Execute(channel, channel.BeginGetDashboardList, channel.EndGetDashboardList, callback, parameters, behavior);
        }

		public void MoveDashboardFolder(MoveDashboardFolderParameterSet parameters, Func<Exception, bool> callback, IClientBehavior behavior)
		{
			var channel = GetChannel();
			Execute(channel, channel.BeginMoveDashboardFolder, channel.EndMoveDashboardFolder, callback, parameters, behavior);
		}

        public void GetDashboardContent(int dashboardId, Func<DashboardContentDTO, Exception, bool> callback, IClientBehavior behavior)
		{
			var channel = GetChannel();
            Execute(channel, channel.BeginGetDashboardContent, channel.EndGetDashboardContent, callback, dashboardId, behavior);
		}

        public void UpdateDashboardContent(DashboardContentDTO content, Func<Exception, bool> callback, IClientBehavior behavior)
        {
            var channel = GetChannel();
            Execute(channel, channel.BeginUpdateDashboardContent, channel.EndUpdateDashboardContent, callback, content, behavior);
        }

		public void SetSortOrder(SetSortOrderParameterSet parameters, Func<Exception, bool> callback, IClientBehavior behavior)
		{
			var channel = GetChannel();
			Execute(channel, channel.BeginSetSortOrder, channel.EndSetSortOrder, callback, parameters, behavior);
		}
    }
}
