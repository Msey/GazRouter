using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.ServiceModel;
using System.Windows.Controls;
using System.Windows.Input;
using Application;
using Common.ViewModel;
using DataProviders;
using DTO.Bindings.EntityBindings;
using DTO.Bindings.PropertyBindings;
using DTO.Dictionaries.Sources;
using DTO.Infrastructure.Faults;
using Microsoft.Practices.Prism.Commands;
using Telerik.Windows.Controls;
using Telerik.Windows.Data;
using DelegateCommand = Microsoft.Practices.Prism.Commands.DelegateCommand;

namespace Modes.Exchange
{
    public class PropertyBindingsViewModel : LockableViewModel
    {
        public PropertyBindingsViewModel()
        {
            SourcesList = new List<SourceDTO>();
			BindingList = new List<BindingModel>();
            RefreshCommand = new DelegateCommand(OnRefreshCommandExecuted, OnRefreshCommandCanExecute);
            KeyEnterCommand = new DelegateCommand<object[]>(KeyEnter);
        }

        public void Refresh()
        {
            LoadSourcesList();
        }

        public DelegateCommand<object[]> KeyEnterCommand { get; protected set; }

        private void KeyEnter(object[] eventArgs)
        {
            if ((eventArgs != null) && (eventArgs.Length > 1))
            {
                object control = eventArgs[0];
                var textBox = control as TextBox;
                if (textBox != null)
                    textBox.GetBindingExpression(TextBox.TextProperty).UpdateSource();

                var keyArgs = eventArgs[1] as KeyEventArgs;
                if (keyArgs != null)
                {
                    switch (keyArgs.Key)
                    {
                        case Key.Enter:
                            RefreshCommands();
                            RefreshData();
                            break;
                    }
                }
            }
        }

        public bool RefreshSources()
        {
            LoadSourcesList();
            return true;
        }

        public void RefreshData()
        {
            SelectedBinding = null;
            if (SelectedSource == null) return;
            new BindingsDataProvider().GetPropertyBindingsList(
                new GetPropertyBindingsParameterSet
                {
                    ShowAll = !ShowBindedOnly,
                    SourceId = SelectedSource.Id
                },
                (dto, exception) =>
                {
                    if (exception == null)
                    {
                        BindingList = dto.Entities.Select(p => new BindingModel(p)).ToList();
                    }
                    return exception == null;
                }, Behavior);
        }

        private void RefreshCommands()
        {
            RefreshCommand.RaiseCanExecuteChanged();
        }

        //private void CreateItems()
        //{
        //    var items = new VirtualQueryableCollectionView<BindingModel> { LoadSize = 30, VirtualItemCount = 100 };
        //    if (BindingList != null)
        //    {
        //        if (BindingList.SortDescriptors.Count > 0)
        //        {
        //            items.SortDescriptors.AddRange(BindingList.SortDescriptors);
        //            BindingList.SortDescriptors.Clear();
        //        }
        //        BindingList.ItemsLoading -= OnItemsLoading;

        //        if (BindingList.FilterDescriptors.Count > 0)
        //        {
        //            items.FilterDescriptors.AddRange(BindingList.FilterDescriptors);
        //            BindingList.FilterDescriptors.Clear();
        //        }
        //        BindingList.ItemsLoading -= OnItemsLoading;
        //    }
        //    items.ItemsLoading += OnItemsLoading;
        //    BindingList = items;
        //}

        //private void OnItemsLoading(object sender, VirtualQueryableCollectionViewItemsLoadingEventArgs args)
        //{
        //    SelectedBinding = null;
        //    var filtername = string.Empty;
        //    if (BindingList.FilterDescriptors.Count > 0)
        //    {
        //        filtername = ((FilterDescriptor)BindingList.FilterDescriptors[0]).Value.ToString();
        //    }

        //        new BindingsDataProvider().GetPropertyBindingsList(
        //            new GetPropertyBindingsPageParameterSet
        //                {
        //                    PageNumber = args.StartIndex/BindingList.PageSize,
        //                    PageSize = 30,
        //                    ShowAll = !ShowBindedOnly,
        //                    NamePart = filtername,
        //                    SourceId = SelectedSource.Id
        //                },
        //            (dto, exception) =>
        //                {
        //                    if (exception == null)
        //                    {
        //                        if (dto.TotalCount != -1 &&
        //                            dto.TotalCount != BindingList.VirtualItemCount)
        //                        {
        //                            BindingList.VirtualItemCount = dto.TotalCount;
        //                        }
        //                        BindingList.ItemsLoaded += BindingListItemsLoaded;
        //                        BindingList.Load(args.StartIndex, dto.Entities.Select(p => new BindingModel(p)));
        //                    }
        //                    return exception == null;
        //                }, Behavior);
        //}

        //private void BindingListItemsLoaded(object sender, VirtualQueryableCollectionViewItemsLoadedEventArgs e)
        //{
        //    BindingList.ItemsLoaded -= BindingListItemsLoaded;
        //    BindingList.Refresh();
        //}

        #region SourcesList

        private List<SourceDTO> _sourcesList;

        public List<SourceDTO> SourcesList
        {
            get { return _sourcesList; }
            set
            {
                _sourcesList = value;
                OnPropertyChanged(() => SourcesList);
            }
        }

        private void LoadSourcesList()
        {
           
            SourcesList = (ClientCache.DictionaryRepository.Sources);
            SelectedSource = SourcesList.FirstOrDefault();
            RefreshData();
        }

        #endregion

        #region SelectedSource

        private SourceDTO _selectedSource;

        public SourceDTO SelectedSource
        {
            get { return _selectedSource; }
            set
            {
                _selectedSource = value;
                OnPropertyChanged(() => SelectedSource);
                RefreshCommands();
                RefreshData();
            }
        }

        #endregion SelectedSource

        #region BindingList

		private List<BindingModel> _bindingList;

		public List<BindingModel> BindingList
        {
            get { return _bindingList; }
            set
            {
                _bindingList = value;
                OnPropertyChanged(() => BindingList);
            }
        }

        #endregion BindingsList

        #region SelectedBinding

		private BindingModel _selectedBinding;

		public BindingModel SelectedBinding
        {
            get { return _selectedBinding; }
            set
            {
				if (_selectedBinding != null)
				{
					_selectedBinding.PropertyChanged -= SelectedBindingOnPropertyChanged;
				}

				_selectedBinding = value;


				if (_selectedBinding != null)
				{
					_selectedBinding.PropertyChanged += SelectedBindingOnPropertyChanged;
				}

                OnPropertyChanged(() => SelectedBinding);
                RefreshCommands();
            }
        }

		private void SelectedBindingOnPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName != "ExtEntityId")
				return;

			var binding = SelectedBinding;
			if (string.IsNullOrEmpty(binding.ExtEntityId))
			{
				if (binding.Model.Id != Guid.Empty)
				{
					new BindingsDataProvider().DeletePropertyBindings(
						binding.Model.Id,
						CallBackAfterRequestOnModifyData, Behavior);
				}

			}
			else
			{
				if (binding.Model.Id == Guid.Empty)
				{
					new BindingsDataProvider().AddPropertyBindings(
						new AddPropertyBindingParameterSet
							{
								SourceId = SelectedSource.Id,
								ExtEntityId = binding.ExtEntityId,
								PropertyId = binding.Model.PropertyId.Value,
							},
						CallBackAfterRequestOnModifyData, Behavior);
				}
				else
				{
					new BindingsDataProvider().EditPropertyBindings(
						new EditPropertyBindingParameterSet
							{
								SourceId = SelectedSource.Id,
								ExtEntityId = binding.ExtEntityId,
								PropertyId = binding.Model.PropertyId.Value,
								Id = binding.Model.Id
							},
						CallBackAfterRequestOnModifyData, Behavior);
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
				RefreshData();
				return true;
			}

			var faultException = ex as FaultException<FaultDetail>;
			if (faultException != null && faultException.Detail.FaultType == FaultType.IntegrityConstraint)
			{
				RadWindow.Alert(faultException.Detail.Message);
				return true;
			}
			return false;
		}

        #endregion SelectedBinding

        #region ShowAll

        private bool _showBindedOnly;

        public bool ShowBindedOnly
        {
            get { return _showBindedOnly; }
            set
            {
                _showBindedOnly = value;
                OnPropertyChanged(() => ShowBindedOnly);
                RefreshCommands();
                RefreshData();
            }
        }

        #endregion

        #region Name

        private string _name;

        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                OnPropertyChanged(() => Name);
            }
        }

        #endregion Name

        #region RefreshCommand

        public DelegateCommand RefreshCommand { get; private set; }

        private void OnRefreshCommandExecuted()
        {
            RefreshData();
        }

        private bool OnRefreshCommandCanExecute()
        {
            return SelectedSource != null;
        }

        #endregion RefreshCommand
    }

	public class BindingModel : PropertyChangedBase
	{
		private string _extEntityId;

		public BindingModel(BindingDTO model)
		{
			Model = model;
			_extEntityId = model.ExtEntityId;
		}

		public BindingDTO Model { get; private set; }

		public string ExtEntityId
		{
			get { return _extEntityId; }
			set
			{
				_extEntityId = value;
				OnPropertyChanged(() => ExtEntityId);
			}
		}


	}
}