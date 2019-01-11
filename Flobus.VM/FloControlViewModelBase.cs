using System.Diagnostics;
using System.Threading.Tasks;
using GazRouter.Common.Events;
using GazRouter.Common.ViewModel;
using GazRouter.DataProviders.ObjectModel;
using GazRouter.DTO.Appearance.Versions;
using GazRouter.DTO.Authorization.User;
using GazRouter.Flobus.Dialogs;
using GazRouter.Flobus.VM.Dialogs;
using GazRouter.Flobus.VM.FloModel;
using GazRouter.Flobus.VM.Model;
using JetBrains.Annotations;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.ServiceLocation;

namespace GazRouter.Flobus.VM
{
    public abstract class FloControlViewModelBase : LockableViewModel
    {
        private SchemeViewModel _model;
        private DelegateCommand _loadSchemeCommand;

        protected FloControlViewModelBase()
        {
            ServiceLocator.Current.GetInstance<IEventAggregator>()
                .GetEvent<UserSettingsChangedEvent>()
                .Subscribe(OnUserSettingChanged);

            FindObjectViewModel = new FindObjectViewModel(Model);
        }

        public DelegateCommand LoadSchemeCommand
        {
            get
            {
                return _loadSchemeCommand ?? (_loadSchemeCommand = new DelegateCommand(() =>
                {
                    var loadSchemeViewModel = new LoadSchemeViewModel(CloseLoadSchemeFormCallback, AllowEditScheme);
                    loadSchemeViewModel.SchemeVersionDeleted += SchemeVersionDeleted;
                    var dlg = new LoadSchemaDialog
                    {
                        DataContext = loadSchemeViewModel,
                        Left = 200,
                        Top = 200
                    };
                    dlg.ShowDialog();
                }));
            }
        }

        public virtual bool AllowEditScheme => false;

        [CanBeNull]
        public SchemeViewModel Model
        {
            get { return _model; }
            protected set
            {
                SetProperty(ref _model, value);
                RefreshCommands();
            }
        }

        public FindObjectViewModel FindObjectViewModel { get; }

        public bool IsModelLoaded => Model != null;
                
        public void OnUserSettingChanged(UserSettings userSettings)
        {
            if (Model != null)
            {
                LoadModel();
            }
        }

        protected async void LoadModel(int schemeVersionId)
        {
            Model = null;
            Schema.IsRepair = false;
            try
            {
                Behavior.TryLock("Загрузка модели c сервера");                
                var data = await new SchemeServiceProxy().GetFullSchemeModelAsync(schemeVersionId);
                if (data.SchemeVersion != null)
                {
                    BusyMessage = "Разбор схемы";
                    var viewModel = await TaskEx.Run(() => new FloModelHelper().ParseModel(data));

                    BusyMessage = "Построение схемы";                    
                    Model = viewModel;
                    AfterModelLoaded(viewModel);
                    Model.IsChanged = false;
                    FindObjectViewModel.Model = Model;
                    OnPropertyChanged(() => IsModelLoaded);
                }
                else
                {
                    MessageBoxProvider.Alert("Версия схемы не найдена", "Открытие схемы");
                }
            }
            finally
            {
                Behavior.TryUnlock();                
            }
        }

        
        protected void LoadModel()
        {
            Debug.Assert(Model != null, "Model != null");
            LoadModel(Model.SchemeInfo.VersionId);           
        }
               
        protected abstract void AfterModelLoaded(SchemeViewModel viewModel);

        protected virtual void RefreshCommands()
        {
        }

        protected void RefreshModel()
        {
//  Hack:          хак иначе dependencypropertychanged не срабатывает

            Behavior.TryLock();
            var model = Model;
            Model = null;
            Model = model;
            Behavior.TryUnlock();
        }

        protected abstract void CloseLoadSchemeFormCallback(SchemeVersionItemDTO schemeDTO);

        private void SchemeVersionDeleted(object sender, SchemeVersionDeletedEventArgs e)
        {
            if (e.ScheveVeresionId == Model?.SchemeInfo?.VersionId)
            {
                Model = null;
            }
        }
    }
}