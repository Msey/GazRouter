using System.Collections.Generic;
using System.Linq;
using GazRouter.Application;
using GazRouter.Common.ViewModel;
using GazRouter.DataProviders.DataExchange;
using GazRouter.DTO.DataExchange.DataSource;
using Microsoft.Practices.Prism.Regions;

namespace GazRouter.Modes.Exchange
{
    [RegionMemberLifetime(KeepAlive = false)]
    public class ExchangesViewModel : MainViewModelBase
    {
        public ExchangesViewModel()
        {
            MappingViewModel = new MappingViewModel();
            TypicalExchangeViewModel = new TypicalExchangeViewModel();
            ExchangeSettingsViewModel = new ExchangeSettingsViewModel();
            SourceViewModel = new SourceViewModel(RefreshSources);
            SharedSourceList =
                ClientCache.DictionaryRepository.Sources.Select(
                    s =>
                        new DataSourceDTO()
                        {
                            Description = s.Description,
                            Name = s.Name,
                            Id = s.Id,
                            SysName = s.SysName,
                            IsReadonly = s.IsReadonly,
                            IsHidden = s.IsHidden
                        }).ToList();

            TimerSettingsViewModel = new TimerSettingsViewModel();
        }



        public ExchangeSettingsViewModel ExchangeSettingsViewModel { get; set; }
        public MappingViewModel MappingViewModel { get; private set; }
        public TypicalExchangeViewModel TypicalExchangeViewModel { get; private set; }
        public SourceViewModel SourceViewModel{get; set;}
        public TimerSettingsViewModel TimerSettingsViewModel { get; set; }

        public void Refresh()
        {
            MappingViewModel.Refresh();
            TypicalExchangeViewModel.Refresh();
            ExchangeSettingsViewModel.Refresh();
            SourceViewModel.Refresh();
            TimerSettingsViewModel.Refresh();
        }

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            Refresh();
        }

        public static List<DataSourceDTO> SharedSourceList
        {
            get; set;
        }

        public async void RefreshSources()
        {
            var result = await new DataExchangeServiceProxy().GetDataSourceListAsync(new GetDataSourceListParameterSet{});
            SharedSourceList.Clear();
            SharedSourceList.AddRange(result);
        }
    }
}