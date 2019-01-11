using System;
using System.ServiceModel;
using GazRouter.Common.ViewModel;
using GazRouter.DataProviders.Bindings;
using GazRouter.DTO.Bindings.EntityPropertyBindings;
using GazRouter.DTO.Dictionaries.PeriodTypes;
using GazRouter.DTO.Dictionaries.Targets;
using GazRouter.DTO.Infrastructure.Faults;
using GazRouter.Utils;
using Telerik.Windows.Controls;

namespace GazRouter.Modes.Exchange
{
    public class PropertyMappingItemViewModel : PropertyChangedBase
    {
        private readonly IClientBehavior _behavior;
        private string _extKey;
        private readonly Guid _entityId;
        private readonly int _sourceId;
        private readonly PeriodType _periodTypeId;
        private readonly Target _target;

        public PropertyMappingItemViewModel(EntityPropertyBindingDTO model, IClientBehavior behavior, Guid entityId, int sourceId, PeriodType periodTypeId, Target target)
        {
            _behavior = behavior;
            Model = model;
            _entityId = entityId;
            _sourceId = sourceId;
            _periodTypeId = periodTypeId;
            _target = target;
            _extKey = model.ExtKey;
        }

        public EntityPropertyBindingDTO Model { get; private set; }

        public string ExtKey
        {
            get { return _extKey; }
            set
            {
                _extKey = value;
                OnPropertyChanged(() => ExtKey);
                Save();
            }
        }

        private void Save()
        {
       
            if (string.IsNullOrEmpty(ExtKey))
            {
                if (Model.Id != Guid.Empty)
                {
                    new BindingsDataProvider().DeleteEntityPropertyBindings(
                        Model.Id,
                        CallBackAfterRequestOnModifyData, _behavior);
                }

            }
            else
            {
                if (Model.Id == Guid.Empty)
                {
                    new BindingsDataProvider().AddEntityPropertyBindings(
                        new AddEntityPropertyBindingParameterSet
                        {
                            EntityId =  _entityId,
                            ExtKey = ExtKey,
                            SourceId = _sourceId,
                            PeriodTypeId = _periodTypeId,
                            PropertyId = Model.PropertyId
                            
                        },
                        CallBackAfterRequestOnModifyData, _behavior);
                }
                else
                {
                    new BindingsDataProvider().EditEntityPropertyBindings(
                        new EditEntityPropertyBindingParameterSet
                            {
                            EntityId = _entityId,
                            ExtKey = ExtKey,
                            SourceId = _sourceId,
                            PeriodTypeId = _periodTypeId,
                            PropertyId =Model.PropertyId,
                            Id = Model.Id
                        },
                        CallBackAfterRequestOnModifyData, _behavior);
                }
            }

        }

        private bool CallBackAfterRequestOnModifyData(Guid id, Exception ex)
        {
            return CallBackAfterRequestOnModifyData(ex);
        }

        private bool CallBackAfterRequestOnModifyData(Exception ex)
        {
            if (ex == null)
            {
                return true;
            }

            var faultException = ex as FaultException<FaultDetail>;
            if (faultException != null && faultException.Detail.FaultType == FaultType.IntegrityConstraint)
            {
                RadWindow.Alert(new DialogParameters { Header = "Ошибка", Content = faultException.Detail.Message });
                return true;
            }
            return false;
        }

    }
}