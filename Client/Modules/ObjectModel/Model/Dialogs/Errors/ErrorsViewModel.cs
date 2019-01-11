using System;
using System.Collections.Generic;
using System.Linq;
using GazRouter.Application;
using GazRouter.Common.Cache;
using GazRouter.Common.ViewModel;
using GazRouter.DataProviders.ObjectModel;
using GazRouter.DTO.Dictionaries.InconsistencyTypes;
using GazRouter.DTO.ObjectModel.Inconsistency;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.ServiceLocation;

namespace GazRouter.ObjectModel.Model.Dialogs.Errors
{
	public class ErrorsViewModel : DialogViewModel
	{
		public ErrorsViewModel(Action<Guid> doubleClickAction)
			: base(() => { })
		{
            SelectCommand = new DelegateCommand(() => doubleClickAction(SelectedError.DTO.EntityId), () => true);
		}

        public DelegateCommand SelectCommand { get; private set; }

        public async void LoadErrors()
		{
            Lock();
            var errs = await new ObjectModelServiceProxy().GetInconsistenciesAsync(null);
            List = errs.SelectMany(t => t.Value).Select(p => new InconsistencyWrap(p)).ToList();
            OnPropertyChanged(() => List);
            Unlock();
		}


	    public List<InconsistencyWrap> List { get; set; }


	    private InconsistencyWrap _selectedError;
        public InconsistencyWrap SelectedError
        {
            get { return _selectedError; }
            set
            {
                _selectedError = value;
                OnPropertyChanged(() => SelectedError);
            }
        }

    }

	public class InconsistencyWrap
	{
		public InconsistencyWrap(InconsistencyDTO dto)
		{
			DTO = dto;
		    Inconsistency =
		        ServiceLocator.Current.GetInstance<IClientCache>()
		            .DictionaryRepository.InconsistencyTypes.Single(t => t.InconsistencyType == dto.InconsistencyTypeId);
		}
		public InconsistencyTypeDTO Inconsistency { get; set; }
		public InconsistencyDTO DTO { get; private set; }
	}
}