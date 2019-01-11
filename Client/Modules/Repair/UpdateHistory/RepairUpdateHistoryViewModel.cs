using System.Collections.ObjectModel;
using System.Linq;
using GazRouter.Common.ViewModel;
using GazRouter.DataProviders.Repairs;
using Microsoft.Practices.Prism;

namespace GazRouter.Repair.UpdateHistory
{
    public class RepairUpdateHistoryViewModel : LockableViewModel
    {
        public RepairUpdateHistoryViewModel(int repairId)
        {
            RepairUpdateList = new ObservableCollection<RepairUpdate>();

            FillRepairHistory(repairId);
        }

        public ObservableCollection<RepairUpdate> RepairUpdateList { get; set; }

        private async void FillRepairHistory(int repairId)
        {
            Behavior.TryLock();
            try
            {
                var list = await new RepairsServiceProxy().GetRepairUpdateHistoryAsync(repairId);
                RepairUpdateList.AddRange(list.Select(ru => new RepairUpdate(ru)).ToList());
            }
            finally
            {
                Behavior.TryUnlock();
            }
        }
    }
}