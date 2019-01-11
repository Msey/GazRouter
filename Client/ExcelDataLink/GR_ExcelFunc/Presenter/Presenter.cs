using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GazRouter.DTO.Dashboards.DashboardGrants;
using GazRouter.DTO.Dictionaries.EntityTypes;
using GazRouter.DTO.ObjectModel.Sites;
using GR_ExcelFunc.Model;
using GR_ExcelFunc.View;

namespace GR_ExcelFunc.Presenter
{
    public class SelectObjectParameterPresenter
    {
        private readonly ISelectObjectParameterView _view;
        private readonly ISelectObjectParameterData _data;

        public SelectObjectParameterPresenter(ISelectObjectParameterView view, ISelectObjectParameterData data)
        {
            _view = view;
            view.Presenter = this;
            _data = data;

            Update();
        }

        void Update()
        {
            //_view.EntityTypeList = (from et in _data.GetSiteList() select et.Name).ToList();
            _view.SiteList = _data.GetSiteList().OrderBy(el => el.Name).ToList();
            _view.EntityTypeList = _data.GetEntityTypeList().OrderBy(el => el.Name).ToList();
            _view.PeriodTypeList = _data.GetPeriodTypeList().OrderBy(e => e.Name).ToList();

        }
        public void UpdateEntityList()
        {
            _view.EntityList = _data.GetEntityList().ToList();
        }
        public void UpdateEntityList(string searchText, EntityType? entityType)
        {
            _view.EntityList = _data.GetEntityListBySearch(searchText, entityType).ToList();
        }
        public void UpdateEntityList(EntityType entityType)
        {
            _view.EntityList = _data.GetEntityListByEntityType(entityType).ToList();
        }
        public void UpdateEntityList(SiteDTO site)
        {
            _view.EntityList = _data.GetEntityListBySite(site).ToList();
        }
        public void UpdatePropertyList(EntityType entityType)
        {
            _view.PropertyTypeList = _data.GetPropertyTypeListByEntityType(entityType).OrderBy(e=>e.Name).ToList();
        }

    }
}
