using GazRouter.DAL.Core;
using GazRouter.DAL.Infra;
using GazRouter.Log;

namespace GazRouter.DataServices.Infrastructure
{
    public static class DbContextHelper
    {
        public static ExecutionContextReal OpenDbContext(string login, MyLogger logger)
        {
            var hostName = System.Net.Dns.GetHostName();
            var context = new ExecutionContextReal(login, AppSettingsManager.ConnectionString, hostName, logger);
            new SetRunModeCommand(context).Execute(RunMode.Standard);

            return context;
        }

    }
}
