using System;
using GazRouter.Application.Wrappers.Entity;
using GazRouter.Common.ViewModel;
using GazRouter.Controls.Dialogs.ObjectDetails.Attachments;
using GazRouter.Controls.Dialogs.ObjectDetails.ChemicalTests;
using GazRouter.Controls.Dialogs.ObjectDetails.Events;
using GazRouter.Controls.Dialogs.ObjectDetails.Measurings;
using GazRouter.Controls.Dialogs.ObjectDetails.Measurings.DistrStation;
using GazRouter.Controls.Dialogs.ObjectDetails.Measurings.Valve;
using GazRouter.Controls.Dialogs.ObjectDetails.Urls;
using GazRouter.DataProviders.ObjectModel;
using GazRouter.DTO.Dictionaries.EntityTypes;

namespace GazRouter.Controls.Dialogs.ObjectDetails
{
    public class ObjectDetailsViewModel : ViewModelBase
    {
        private string _measuringsTitle = "Режим";
        public string MeasuringsTitle
        {
            get { return _measuringsTitle; }
            set
            {
                _measuringsTitle = value;
                OnPropertyChanged(() => MeasuringsTitle);
            }
        }

        private bool _valveModeVisibility = false;
        public bool ValveModeVisibility
        {
            get { return _valveModeVisibility; }
            set
            {
                _valveModeVisibility = value;
                OnPropertyChanged(() => ValveModeVisibility);
            }
        }

        public MeasuringsViewModel ValveMeasurings { get; set; }

        public ObjectDetailsViewModel(Guid id, EntityType type)
        {
            LoadWrapper(id);

            Events = new EventsViewModel(id);
            ChemicalTests = new ChemicalTestsViewModel(id);
            ValveMeasurings = new MeasuringsViewModel();
            switch (type)
            {
                case EntityType.DistrStation:
                    Measurings = new DistrStationMeasuringsViewModel(id);
                    break;

                case EntityType.Valve:
                    Measurings = new ValveStatesViewModel(id);

                    MeasuringsTitle = "Переключения";
                    ValveMeasurings.SetEntity(id, type);
                    ValveModeVisibility = true;
                    OnPropertyChanged(() => ValveMeasurings);
                    break;

                default:
                    Measurings = new MeasuringsViewModel(id, type);
                    break;
            }

            Attachments = new AttachmentsViewModel { EntityId = id, IsActive = true, IsReadOnly = true };
            Urls = new UrlsViewModel { EntityId = id, IsActive = true, IsReadOnly = true };
        }
       

        public IEntityWrapper Wrapper { get; set; }
        public EventsViewModel Events { get; set; }
        public ChemicalTestsViewModel ChemicalTests { get; set; }

        public ViewModelBase Measurings { get; set; }

        public AttachmentsViewModel Attachments { get; set; }

        public UrlsViewModel Urls { get; set; }


        private async void LoadWrapper(Guid id)
        {
            var entity = await new ObjectModelServiceProxy().GetEntityByIdAsync(id);
            Wrapper = entity.GetWrapper(false);
            OnPropertyChanged(() => Wrapper);
        }
    }
}