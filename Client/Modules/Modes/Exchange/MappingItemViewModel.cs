using System;
using System.ServiceModel;
using GazRouter.Common.ViewModel;
using GazRouter.DataProviders.Bindings;
using GazRouter.DTO.Bindings.EntityBindings;
using GazRouter.DTO.Infrastructure.Faults;
using GazRouter.Utils;
using Telerik.Windows.Controls;

namespace GazRouter.Modes.Exchange
{
    public class MappingItemViewModel : PropertyChangedBase
    {
        private readonly BindingDTO _model;
        private readonly IClientBehavior _behavior;
        private string _extEntityId;
        private bool _isActive;

        public MappingItemViewModel(BindingDTO model, IClientBehavior behavior, int sourceId)
        {
            _model = model;
            _behavior = behavior;
            SourceId = sourceId;
            _extEntityId = model.ExtEntityId;
            _isActive = model.IsActive;
        }

        public BindingDTO Model
        {
            get { return _model; }
        }


        public string Name
        {
            get { return _model.Name; }
            set { _model.Name = value; }
        }


        public string Path
        {
            get { return _model.Path; }
            set { _model.Path = value; }
        }

        public string ExtEntityId
        {
            get { return _extEntityId; }
            set
            {
                _extEntityId = value;
                OnPropertyChanged(() => ExtEntityId);
                Save();

            }
        }
        public bool IsActive
        {
            get { return _isActive; }
            set
            {
                _isActive = value;
                OnPropertyChanged(() => IsActive);
                Save();
            }
        }


        private void Save()
        {
            if (string.IsNullOrEmpty(ExtEntityId))
            {
                if (Model.Id != Guid.Empty)
                {
                    new BindingsDataProvider().DeleteEntityBindings(
                        Model.Id,
                        CallBackAfterRequestOnModifyData, _behavior);
                }

            }
            else
            {
                if (Model.Id == Guid.Empty)
                {
                    new BindingsDataProvider().AddEntityBindings(
                        new EntityBindingParameterSet
                        {
                            EntityId = Model.EntityId,
                            ExtEntityId = ExtEntityId,
                            SourceId = SourceId,
                            IsActive = true
                        },
                        (id, e) =>
                        {
                            if (e == null)
                            {
                                _isActive = true;
                                OnPropertyChanged(() => IsActive);
                            }
                            return e == null;
                        }, _behavior);
                }
                else
                {
                    new BindingsDataProvider().EditEntityBindings(
                 new EditEntityBindingParameterSet { EntityId = Model.EntityId, ExtEntityId = ExtEntityId, SourceId = SourceId, Id = Model.Id, IsActive = IsActive},
                 CallBackAfterRequestOnModifyData, _behavior);
                }
            }

        }

        protected int SourceId { get; private set; }

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