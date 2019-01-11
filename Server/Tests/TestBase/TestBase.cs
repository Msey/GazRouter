using System.Configuration;
using GazRouter.DAL.Infra;
using GazRouter.Log;
using TestBase.Infra;

namespace TestBase
{
    public abstract class TestBase
    {
        public ExecutionContextTest OpenDbContext()
        {
            var hostName = System.Net.Dns.GetHostName();
            var context = new ExecutionContextTest("SystemUser", ConfigurationManager.ConnectionStrings["MainDb"].ConnectionString, hostName, new MyLogger(""));
            new SetRunModeCommand(context).Execute(RunMode.Debug);

            return context;
        }

	    public const string Stable = "Stable";
		public const string UnStable = "UnStable";
    }
}
