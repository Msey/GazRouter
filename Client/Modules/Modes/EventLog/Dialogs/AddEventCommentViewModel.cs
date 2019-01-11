using System;
using System.Threading.Tasks;
using GazRouter.Common.ViewModel;
using GazRouter.DataProviders.EventLog;
using GazRouter.DTO.EventLog.Attachments;

namespace GazRouter.Modes.EventLog.Dialogs
{
    public class AddEventCommentViewModel : AddEditViewModelBase<EventAttachmentDTO, Guid>
    {
        private readonly int _eventId;

        public AddEventCommentViewModel(int eventId, Action<Guid> actionBeforeClosing)
            : base(actionBeforeClosing)
        {
            _eventId = eventId;
        }

        public AddEventCommentViewModel(EventAttachmentDTO dto, Action<Guid> actionBeforeClosing)
            : base(actionBeforeClosing, dto)
        {
            Description = dto.Description;
        }

        protected override Task<Guid> CreateTask => 
            new EventLogServiceProxy().AddEventAttachmentAsync(
                new AddEventAttachmentParameterSet
                {
                    EventId = _eventId,
                    Description = _description
                });

        protected override Task UpdateTask => 
            new EventLogServiceProxy().EditEventAttachmentAsync(
                new EditEventAttachmentParameterSet
                {
                    EventAttachmentId = Model.Id,
                    Description = Description
                });

        protected override bool OnSaveCommandCanExecute()
        {
            return !string.IsNullOrEmpty(Description) && Description != Model.Description;
        }

        protected override string CaptionEntityTypeName
        {
            get { return "примечания"; }
        }

        private string _description;
        public string Description
        {
            get { return _description; }
            set
            {
                if(SetProperty(ref _description, value))
                    SaveCommand.RaiseCanExecuteChanged();
            }
        }
    }
}