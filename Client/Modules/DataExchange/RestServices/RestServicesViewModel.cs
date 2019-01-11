using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Browser;
using System.Windows.Media.Imaging;
using System.Xml.Linq;
using GazRouter.Common.ViewModel;
using GazRouter.DataExchange.CustomSource;
using GazRouter.DataProviders.Balances;
using GazRouter.DataProviders.ObjectModel;
using GazRouter.DTO;
using GazRouter.DTO.Balances.ConsumerContracts;
using GazRouter.DTO.Balances.Contracts;
using GazRouter.DTO.Dictionaries.EntityTypes;
using GazRouter.DTO.Dictionaries.PeriodTypes;
using GazRouter.DTO.Dictionaries.Targets;
using GazRouter.DTO.ObjectModel;
using GazRouter.DTO.ObjectModel.Consumers;
using GazRouter.DTO.ObjectModel.DistrStations;
using GazRouter.DTO.ObjectModel.Sites;
using Microsoft.Practices.Prism.Commands;
using UriBuilder = GazRouter.DataProviders.UriBuilder;

namespace DataExchange.RestServices
{
    public enum RestExports
    {
        Consumers
    }
    public class RestServicesViewModel : LockableViewModel
    {
        public DelegateCommand SaveCommand { get; set; }
        public DelegateCommand SaveAsCommand { get; set; }

        public RestServicesViewModel()
        {
            _exportList = new List<string> {"Экспорт потребителей"};
            SaveCommand = new DelegateCommand(Save, () => SelectedDate.HasValue );
            SaveAsCommand = new DelegateCommand(Save, () => SelectedDate.HasValue);

        }

        private void Save()
        {
            //HtmlPage.Window.Navigate(
            //    UriBuilder.GetSpecificExchangeHandlerUri(task.Id, SelectedTimeStamp.Value, xmlOnly: !task.IsTransform));

        }

        private async void Load()
        {
            try
            {
                Behavior.TryLock();

                var contract = await new BalancesServiceProxy().GetContractAsync(
                    new GetContractListParameterSet
                    {
                        ContractDate = SelectedDate.GetValueOrDefault(),
//                        GasTransportSystemId = SelectedSystem.Id,
                        PeriodTypeId = PeriodType.Month,
                        TargetId = Target.Plan
                    });

                if (contract == null) return;


                var consumers = await new ObjectModelServiceProxy().GetConsumerListAsync(
                    new GetConsumerListParameterSet
                    {
                    });
                // Загрузить список ЛПУ
                var sites = await new ObjectModelServiceProxy().GetSiteListAsync(
                    new GetSiteListParameterSet
                    {
                    });
                // Загрузить список ГРС
                var distrStations = await new ObjectModelServiceProxy().GetDistrStationTreeAsync(
                    new GetDistrStationListParameterSet
                    {
                    });
                

                var values = await new BalancesServiceProxy().GetBalanceValuesAsync(contract.Id);
                var consumerValues = values.Where(v => v.EntityType == EntityType.Consumer).ToList();

                foreach (var site in sites)
                {
                    var xcontr = XElement.Parse("<contract/>");
                    //var xsite = new XElement();
                    foreach (var station in distrStations.DistrStations.Where(m => m.ParentId == site.Id))
                    {
                        foreach (var consumer in consumers.Where(c => c.DistrStationId == station.Id))
                        {
                            var vals = consumerValues.Where(v => v.EntityId == consumer.Id).ToList();

                            

                        }


                    }

                }

            }
            finally
            {
                Behavior.TryUnlock();
            }
        }

        private List<string> _exportList;

        public List<string> ExportList
        {
            get { return _exportList; }
            set
            {
                SetProperty(ref _exportList, value);
            }
        }

        public string SelectedItem { get; set; }

        private DateTime? _selectedDate;
        public DateTime? SelectedDate
        {
            get { return _selectedDate; }
            set { SetProperty(ref _selectedDate, value); }
        }
    }
}