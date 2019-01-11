using System.Collections.Generic;

namespace GazRouter.DTO.ExcelReports
{
    public class UpdateDashboardGrantParameterSet
    {
        public List<DashboardGrantDTO> NewGrants { get; set; }
        public List<DashboardGrantDTO> UpdateGrants { get; set; }
    }
}