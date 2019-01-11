using System;
using System.Collections.Generic;
using GazRouter.DTO.Dictionaries.EntityTypes;
using GazRouter.DTO.Dictionaries.PeriodTypes;
using GazRouter.DTO.Dictionaries.PropertyTypes;
using GazRouter.DTO.ObjectModel;
using GazRouter.DTO.ObjectModel.Sites;
using GR_ExcelFunc.Presenter;

namespace GR_ExcelFunc.View
{
    public interface ISelectObjectParameterView
    {
        IList<SiteDTO> SiteList { get; set; }
        IList<EntityTypeDTO> EntityTypeList { get; set; }
        IList<CommonEntityDTO> EntityList { get; set; }
        CommonEntityDTO SelectedEntity { get;  }
        IList<PropertyTypeDTO> PropertyTypeList { get; set; }
        IList<PeriodTypeDTO> PeriodTypeList { get; set; }

        DateTime StartDate { get; set; }
        DateTime EndDate { get; set; }

        SelectObjectParameterPresenter Presenter { set; } 
    }
}
