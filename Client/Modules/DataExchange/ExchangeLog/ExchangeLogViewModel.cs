using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Browser;
using System.Windows.Controls;
using GazRouter.Application;
using GazRouter.Application.Helpers;
using GazRouter.Common.ViewModel;
using GazRouter.DataExchange.CustomSource;
using GazRouter.DataProviders.DataExchange;
using GazRouter.DataProviders.ObjectModel;
using GazRouter.DTO.DataExchange.ExchangeEntity;
using GazRouter.DTO.DataExchange.ExchangeLog;
using GazRouter.DTO.DataExchange.ExchangeTask;
using GazRouter.DTO.Dictionaries.EntityTypes;
using GazRouter.DTO.Dictionaries.ExchangeTypes;
using GazRouter.DTO.Dictionaries.PeriodTypes;
using GazRouter.DTO.DispatcherTasks.RecordNotes;
using GazRouter.DTO.ObjectModel.DistrStations;
using GazRouter.DTO.ObjectModel.MeasStations;
using GazRouter.DTO.ObjectModel.Pipelines;
using GazRouter.DTO.ObjectModel.ReducingStations;
using GazRouter.DTO.ObjectModel.Sites;
using GazRouter.DTO.ObjectModel.Valves;
using Telerik.Windows.Controls;
using Utils.Extensions;
using DelegateCommand = Microsoft.Practices.Prism.Commands.DelegateCommand;
using UriBuilder = GazRouter.DataProviders.UriBuilder;


namespace GazRouter.DataExchange.ExchangeLog
{
    public class ExchangeLogViewModel : LockableViewModel
    {

        public DelegateCommand RefreshCommand { get; private set; }
        
        public ExchangeLogViewModel()
        {
            RefreshCommand = new DelegateCommand(Refresh);

            _selectedDate = DateTime.Today;
            
            Refresh();
        }

        private DateTime _selectedDate;
        public DateTime SelectedDate
        {
            get { return _selectedDate; }
            set
            {
                if(SetProperty(ref _selectedDate, value))
                    Refresh();
            }
        }


        public List<ExchangeLogDTO> LogItems { get; set; }


        private async void Refresh()
        {
            Lock();
            LogItems = await new DataExchangeServiceProxy().GetExchangeLogAsync(
                new GetExchangeLogParameterSet
                {
                    StartDate = _selectedDate.Date,
                    EndDate = _selectedDate.Date.AddDays(1)
                });
            OnPropertyChanged(() => LogItems);
            Unlock();
        }
        
        
    }
    
}