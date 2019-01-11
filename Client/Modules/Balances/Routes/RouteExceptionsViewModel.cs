using System;
using System.Collections.Generic;
using GazRouter.Common;
using GazRouter.Common.ViewModel;
using GazRouter.DataProviders.Balances;
using GazRouter.DTO.Balances.Routes;
using GazRouter.DTO.Balances.Routes.Exceptions;
using DelegateCommand = Microsoft.Practices.Prism.Commands.DelegateCommand;

namespace GazRouter.Balances.Routes
{
	public class RouteExceptionsViewModel : LockableViewModel
	{
	    private readonly RouteDTO _route;
	    private Action _exceptionChanged;
	    private int _systemId;
        public bool IsReadOnly { get; set; }
        
        public RouteExceptionsViewModel(RouteDTO route, int systemId, Action exceptionChanged)
        {
            var editPermission = Authorization2.Inst.IsEditable(LinkType.Routes);

            _route = route;
            _systemId = systemId;
            _exceptionChanged = exceptionChanged;

            AddExceptionCommand = new DelegateCommand(AddException, () => editPermission && _route != null);
            EditExceptionCommand = new DelegateCommand(EditException, () => editPermission && _selectedException != null);
            DeleteExceptionCommand = new DelegateCommand(DeleteException, () => editPermission && _selectedException != null);
        }


	    public List<RouteExceptionDTO> ExceptionList => _route?.ExceptionList;


        private RouteExceptionDTO _selectedException;
        public RouteExceptionDTO SelectedException
	    {
	        get { return _selectedException; }
            set
            {
                if (SetProperty(ref _selectedException, value))
                {
                    EditExceptionCommand.RaiseCanExecuteChanged();
                    DeleteExceptionCommand.RaiseCanExecuteChanged();
                }
            }
	    }


	    public DelegateCommand AddExceptionCommand { get; set; }

	    private void AddException()
	    {
	        var vm = new AddEditExceptionViewModel(x => _exceptionChanged?.Invoke(), _route, _systemId);
            var v = new AddEditExceptionView {DataContext = vm};
            v.ShowDialog();
	    }


        public DelegateCommand EditExceptionCommand { get; set; }

	    private void EditException()
	    {
            var vm = new AddEditExceptionViewModel(x => { }, _selectedException, _route, _systemId);
            var v = new AddEditExceptionView { DataContext = vm };
            v.ShowDialog();
        }

        public DelegateCommand DeleteExceptionCommand { get; set; }

	    private void DeleteException()
	    {
            MessageBoxProvider.Confirm(
                "Внимание! Необходимо Ваше подтверждение удаления исключения маршрута.",
                async ok =>
                {
                    if (ok)
                    {
                        await new BalancesServiceProxy().DeleteRouteExceptionAsync(SelectedException.RouteExceptionId);
                        _exceptionChanged?.Invoke();
                    }
                }, "Удаление исключения", "Удалить", "Отмена");
	    }

        public DelegateCommand RefreshRoutesCommand { get; set; }
        
	}
}
