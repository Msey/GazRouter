using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GazRouter.Common.ViewModel;
using GazRouter.DataProviders.Balances;
using GazRouter.DTO.Balances.GasOwners;
using GazRouter.DTO.Balances.Routes;
using GazRouter.DTO.Balances.Routes.Exceptions;

namespace GazRouter.Balances.Routes
{
    public class AddEditExceptionViewModel : AddEditViewModelBase<RouteExceptionDTO, int>
    {
        private GasOwnerDTO _selectedOwner;
        private int _routeId;

        public AddEditExceptionViewModel(Action<int> closeCallback, RouteDTO routeDto, int systemId)
            : base(closeCallback)
        {
            _routeId = routeDto.RouteId.Value;
            LoadOwners(routeDto, systemId);
            SetValidationRules();
        }

        public AddEditExceptionViewModel(Action<int> closeCallback, RouteExceptionDTO dto, RouteDTO routeDto, int systemId)
            : base(closeCallback, dto)
        {
            LoadOwners(routeDto, systemId);
            Length = Model.Length;
            SetValidationRules();
        }
        

        private async void LoadOwners(RouteDTO routeDto, int systemId)
        {
            Lock();

            var owners = await new BalancesServiceProxy().GetGasOwnerListAsync(systemId);
            OwnerList = IsEdit ? owners : owners.Where(o => routeDto.ExceptionList.All(e => e.OwnerId != o.Id)).ToList();
            OnPropertyChanged(() => OwnerList);

            SelectedOwner = OwnerList.SingleOrDefault(o => o.Id == Model.OwnerId);
            
            
            Unlock();
        }


        #region Properties

        public List<GasOwnerDTO> OwnerList { get; set; }


        public GasOwnerDTO SelectedOwner
        {
            get { return _selectedOwner; }
            set
            {
                if (SetProperty(ref _selectedOwner, value))
                    SaveCommand.RaiseCanExecuteChanged();
            }
        }


        public double Length { get; set; }


        #endregion

        protected override Task<int> CreateTask => new BalancesServiceProxy().AddRouteExceptionAsync(
            new AddRouteExceptionParameterSet
            {
                RouteId = _routeId,
                OwnerId = SelectedOwner.Id,
                Length = Length
            });

        protected override Task UpdateTask => new BalancesServiceProxy().EditRouteExceptionAsync(
            new EditRouteExceptionParameterSet
            {
                RouteExceptionId = Model.RouteExceptionId,
                OwnerId = SelectedOwner.Id,
                Length = Length
            });

        protected override string CaptionEntityTypeName => "исключения маршрута";

        protected override bool OnSaveCommandCanExecute()
        {
            ValidateAll();
            return !HasErrors;
        }

        private void SetValidationRules()
        {
            AddValidationFor(() => SelectedOwner).When(() => SelectedOwner == null).Show("Не выбран поставщик");
        }
    }
}
