using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Browser;
using GazRouter.Common.ViewModel;
using GazRouter.Controls.Attachment;
using GazRouter.DataProviders.DataExchange;
using GazRouter.DataProviders.ObjectModel;
using GazRouter.DTO.Attachments;
using GazRouter.DTO.DataExchange.DataSource;
using GazRouter.DTO.DataExchange.ExchangeEntity;
using GazRouter.DTO.DataExchange.ExchangeTask;
using Microsoft.Practices.Prism.Commands;


namespace GazRouter.Controls.Dialogs.ObjectDetails.Bindings
{
    public class BindingsViewModel : ValidationViewModel
    {
        private Guid? _entityId;
        private bool _isActive;
        private bool _isReadOnly;
        
        public BindingsViewModel()
        {
            RefreshCommand = new DelegateCommand(Refresh);
            Refresh();
        }

        /// <summary>
        /// »дентификатор объекта, дл€ которого отображаютс€ прикрепленные документы
        /// </summary>
        public Guid? EntityId
        {
            get { return _entityId;}
            set
            {
                if(SetProperty(ref _entityId, value))
                {
                    Refresh();
                }
            }
        }

        /// <summary>
        /// ≈сли установлен в True, то при каждом изменении EntityId обновл€етс€ список вложений
        /// Ёто сделано дл€ того, чтобы грузить данные только в том случае когда вкладка с вложени€ми активна
        /// </summary>
        public bool IsActive
        {
            get { return _isActive; }
            set
            {
                if (SetProperty(ref _isActive, value))
                {
                    Refresh();
                }
            }
        }

        public bool IsReadOnly
        {
            get { return _isReadOnly; }
            set { SetProperty(ref _isReadOnly, value); }
        }

        public List<DataSourceDTO> SourceList { get; set; }

        private DataSourceDTO _selectedSource;
        public DataSourceDTO SelectedSource
        {
            get { return _selectedSource; }
            set
            {
                if (SetProperty(ref _selectedSource, value))
                {
                    OnPropertyChanged(() => BindingList);
                }
            }
        }

        private bool _hasExtId;

        public bool HasExtId
        {
            get { return _hasExtId; }
            set
            {
                if (SetProperty(ref _hasExtId, value))
                {
                    OnPropertyChanged(() => BindingList);
                }
            }
        }

        private List<BindingItem> _bindings;

        public List<BindingItem> BindingList
            =>
                _bindings?.Where(b => _selectedSource == null || b.TaskDto.DataSourceId == _selectedSource.Id)
                    .Where(b => !HasExtId || !string.IsNullOrEmpty(b.ExtId))
                    .ToList();

        
        
        

        public DelegateCommand RefreshCommand { get; set; }


        private async void Refresh()
        {
            if (!_isActive || !EntityId.HasValue) return;

            Lock();

            if (SourceList == null)
            {
                SourceList = await new DataExchangeServiceProxy().GetDataSourceListAsync(
                    new GetDataSourceListParameterSet
                    {
                        GetHidden = true,
                        GetReadonly = true
                    });
                OnPropertyChanged(() => SourceList);
            }
            var taskList = await new DataExchangeServiceProxy().GetExchangeTaskListAsync(null);
            var bindings =
                await
                    new DataExchangeServiceProxy().GetExchangeEntityListAsync(
                        new GetExchangeEntityListParameterSet
                        {
                            EntityId = EntityId
                        });

            _bindings = new List<BindingItem>();
            foreach (var task in taskList)
            {
                _bindings.Add(new BindingItem(task,
                    bindings.SingleOrDefault(b => b.EntityId == EntityId.Value && b.ExchangeTaskId == task.Id)));
            }
            OnPropertyChanged(() => BindingList);

            Unlock();
        }
    }

    public class BindingItem
    {
        public BindingItem(ExchangeTaskDTO taskDto, ExchangeEntityDTO bindingDto)
        {
            TaskDto = taskDto;

            if (bindingDto != null)
            {
                ExtId = bindingDto.ExtId;
                IsActive = bindingDto.IsActive;
            }
        }

        public ExchangeTaskDTO TaskDto { get; set; }

        public string ExtId { get; set; }

        public bool IsActive { get; set; }
    }
}