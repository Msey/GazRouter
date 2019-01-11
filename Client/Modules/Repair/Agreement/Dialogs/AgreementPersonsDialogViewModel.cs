using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using GazRouter.Common.ViewModel;
using GazRouter.DTO.Repairs.Agreed;
using GazRouter.DataProviders.Authorization;
using Microsoft.Practices.Prism.Commands;

namespace GazRouter.Repair.Agreement.Dialogs
{
    public class AgreementPersonsDialogViewModel : DialogViewModel<Action<IList<AgreedUserDTO>>>
    {
        private ObservableCollection<AgreedUserDTO> _SelectedPersons;
        public ObservableCollection<AgreedUserDTO> SelectedPersons => _SelectedPersons;

        private readonly List<AgreedUserDTO> _AgreementPersonList;
        public List<AgreedUserDTO> AgreementPersons => _AgreementPersonList;

        private readonly DelegateCommand _SaveCommand;
        public DelegateCommand SaveCommand => _SaveCommand;

        private readonly bool _hasAvailablePersons;
        public bool hasAvailablePersons => _hasAvailablePersons;

        private readonly string _cancelButtonText;
        public string cancelButtonText => _cancelButtonText;

        public AgreementPersonsDialogViewModel(List<AgreedUserDTO> AgreementPersons, Action<IList<AgreedUserDTO>> closeCallback) : base(closeCallback)
        {
            _hasAvailablePersons = AgreementPersons.Count > 0;
            if (_hasAvailablePersons)
                _cancelButtonText = "Отмена";
            else
                _cancelButtonText = "ОК";
            _SelectedPersons = new ObservableCollection<AgreedUserDTO>();
            _SelectedPersons.CollectionChanged += _SelectedPersons_CollectionChanged;
            _AgreementPersonList = AgreementPersons;
            _SaveCommand = new DelegateCommand(() => DialogResult = true, ()=>_SelectedPersons.Count>0 );
        }

        private void _SelectedPersons_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            SaveCommand.RaiseCanExecuteChanged();
        }

        protected override void InvokeCallback(Action<IList<AgreedUserDTO>> closeCallback)
        {
            if(closeCallback != null)
                closeCallback(SelectedPersons);
        }
    }
}
