using System;

namespace GazRouter.Application
{
    public static class Settings 
    {
        public static int DispatherDayStartHour { get; set; }
        public static string ServerAssemblyVersion { get; set; }
        public static DateTime ServerAssemblyDate { get; set; }
		public static Guid EnterpriseId { get; set; }
        public static int ServerTimeUtcOffset { get; set; }
    }
}
