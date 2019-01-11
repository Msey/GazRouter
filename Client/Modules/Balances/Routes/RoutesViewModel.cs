using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using GazRouter.Common;
using GazRouter.Common.ViewModel;
using GazRouter.DTO.Dictionaries.GasTransportSystems;
using GazRouter.DTO.Balances.Routes;
using GazRouter.DTO.Dictionaries.EntityTypes;
using GazRouter.DTO.ObjectModel;
using DelegateCommand = Microsoft.Practices.Prism.Commands.DelegateCommand;

namespace GazRouter.Balances.Routes
{
	public class RoutesViewModel : LockableViewModel
	{
        public RoutesViewModel()
        {
            IsReadOnly = !Authorization2.Inst.IsEditable(LinkType.Routes);

            _selectedSystem = SystemList.FirstOrDefault();
		    _selectedOutletType = OutletTypeList.First();
            _selectedInputType = InputType.ByInlet;
            
            RefreshCommand = new DelegateCommand(LoadData);
            
            LoadData();
		}


        public bool IsReadOnly { get; set; }


        #region СПИСОК ГТС
        public List<GasTransportSystemDTO> SystemList => ClientCache.DictionaryRepository.GasTransportSystems;


        private GasTransportSystemDTO _selectedSystem;
        public GasTransportSystemDTO SelectedSystem
        {
            get { return _selectedSystem; }
            set
            {
                if(SetProperty(ref _selectedSystem, value))
                    LoadData();
            }
        }

        #endregion


        #region ТИП ТОЧКИ СДАЧИ ГАЗА

        public IEnumerable<EntityType> OutletTypeList
        {
            get
            {
                yield return EntityType.MeasStation; // Транзит
                yield return EntityType.DistrStation; // Потребители
                yield return EntityType.OperConsumer; // ПЭН
            }
        }

        private EntityType _selectedOutletType;
        
        public EntityType SelectedOutletType
        {
            get { return _selectedOutletType; }
            set
            {
                if (SetProperty(ref _selectedOutletType, value))
                {
                    if (_selectedInputType == InputType.ByInlet)
                        RefreshRightPointList();
                    else
                        RefreshLeftPointList();
                }
            }
        }

        #endregion


        #region ТИП ВВОДА МАРШРУТА

	    public enum InputType
	    {
	        ByInlet = 0,
            ByOutlet = 1
	    }

	    private InputType _selectedInputType;

	    public int SelectedInputType
	    {
	        get { return (int)_selectedInputType; }
	        set
	        {
	            if (SetProperty(ref _selectedInputType, (InputType)value))
	            {
	                RefreshLeftPointList();
	            }
	        }
	    }

        #endregion

        public DelegateCommand RefreshCommand { get; set; }

        private RoutesData _data;
        private async void LoadData()
        {
            Lock();

            _data = await RoutesData.GetData(SelectedSystem.Id);

            RefreshLeftPointList();

            Unlock();
        }


        #region LEFT POINT LIST

        public List<RoutePoint> LeftPointList { get; set; }

        private RoutePoint _selectedLeftPoint;
        
        public RoutePoint SelectedLeftPoint
        {
            get { return _selectedLeftPoint; }
            set
            {
                if (SetProperty(ref _selectedLeftPoint, value))
                {
                    RefreshRightPointList();
                }
            }
        }

        private void RefreshLeftPointList()
        {
            var prevSelectedPointId = _selectedLeftPoint?.Id;
            RoutePoint selectedPoint = null;

            LeftPointList = new List<RoutePoint>();

            var entities = _selectedInputType == InputType.ByInlet
                ? _data.GetInletList()
                : _data.GetOutletList(_selectedOutletType);


            foreach (var site in _data.Sites)
            {
                var siteItem = new RoutePoint(site);

                foreach (var entity in entities.Where(e => e.ParentId == site.Id))
                {
                    var entityItem = new RoutePoint(entity) { RouteCount = _data.GetRouteCount(entity.Id) };
                    if (entityItem.Id == prevSelectedPointId) selectedPoint = entityItem;
                    siteItem.Childs.Add(entityItem);
                }
                if (siteItem.Childs.Any())
                    LeftPointList.Add(siteItem);
            }

            OnPropertyChanged(() => LeftPointList);

            SelectedLeftPoint = selectedPoint;
        }

        #endregion



        #region RIGHT POINT LIST

        public List<RoutePoint> RightPointList { get; set; }

        private RoutePoint _selectedRightPoint;

        public RoutePoint SelectedRightPoint
        {
            get { return _selectedRightPoint; }
            set
            {
                if (SetProperty(ref _selectedRightPoint, value))
                {
                    if (_selectedRightPoint == null) return;

                    RouteExceptions = new RouteExceptionsViewModel(
                        _data.GetRoute(_selectedLeftPoint.Id, _selectedRightPoint.Id),
                        _selectedSystem.Id,
                        async () =>
                        {
                            await _data.ReloadRoutes();
                            RefreshRightPointList();
                        });
                    OnPropertyChanged(() => RouteExceptions);
                }
            }
        }

        private void RefreshRightPointList()
        {
            var prevSelectedPointId = _selectedRightPoint?.Id;
            RoutePoint selectedPoint = null;

            RightPointList = new List<RoutePoint>();

            if (_selectedLeftPoint != null && _selectedLeftPoint.EntityType != EntityType.Site)
            {
                var entities = _selectedInputType == InputType.ByInlet
                    ? _data.GetOutletList(_selectedOutletType)
                    : _data.GetInletList();


                foreach (var site in _data.Sites)
                {
                    var siteItem = new RoutePoint(site);
                    
                    foreach (var entity in entities.Where(e => e.ParentId == site.Id))
                    {
                        var entityItem = new InputRoutePoint(entity, _data.GetRoute(_selectedLeftPoint.Id, entity.Id), SetRoute);
                        if (entityItem.Id == prevSelectedPointId) selectedPoint = entityItem;
                        siteItem.Childs.Add(entityItem);
                    }
                    if (siteItem.Childs.Any())
                        RightPointList.Add(siteItem);
                }
            }
            OnPropertyChanged(() => RightPointList);

            SelectedRightPoint = selectedPoint;
        }

	    #endregion
        
        private async void SetRoute(Guid entityId, double? len)
        {
            var inletId = _selectedInputType == InputType.ByInlet ? _selectedLeftPoint.Id : entityId;
            var outletId = _selectedInputType == InputType.ByInlet ? entityId : _selectedLeftPoint.Id;
            
            await _data.SetRoute(inletId, outletId, len);
            //RefreshLeftPointList();
        }


        public RouteExceptionsViewModel RouteExceptions { get; set; }
    }

    public class RoutePoint : PropertyChangedBase
    {
        private readonly CommonEntityDTO _entity;
        public RoutePoint(CommonEntityDTO entity)
        {
            _entity = entity;

            Childs = new List<RoutePoint>();
        }
        public List<RoutePoint> Childs { get; }
        public string Name => _entity.Name;

        public Guid Id => _entity.Id;

        public EntityType EntityType => _entity.EntityType;

        public int? RouteCount { get; set; }

        public bool HasNoRoute => RouteCount == 0;

        public FontWeight FontWeight => _entity.EntityType == EntityType.Site ? FontWeights.Bold : FontWeights.Normal;
    }
    
    
    public class InputRoutePoint : RoutePoint
    {
        private Action<Guid, double?> _setRouteAction;
        public InputRoutePoint(CommonEntityDTO entity, RouteDTO route, Action<Guid, double?> setRouteAction)
            : base(entity)
        {
            _length = route?.Length;
            _setRouteAction = setRouteAction;
            ExceptionsCount = route?.ExceptionsCount;
        }

        private double? _length;
        public double? Length
        {
            get { return _length; }
            set
            {
                if (SetProperty(ref _length, value))
                {
                    _setRouteAction(Id, value);
                }
            }
        }

        public int? ExceptionsCount { get; }

        public bool HasExceptions => ExceptionsCount.HasValue;
    }
}
