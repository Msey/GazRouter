using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using GazRouter.Common;
using GazRouter.Common.Cache;
using GazRouter.Common.Services;
using GazRouter.Common.ViewModel;
using GazRouter.Controls.Dialogs;
using GazRouter.DataProviders.ObjectModel;
using GazRouter.DataProviders.SeriesData;
using GazRouter.DTO.Dictionaries.EntityTypeProperties;
using GazRouter.DTO.Dictionaries.EntityTypes;
using GazRouter.DTO.Dictionaries.PropertyTypes;
using GazRouter.DTO.SeriesData.SerieChecks;
using Microsoft.Practices.Prism.Regions;
using Telerik.Windows.Controls;
using ViewModelBase = GazRouter.Common.ViewModel.ViewModelBase;

namespace GazRouter.ManualInput.Settings.SerieChecks
{
    [RegionMemberLifetime(KeepAlive = true)]
    public class SerieChecksViewModel : LockableViewModel
    {
        public SerieChecksViewModel(bool isReadOnly)
        {
            IsReadOnly = isReadOnly;
            Load();
        }


        public bool IsReadOnly { get; set; }

        /// <summary>
        /// Список проверок
        /// </summary>
        public List<SerieCheck> CheckList { get; set; }

        
        private async void Load()
        {
            Lock();

            // Загрузка проверок
            var checkList = await new SeriesDataServiceProxy().GetSerieCheckListAsync();
            var propTypes = await new ObjectModelServiceProxy().GetEntityTypePropertiesAsync(null);
            CheckList = checkList.Select(c => new SerieCheck(c) { IsEnabled = !IsReadOnly}).ToList();
            OnPropertyChanged(() => CheckList);
            Unlock();
        }

    }
}