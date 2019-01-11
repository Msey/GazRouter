using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GazRouter.Application;
using GazRouter.DataProviders.Balances;
using GazRouter.DataProviders.ObjectModel;
using GazRouter.DTO.Balances.BalanceGroups;
using GazRouter.DTO.Dictionaries.GasTransportSystems;
using GazRouter.DTO.ObjectModel.Sites;

namespace GazRouter.ObjectModel.Model.Dialogs.AddEdit.Objects.Site
{
    public class AddEditSiteViewModel : AddEditViewModelBase<SiteDTO>
    {
        private readonly Guid _enterpriseId;

        #region SaveCommand

        protected override bool OnSaveCommandCanExecute()
        {
            return !string.IsNullOrEmpty(Name) && SelectedGasTransport != null;
        }

		protected override Task UpdateTask
		{
			get
			{
				return new ObjectModelServiceProxy().EditSiteAsync(
					 new EditSiteParameterSet
					 {
						 Name = Name,
						 Id = Model.Id,
						 ParentId = _enterpriseId,
						 GasTransportSystemId = SelectedGasTransport.Id,
                         BalanceGroupId = SelectedBalGroup?.Id
					 });
			}
		}

		protected override Task<Guid> CreateTask
		{
			get
			{
				return new ObjectModelServiceProxy().AddSiteAsync(
					new AddSiteParameterSet
					{
						Name = Name,
						ParentId = _enterpriseId,
						GasTransportSystemId = SelectedGasTransport.Id,
						SortOrder = _sortorder,
                        BalanceGroupId = SelectedBalGroup?.Id
                    });
			}
		}

        #endregion SaveCommand

        public AddEditSiteViewModel(Action<Guid> actionBeforeClosing, SiteDTO model)
            : base(actionBeforeClosing, model)
        {
            _enterpriseId = model.ParentId.Value;
            ListGasTransportSystems = ClientCache.DictionaryRepository.GasTransportSystems;
            SelectedGasTransport = ListGasTransportSystems.FirstOrDefault(p => p.Id == model.SystemId);
            
            LoadBalGroupList();
        }

		public AddEditSiteViewModel(Action<Guid> actionBeforeClosing, Guid enterpriseId, int gasTransportSystemId, int sortorder)
            : base(actionBeforeClosing)
        {
            ListGasTransportSystems = ClientCache.DictionaryRepository.GasTransportSystems;
            SelectedGasTransport = ListGasTransportSystems.FirstOrDefault(p => p.Id == gasTransportSystemId);

            _enterpriseId = enterpriseId;
			_sortorder = sortorder;

		    LoadBalGroupList();
        }


        public List<BalanceGroupDTO> BalGroupList { get; set; }

        public BalanceGroupDTO SelectedBalGroup { get; set; }

        private async void LoadBalGroupList()
        {
            Lock();
            BalGroupList = await new BalancesServiceProxy().GetBalanceGroupListAsync(SelectedGasTransport.Id);
            OnPropertyChanged(() => BalGroupList);
            SelectedBalGroup = BalGroupList.SingleOrDefault(g => g.Id == Model.BalanceGroupId);
            OnPropertyChanged(() => SelectedBalGroup);
            Unlock();
        }


        protected override string CaptionEntityTypeName
        {
            get { return "ЛПУ"; }
        }

	    private int _sortorder;

        #region ListGasTransportSystems

        private List<GasTransportSystemDTO> _listGasTransportSystems = new List<GasTransportSystemDTO>();

        public List<GasTransportSystemDTO> ListGasTransportSystems
        {
            get { return _listGasTransportSystems; }
            set { SetProperty(ref _listGasTransportSystems, value); }
        }

        #endregion

        #region SelectedGasTransport

        private GasTransportSystemDTO _selectedGasTransport;

        public GasTransportSystemDTO SelectedGasTransport
        {
            get { return _selectedGasTransport; }
            set
            {
                if (SetProperty(ref _selectedGasTransport, value))
                    SaveCommand.RaiseCanExecuteChanged();
            }
        }

        #endregion
    }
}