using System.Collections.Generic;
using System.Collections.ObjectModel;
using GazRouter.DataProviders.EventLog;
using GazRouter.DTO.EventLog.EventRecipient;
using Microsoft.Practices.Prism;
using System.Threading.Tasks;

namespace GazRouter.Modes.EventLog
{
    public class RecepientsViewModel : EventTabViewModelBase
	{
        public RecepientsViewModel()
        {
            Items = new ObservableCollection<EventRecepientDTO>();
        }

        public ObservableCollection<EventRecepientDTO> Items { get; private set; }

        public override string Header => "Получатели";

        protected override void Refresh(bool refreshParent = false)
        {
            LoadData();
        }

        protected async override void LoadData()
        {
            Items.Clear();  
            if (EventDTO == null) return;

            Lock();
            Items.AddRange(await new EventLogServiceProxy().GetEventRecepientListAsync(EventDTO.Id));
            Unlock();
        }

        public async Task<List<EventRecepientDTO>> LoadData(DTO.EventLog.EventDTO dto)
        {
            return await new EventLogServiceProxy().GetEventRecepientListAsync(dto.Id);
        }
    }
}
