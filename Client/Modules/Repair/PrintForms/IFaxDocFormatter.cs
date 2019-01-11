using GazRouter.DTO.ObjectModel;
using GazRouter.DTO.Repairs.Agreed;
using GazRouter.DTO.Repairs.Plan;
using System.Collections.Generic;
using System.Threading.Tasks;
using Telerik.Windows.Documents.Model;


namespace GazRouter.Repair.PrintForms
{
    public interface IFaxDocFormatter
    {
        Task<RadDocument> CreatePrintDocument();
    }
}
