using System;
using GazRouter.Common.Services;
using GazRouter.Common.ViewModel;
using Microsoft.Practices.Prism.Interactivity.InteractionRequest;
using Microsoft.Practices.ServiceLocation;

namespace GazRouter.ObjectModel.Model.Schema
{
    public class SchemaViewModel : PropertyChangedBase
    {
        private bool _isSelected;

        public SchemaViewModel()
        {

            FloEditControlViewModel = new FloEditControlViewModel();

        }

        public FloEditControlViewModel FloEditControlViewModel { get; }

        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                _isSelected = value; OnPropertyChanged(() => IsSelected);
                if (IsSelected && !FloEditControlViewModel.IsSchemeLoaded)
                {
                    FloEditControlViewModel.LoadSchemeCommand.Execute();
                }
            }
        }

        public void ConfirmNavigationRequest(InteractionRequest<Confirmation> navigationRequest, Action<bool> continuationCallback)
        {

            FloEditControlViewModel.ConfirmNavigationRequest(navigationRequest, continuationCallback);
        }

    
    }
}
