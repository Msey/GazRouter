using System.Collections.Generic;
using System.Linq;
using GazRouter.Common.ViewModel;
using GazRouter.DTO.Bindings.Sources;

namespace GazRouter.Modes.Exchange
{
    public class MappingViewModel : LockableViewModel
    {
        public MappingViewModel()
        {

            SourcesList = new List<Source>();
            PropertyMapping = new PropertyMappingViewModel(this);   
        
            ObjectMapping = new ObjectMappingViewModel(this);
        }

        public ObjectMappingViewModel ObjectMapping { get; private  set; }

        public void Refresh()
        {
            LoadSourcesList();
        }

		#region IsSelected

	    public bool IsSelected
	    {
		    get { return ObjectMapping.IsSelected; }
		    set
		    {
			    ObjectMapping.IsSelected = value;
			    OnPropertyChanged(() => IsSelected);
		    }
	    }

	    #endregion


        #region SelectedSource

		private Source _selectedSource;

        public Source SelectedSource
        {
            get { return _selectedSource; }
            set
            {
                _selectedSource = value;
                OnPropertyChanged(() => SelectedSource);
             
            }
        }

        #endregion SelectedSource

        #region SourcesList

        private List<Source> _sourcesList;

        public List<Source> SourcesList
        {
            get { return _sourcesList; }
            set
            {
                _sourcesList = value;
                OnPropertyChanged(() => SourcesList);
            }
        }

        private void LoadSourcesList()
        {
            SourcesList = ExchangesViewModel.SharedSourceList;
            SelectedSource = SourcesList.FirstOrDefault();
        }

        #endregion

        #region Name

        private string _name;

        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                OnPropertyChanged(() => Name);
            }
        }

        #endregion Name


        public PropertyMappingViewModel PropertyMapping { get; private set; }

     
    }
}