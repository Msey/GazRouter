using System;
using System.ComponentModel;
using System.Threading;
using System.Windows.Input;
using GazRouter.Common;
using GazRouter.DTO.Appearance.Versions;
using GazRouter.Flobus.FloScheme;
using GazRouter.Flobus.VM;
using GazRouter.Flobus.VM.FloModel;
using GazRouter.Flobus.VM.Model;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Interactivity.InteractionRequest;
using Microsoft.Practices.Prism.Regions;
using Telerik.Windows.Controls;
using DelegateCommand = Microsoft.Practices.Prism.Commands.DelegateCommand;
using GazRouter.Flobus.Dialogs;

namespace GazRouter.ObjectModel.Model.Schema
{
    [RegionMemberLifetime(KeepAlive = false)]
    public class FloEditControlViewModel : FloControlViewModelBase
    {
        AddSchemaViewModel _model;
        private SchemeLayers _visibleLayers;
        private ICommand _gotoTreeCommand;

        private bool _isEditPermission;

        private bool IsEditPermission
        {
            get { return _isEditPermission; }
            set
            {
                _isEditPermission = value;
                OnPropertyChanged(() => IsEditPermission);
            }
        }

        public FloEditControlViewModel()
        {
            IsEditPermission = Authorization2.Inst.IsEditable(LinkType.ObjectModel);
            IsMeasuringLayer = true;
            SaveSchemeCommand = new DelegateCommand(async () =>
            {
                Behavior.TryLock();
                await FloModelHelper.SaveModelAsync(Model);
                RefreshModel();
                Behavior.TryUnlock();
            }, () => Model != null && Model.IsChanged && IsEditPermission);

            SaveAsSchemeCommand = new DelegateCommand(SaveAsNew,()=> Model != null && IsEditPermission);
            ExportSchemeCommand = new DelegateCommand<Flobus.Schema>(s => s?.Export(), s => Model != null);
            RefreshCommand = new DelegateCommand(LoadModel, () => Model != null);
        }
        public DelegateCommand SaveSchemeCommand { get; }
        public DelegateCommand SaveAsSchemeCommand { get; }
        public DelegateCommand<Flobus.Schema> ExportSchemeCommand { get; }
        public DelegateCommand RefreshCommand { get; }

        public override bool AllowEditScheme => true;

        public ICommand GotoTreeCommand
        {
            get { return _gotoTreeCommand; }
            set
            {
                _gotoTreeCommand = value;
                OnPropertyChanged(() => GotoTreeCommand);
            }
        }

        public bool IsSchemeLoaded { get; set; }

        public bool IsMeasuringLayer
        {
            get { return VisibleLayers.HasFlag(SchemeLayers.Measurings); }
            set
            {
                if (value)
                {
                    VisibleLayers |= SchemeLayers.Measurings;
                }
                else
                {
                    VisibleLayers &= ~SchemeLayers.Measurings;
                }
            }
        }

        public SchemeLayers VisibleLayers
        {
            get { return _visibleLayers; }
            set
            {
                _visibleLayers = value;
                OnPropertyChanged(() => VisibleLayers);
            }
        }

        public void ConfirmNavigationRequest(InteractionRequest<Confirmation> navigationRequest,
            Action<bool> continuationCallback)
        {
            if (!IsEditPermission)
            {
                continuationCallback(true);
                return;
            }
            //
            if (Model?.IsChanged ?? false)
            {
                RadWindow.Confirm(new DialogParameters
                {
                    CancelButtonContent = "Нет",
                    OkButtonContent = "Да",
                    Content = "На схеме есть изменения. При переходе, они будут потеряны. Сохранить?",
                    Header = "Переход в другой модуль",
                    Closed =
                        (s, args) =>
                        {
                            if (args.DialogResult == true)
                            {
                                SaveSchemeCommand.Execute();
                            }
                            continuationCallback(true);
                        }
                });
                Thread.Sleep(TimeSpan.FromSeconds(1));
            }
            else
            {
                continuationCallback(true);
            }
        }

        protected override void CloseLoadSchemeFormCallback(SchemeVersionItemDTO schemeDTO)
        {
            LoadModel(schemeDTO.Id);
            IsSchemeLoaded = true;
        }

        protected override void AfterModelLoaded(SchemeViewModel viewModel)
        {
            RefreshCommands();
        }

        public void SaveAsNew()
        {
            _model = new AddSchemaViewModel(() =>
            {
                if (_model.DialogResult ?? false)
                {
                    new SchemeVersionItemDTO { Id = _model.Id };
                    LoadModel(_model.Id);
                }
            }, Model.Save(), Model.Dto.SchemeVersion.SystemId, true);
            var dialog = new AddSchemaDialog { DataContext = _model };
            dialog.ShowDialog();
        }

        protected override void RefreshCommands()
        {
            base.RefreshCommands();
            if (Model != null)
            {
                Model.PropertyChanged += OnModelPropertyChanged;
            }
            SaveSchemeCommand.RaiseCanExecuteChanged();
            SaveAsSchemeCommand.RaiseCanExecuteChanged();
            ExportSchemeCommand.RaiseCanExecuteChanged();
            RefreshCommand.RaiseCanExecuteChanged();
        }

        private void OnModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(SchemeViewModel.IsChanged))
            {
                SaveSchemeCommand.RaiseCanExecuteChanged();
            }
        }
    }
}