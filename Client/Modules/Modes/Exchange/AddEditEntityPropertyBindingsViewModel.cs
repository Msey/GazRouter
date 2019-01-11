using System;
using Common.ViewModel;
using DataProviders;
using DTO.Bindings.EntityBindings;
using DTO.Bindings.PropertyBindings;
using DTO.Dictionaries.PropertyTypes;

namespace Modes.Exchange
{
    public class AddEditEntityPropertyBindingsViewModel : AddEditViewModelBase<BindingDTO, Guid>
    {
        public AddEditEntityPropertyBindingsViewModel(Action<Guid> actionBeforeClosing, BindingDTO model, int sourceId)
            : base(actionBeforeClosing, model)
        {
            Extid = model.ExtEntityId;
            Id = model.Id;
            SourcesId = sourceId;
            PropertyId = model.PropertyId;
            EntityId = model.EntityId;
        }

        public AddEditEntityPropertyBindingsViewModel(Action<Guid> actionBeforeClosing)
            : base(actionBeforeClosing)
        {
        }

        #region Extid

        private string _extId;

        public string Extid
        {
            get { return _extId; }
            set
            {
                if (_extId != value)
                {
                    _extId = value;
                    SaveCommand.RaiseCanExecuteChanged();
                }
            }
        }

        #endregion

        #region SourceId

        private int _sourcesId;

        public int SourcesId
        {
            get { return _sourcesId; }
            set
            {
                _sourcesId = value;
                OnPropertyChanged(() => SourcesId);
            }
        }

        #endregion

        #region PropertyId

        public PropertyType? PropertyId { get; set; }

        #endregion

        #region EntityId

        private Guid _entityId;

        public Guid EntityId
        {
            get { return _entityId; }
            set
            {
                _entityId = value;
                OnPropertyChanged(() => EntityId);
            }
        }

        #endregion

        #region Id

        private Guid _id;

        public Guid Id
        {
            get { return _id; }
            set
            {
                _id = value;
                OnPropertyChanged(() => Id);
            }
        }

        #endregion

        protected override string CaptionEntityTypeName
        {
            get { return "привязки"; }
        }

        protected override bool OnSaveCommandCanExecute()
        {
            return !string.IsNullOrEmpty(Extid) && SourcesId != default(int) &&
                   (PropertyId.HasValue || EntityId != default(Guid));
        }

        protected override void UpdateCurrent()
        {
            if (!PropertyId.HasValue && EntityId != default(Guid))
                new BindingsDataProvider().EditEntityBindings(
                    new EditEntityBindingParameterSet
                        {EntityId = EntityId, ExtEntityId = Extid, SourceId = SourcesId, Id = Id},
                    CallBackAfterRequestOnModifyData, Behavior);
            else
                new BindingsDataProvider().EditPropertyBindings(
                    new EditPropertyBindingParameterSet
                        {PropertyId = PropertyId.Value, ExtEntityId = Extid, Id = Id, SourceId = SourcesId},
                    CallBackAfterRequestOnModifyData, Behavior);
        }

        protected override void CreateNew()
        {
            if (!PropertyId.HasValue && EntityId != default(Guid))
                new BindingsDataProvider().AddEntityBindings(
                    new EntityBindingParameterSet {EntityId = EntityId, ExtEntityId = Extid, SourceId = SourcesId},
                    CallBackAfterRequestOnModifyData, Behavior);
            else
                new BindingsDataProvider().AddPropertyBindings(
                    new AddPropertyBindingParameterSet
                        {PropertyId = PropertyId.Value, ExtEntityId = Extid, SourceId = SourcesId},
                    CallBackAfterRequestOnModifyData, Behavior);
        }
    }
}