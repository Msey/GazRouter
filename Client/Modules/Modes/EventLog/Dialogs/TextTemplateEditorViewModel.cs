using System.Collections.Generic;
using System.Linq;
using GazRouter.Application;
using GazRouter.Common.ViewModel;
using GazRouter.DataProviders.EventLog;
using GazRouter.DTO.EventLog.EventTextTemplates;
using GazRouter.DTO.EventLog.TextTemplates;
using Microsoft.Practices.Prism.Commands;

namespace GazRouter.Modes.EventLog.Dialogs
{
    public class TextTemplateEditorViewModel : ViewModelBase
    {
        private string _name;
        private EventTextTemplateDTO _selectedTemplate;
        private string _text;

        public TextTemplateEditorViewModel()
        {
            LoadTemplateList(0);

            AddCommand = new DelegateCommand(AddTemplate);
            RemoveCommand = new DelegateCommand(RemoveTemplate, () => SelectedTemplate != null);
            UpdateCommand = new DelegateCommand(UpdateTemplate, () => SelectedTemplate != null && IsModified);
        }


        public List<EventTextTemplateDTO> TemplateList { get; set; }

        public EventTextTemplateDTO SelectedTemplate
        {
            get { return _selectedTemplate; }
            set
            {
                if (IsModified)
                    MessageBoxProvider.Confirm("Шаблон был изменен. Сохранить изменения?",
                        result =>
                        {
                            if (result)
                                UpdateTemplate();

                            UpdateSelectedTemplate(value);
                        }, "Подтверждение сохранения");

                else
                {
                    UpdateSelectedTemplate(value);
                }
            }
        }

        public DelegateCommand RefreshCommand { get; set; }
        public DelegateCommand AddCommand { get; set; }
        public DelegateCommand UpdateCommand { get; set; }
        public DelegateCommand RemoveCommand { get; set; }


        public string Text
        {
            get { return _text; }
            set
            {
                _text = value;
                OnPropertyChanged(() => Text);
                UpdateCommands();
            }
        }

        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                OnPropertyChanged(() => Name);
                UpdateCommands();
            }
        }

        public bool IsModified
        {
            get { return SelectedTemplate != null && (SelectedTemplate.Name != Name || SelectedTemplate.Text != Text); }
        }

        private void UpdateSelectedTemplate(EventTextTemplateDTO value)
        {
            _selectedTemplate = value;
            OnPropertyChanged(() => SelectedTemplate);
            if (_selectedTemplate != null)
            {
                Name = value.Name;
                Text = value.Text;
            }
            else
            {
                Name = string.Empty;
                Text = string.Empty;
            }
            UpdateCommands();
        }


        private void UpdateCommands()
        {
            RemoveCommand.RaiseCanExecuteChanged();
            UpdateCommand.RaiseCanExecuteChanged();
        }

        private async void LoadTemplateList(int templId)
        {
            _selectedTemplate = null;
            Behavior.TryLock();
            try
            {
                TemplateList = await new EventLogServiceProxy().GetEventTextTemplateListAsync(UserProfile.Current.Site.Id);
            }
            finally
            {
                Behavior.TryUnlock();
            }

            if (templId > 0 && TemplateList.Any(t => t.Id == templId))
                SelectedTemplate = TemplateList.Single(t => t.Id == templId);
            else if (TemplateList.Count > 0)
                SelectedTemplate = TemplateList.First();
            else
                SelectedTemplate = null;

            OnPropertyChanged(() => TemplateList);
        }


        private async void AddTemplate()
        {
            int id;
            Behavior.TryLock();
            try
            {
                id = await new EventLogServiceProxy().AddEventTextTemplateAsync(new AddEventTextTemplateParameterSet
                {
                    Name = "Новый шаблон",
                    Text = ""
                });
            }
            finally
            {
                Behavior.TryUnlock();
            }
            
            LoadTemplateList(id);
        }

        private async void UpdateTemplate()
        {
            Behavior.TryLock();
            try
            {
                await new EventLogServiceProxy().EditEventTextTemplateAsync(new EditEventTextTemplateParameterSet
                {
                    Id = SelectedTemplate.Id,
                    Name = _name,
                    Text = _text
                });
            }
            finally
            {
                Behavior.TryUnlock();
            }
            LoadTemplateList(SelectedTemplate.Id);
          
        }


        private async void RemoveTemplate()
        {
            await new EventLogServiceProxy().DeleteEventTextTemplateAsync(SelectedTemplate.Id);
            LoadTemplateList(0);
        }
    }
}