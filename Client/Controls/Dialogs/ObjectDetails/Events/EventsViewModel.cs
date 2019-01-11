using System;
using System.Collections.Generic;
using GazRouter.Application;
using GazRouter.Application.Helpers;
using GazRouter.Common.ViewModel;
using GazRouter.DataProviders.EventLog;
using GazRouter.DTO.EventLog;


namespace GazRouter.Controls.Dialogs.ObjectDetails.Events
{
    public class EventsViewModel : ValidationViewModel
    {
        private readonly Guid _entityId;
        private Period _selectedPeriod;

        public EventsViewModel(Guid id)
        {
            _entityId = id;

            SelectedPeriod = new Period(new DateTime(DateTime.Today.Year, 1, 1), DateTime.Now);
            LoadEvents();
        }


        public List<EventDTO> EventList { get; set; }

        public Period SelectedPeriod
        {
            get { return _selectedPeriod; }
            set
            {
                if (SetProperty(ref _selectedPeriod, value))
                    LoadEvents();
            }
        }

        private async void LoadEvents()
        {
            try
            {
                Behavior.TryLock();

                // Загрузка списка событий по выбранному объекту 
                EventList = await new EventLogServiceProxy().GetEventListAsync(
                    new GetEventListParameterSet
                    {
                        EntityId = _entityId,
                        SiteId = UserProfile.Current.Site.Id,
                        QueryType = EventListType.Archive,
                        StartDate = SelectedPeriod.Begin,
                        EndDate = SelectedPeriod.End
                    });
                
                OnPropertyChanged(() => EventList);
            }
            finally 
            {
                Behavior.TryUnlock();
            }
            
        }
    }
}