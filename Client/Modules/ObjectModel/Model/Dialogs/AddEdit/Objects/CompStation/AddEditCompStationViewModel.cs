using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GazRouter.Application;
using GazRouter.DataProviders.ObjectModel;
using GazRouter.DTO.Dictionaries.Regions;
using GazRouter.DTO.ObjectModel.CompStations;

namespace GazRouter.ObjectModel.Model.Dialogs.AddEdit.Objects.CompStation
{
    public sealed class AddEditCompStationViewModel : AddEditViewModelBase<CompStationDTO, Guid>
    {
        #region ListRegion

        private List<RegionDTO> _listRegion = new List<RegionDTO>();

        public List<RegionDTO> ListRegion
        {
            get { return _listRegion; }
            set { SetProperty(ref _listRegion, value); }
        }

        #endregion

        #region SelectedRegion

        private RegionDTO _selectedRegion;

        public RegionDTO SelectedRegion
        {
            get { return _selectedRegion; }
            set
            {
                if (SetProperty(ref _selectedRegion, value))
                    SaveCommand.RaiseCanExecuteChanged();
            }
        }
        #endregion

        private bool _useInBalance;

        public bool UseInBalance
        {
            get { return _useInBalance; }
            set { SetProperty(ref _useInBalance, value); }
        }

        

        #region SaveCommand

        protected override bool OnSaveCommandCanExecute()
        {
            return !string.IsNullOrEmpty(Name) &&
                   SelectedRegion != null;
        }

        #endregion SaveCommand

        private readonly Guid _siteId;

        public AddEditCompStationViewModel(Action<Guid> actionBeforeClosing, CompStationDTO dto)
            : base(actionBeforeClosing, dto)
        {
            _siteId = dto.ParentId.Value;
            Name = dto.Name;
            UseInBalance = dto.UseInBalance;
            LoadRegions();
        }

        public AddEditCompStationViewModel(Action<Guid> actionBeforeClosing, Guid siteId, int sortorder)
            : base(actionBeforeClosing)
        {
            _siteId = siteId;
            _sortorder = sortorder;
            LoadRegions();
        }

        private int _sortorder;

        protected override string CaptionEntityTypeName
        {
            get { return "КС"; }
        }

        private void LoadRegions()
        {
            var dtos = ClientCache.DictionaryRepository.Regions;
            ListRegion.Clear();
            ListRegion.AddRange(dtos.OrderBy(p => p.Name).ToList());
            SelectedRegion = ListRegion.FirstOrDefault(p => Equals(p.Id, Model.RegionId)) ?? ListRegion.First();
        }

        protected override Task UpdateTask
        {
            get
            {
                return new ObjectModelServiceProxy().EditCompStationAsync(
                    new EditCompStationParameterSet
                    {
                        Id = Model.Id,
                        Name = Name,
                        ParentId = _siteId,
                        RegionId = SelectedRegion.Id,
                        UseInBalance = UseInBalance,
                    });
            }
        }

        protected override Task<Guid> CreateTask
        {
            get
            {
                return new ObjectModelServiceProxy().AddCompStationAsync(
                    new AddCompStationParameterSet
                    {
                        Name = Name,
                        ParentId = _siteId,
                        RegionId = SelectedRegion.Id,
                        UseInBalance = UseInBalance,
                        SortOrder = _sortorder
                    });
            }
        }
    }
}