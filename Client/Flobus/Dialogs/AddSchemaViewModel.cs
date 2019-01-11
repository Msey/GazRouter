using System;
using System.Collections.Generic;
using GazRouter.Common.ViewModel;
using GazRouter.DataProviders.ObjectModel;
using GazRouter.DTO.Appearance;
using GazRouter.DTO.Dictionaries.GasTransportSystems;
using Microsoft.Practices.Prism.Commands;
using GazRouter.DTO.Appearance.Versions;

namespace GazRouter.Flobus.Dialogs
{
    public class AddSchemaViewModel : DialogViewModel
    {
        public int Id;
        public DelegateCommand AddCommand { get; }

        public DelegateCommand AddCopyCommand { get; }

        public string Header => IsEdit ? "Сохранить как" : "Добавить схему";
        public bool IsAdd { get; set; }
        public bool IsEdit { get; set; }
        public string Content { get; set; }
        public int? SystemId;

        public AddSchemaViewModel(Action closeCallback, string content = null, int? systemId = null, bool IsEdit = false) : base(closeCallback)
        {
            AddCommand = new DelegateCommand(AddCommandExecute,
                () => (SelectedGasTransport != null && !string.IsNullOrEmpty(Name)));
            AddCopyCommand = new DelegateCommand(AddCopyCommandExecute,
                () => (!string.IsNullOrEmpty(Name)));
            IsAdd = !IsEdit;
            this.IsEdit = IsEdit;
            Content = content;
            SystemId = systemId;

            ListGasTransportSystems = ClientCache.DictionaryRepository.GasTransportSystems;
        }

        private async void AddCommandExecute()
        {
            Behavior.TryLock();
            try
            {
                Id = await new SchemeServiceProxy().AddSchemeAsync(new SchemeParameterSet
                {
                    SystemId = SelectedGasTransport.Id,
                    Name = Name,
                    Description = Description
                });
                DialogResult = true;

            }
            finally 
            {
                Behavior.TryUnlock();
            }
        }

        private bool _saveInstance;
        private async void AddCopyCommandExecute()
        {
            if (!_saveInstance)
            {
                _saveInstance = true;
                Behavior.TryLock();
                try
                {
                    int empty_versionId = await new SchemeServiceProxy().AddSchemeAsync(new SchemeParameterSet
                    {
                        SystemId = SystemId,
                        Name = Name,
                        Description = Description
                    });

                    var data = await new SchemeServiceProxy().GetFullSchemeModelAsync(empty_versionId);

                    await new SchemeServiceProxy().DeleteSchemeVersionAsync(empty_versionId);

                    Id = await new SchemeServiceProxy().AddSchemeVersionAsync(new SchemeVersionParameterSet
                    {
                        SchemeId = data.SchemeVersion.SchemeId,
                        Content = Content
                    });

                    DialogResult = true;
                }
                finally
                {                    
                    Behavior.TryUnlock();
                }
            }            
        }

        private List<GasTransportSystemDTO> _listGasTransportSystems = new List<GasTransportSystemDTO>();
        public List<GasTransportSystemDTO> ListGasTransportSystems
        {
            get { return _listGasTransportSystems; }
            set
            {
                if (_listGasTransportSystems == value) return;
                _listGasTransportSystems = value;
                OnPropertyChanged(() => ListGasTransportSystems);
            }
        }

        private GasTransportSystemDTO _selectedGasTransport;
        public GasTransportSystemDTO SelectedGasTransport
        {
            get
            {
                return _selectedGasTransport;
            }
            set
            {
                if (_selectedGasTransport == value) return;
                _selectedGasTransport = value;
                OnPropertyChanged(() => SelectedGasTransport);
                AddCommand.RaiseCanExecuteChanged();
                AddCopyCommand.RaiseCanExecuteChanged();
            }
        }

        private string _name;
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
                OnPropertyChanged(() => Name);
                AddCommand.RaiseCanExecuteChanged();
                AddCopyCommand.RaiseCanExecuteChanged();
            }
        }

        private string _description;
        public string Description
        {
            get
            {
                return _description;
            }
            set
            {
                _description = value;
                OnPropertyChanged(() => Description);
            }
        }
    }
}
