using System;
using System.Collections.Generic;
using System.Linq;
using GazRouter.Common.ViewModel;
using GazRouter.DataProviders.Calculations;
using GazRouter.DataProviders.DataExchange;
using GazRouter.DTO.Calculations.Calculation;
using GazRouter.DTO.DataExchange.DataSource;
using GazRouter.DTO.DataExchange.ExchangeEntity;
using GazRouter.DTO.DataExchange.ExchangeTask;
using Microsoft.Practices.Prism.Commands;


namespace GazRouter.Controls.Dialogs.ObjectDetails.Calculations
{
    public class CalculationsViewModel : ValidationViewModel
    {
        private Guid? _entityId;
        private bool _isActive;
        private bool _isReadOnly;
        
        public CalculationsViewModel()
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

        
        public List<CalculationDTO> CalculationList { get; set; }
        
        

        public DelegateCommand RefreshCommand { get; set; }


        private async void Refresh()
        {
            if (!_isActive || !EntityId.HasValue) return;

            Lock();

            CalculationList = await new CalculationServiceProxy().GetCalculationListAsync(
                new GetCalculationListParameterSet
                {
                    EntityId = EntityId
                });

            OnPropertyChanged(() => CalculationList);

            Unlock();
        }
    }
}