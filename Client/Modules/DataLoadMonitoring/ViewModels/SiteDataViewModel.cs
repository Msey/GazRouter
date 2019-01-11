using System;
using System.Collections.Generic;
using GazRouter.Common.ViewModel;
using GazRouter.DataProviders.DataLoadMonitoring;
using GazRouter.DTO.DataLoadMonitoring;
using GazRouter.DTO.ObjectModel.Sites;
using GazRouter.DTO.SeriesData.Series;

namespace GazRouter.DataLoadMonitoring.ViewModels
{
    public class SiteDataViewModel : LockableViewModel
    {
        private readonly DataLoadMonitoringDataProvider _techDataProvider = new DataLoadMonitoringDataProvider();

        private List<BaseEntityProperty> _propertyValueList;
        public List<BaseEntityProperty> PropertyValueList
        {
            get
            {
                return _propertyValueList;
            }
            set
            {
                _propertyValueList = value;
                OnPropertyChanged(() => PropertyValueList);
            }
        }

        public string Sitename { get; set; }
        public string Caption { get; set; }

        public SiteDataViewModel(SiteDTO site, SeriesDTO serie)
        {
            Sitename = site.Name;
            Caption = "Данные по " + site.Name + " за "+ serie.KeyDate;
            _techDataProvider.GetTechDataBySite(new EntityPropertyValueParameterSet{DataSeries = serie,Site = site},Callback,Behavior );
        }

        private bool Callback(List<BaseEntityProperty> propertyValueList, Exception exception)
        {
            if (exception != null)
            {
                return false;
            }

            PropertyValueList = propertyValueList;
            return true;
        }

    }
}
