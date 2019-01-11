using GazRouter.DTO.GasCosts;
using Telerik.Windows.Controls;

namespace GazRouter.Modes.GasCosts
{
    public class ColumnDescription
    {
        public ColumnDescription(CostType costType, string fullName)
        {
            CostType = costType;
            FullName = fullName;
        }

        public CostType CostType { get; set; }
        public string FullName { get; set; }
        public GridViewLength Width { get; set; }
    }
}