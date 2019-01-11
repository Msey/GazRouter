using System.Collections.ObjectModel;
using System.Windows;
using GazRouter.Common.ViewModel;
using GazRouter.DTO.DispatcherTasks.TaskStatuses;
using Microsoft.Practices.Prism.Commands;

namespace GazRouter.Modes.DispatcherTasks.PDS
{
	public abstract class TaskDetailsViewModelBase<TDto> : LockableViewModel, ITaskDetailsTab where TDto : class
	{
		protected TaskDetailsViewModelBase()
		{
			_items = new ObservableCollection<TDto>();
		}

		protected abstract void Add();

        protected abstract void Edit();


        private ObservableCollection<TDto> _items;
		public ObservableCollection<TDto> Items
		{
			get { return _items; }
			set
			{
				_items = value;
				OnPropertyChanged(() => Items);
			}
		}

		private TaskStatusDTO _status;
		public TaskStatusDTO Status
		{
			get { return _status; }
			set
			{
				_status = value;
				OnPropertyChanged(() => Status);
				OnPropertyChanged(() => IsTabVisible);
                RaiseCommands();
				if (_status != null) Refresh();
			}
		}


		public abstract Visibility IsTabVisible { get; }

		protected abstract void RaiseCommands();

		public DelegateCommand AddCommand { get; protected set; }

        public DelegateCommand EditCommand { get; protected set; }

        public DelegateCommand DeleteCommand { get; protected set; }

        public abstract void Refresh();

		public abstract string Header { get; }
	}
}