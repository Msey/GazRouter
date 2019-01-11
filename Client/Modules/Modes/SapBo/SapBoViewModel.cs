using System.Collections.Generic;
using System.Collections.ObjectModel;
using GazRouter.DataProviders.EventLog;
using GazRouter.DTO.EventLog.EventRecipient;
using Microsoft.Practices.Prism;

namespace GazRouter.Modes.EventLog
{
    public class SapBoViewModel : EventTabViewModelBase
    {
        public SapBoViewModel()
        {
            Items = new ObservableCollection<EventRecepientDTO>();
        }

        public ObservableCollection<EventRecepientDTO> Items { get; private set; }

        protected override void LoadData()
        {
        }

        protected override void Refresh(bool refreshParent = false)
        {
        }

        public override string Header
        {
            get { return "Получатели"; }
        }

    }
}