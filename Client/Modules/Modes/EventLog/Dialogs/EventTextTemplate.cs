using GazRouter.DTO.EventLog.TextTemplates;
using Microsoft.Practices.Prism.Commands;

namespace GazRouter.Modes.EventLog.Dialogs
{
    public class EventTextTemplate
    {
        private EventTextTemplateDTO _dto;
        public EventTextTemplate(EventTextTemplateDTO dto, DelegateCommand<string> cmd)
        {
            _dto = dto;
            InsertTextCommand = cmd;
        }

        public DelegateCommand<string> InsertTextCommand { get; private set; }

        public string Name
        {
            get { return _dto.Name; }
        }
        
        public string Text 
        {
            get { return _dto.Text; }
        }
    
    }
}