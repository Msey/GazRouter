using System.Collections.Generic;
using System.Linq;
using GazRouter.DataProviders.ObjectModel;
using GazRouter.DTO.ObjectModel.Pipelines;
using GazRouter.DTO.ObjectModel.Segment;
using GazRouter.ObjectModel.Model.Dialogs;
using GazRouter.DTO.Dictionaries.Regions;

namespace GazRouter.ObjectModel.Model.Tabs.Segments.Regions
{
    public class RegionSegmentViewModel : EditableDtoListViewModel<RegionSegmentDTO, int>
    {
        public override string Header
        {
            get { return "Сегменты по регионам"; }
        }


        /// <summary>
        /// Идентификатор газопровода
        /// </summary>
        public PipelineDTO Pipeline
        {
            get { return (PipelineDTO)ParentEntity; }
        }

        /// <summary>
        /// Список сегментов
        /// </summary>
        public List<BaseSegmentDTO> SegmentList
        {
            get { return Items.Select(s => (BaseSegmentDTO)s).ToList(); }
        }

       

        protected override void Add()
        {
            DialogHelper.AddRegionSegment(id => Refresh(), (PipelineDTO)ParentEntity);
        }
        
        protected override void Edit()
        {
            DialogHelper.EditRegionSegment(id => Refresh(), SelectedItem, (PipelineDTO)ParentEntity);
        }

        protected async override void Delete()
        {
            try
            {
                Behavior.TryLock();
                await new ObjectModelServiceProxy().DeleteRegionSegmentAsync(SelectedItem.Id);
                DeleteCallback(null);
            }
            finally
            {
                Behavior.TryUnlock();
            }

        }

        protected async override void GetList()
        {
            try
            {
                Behavior.TryLock();
                var list = await new ObjectModelServiceProxy().GetRegionSegmentListAsync(ParentEntity.Id);
                GetListCallback(list, null);

                foreach (RegionSegmentDTO s in SegmentList)
                {
                    s.RegionName = ClientCache.DictionaryRepository.Regions.FirstOrDefault(r => r.Id == s.RegionID).Name;
                }

                OnPropertyChanged(() => SegmentList);
            }
            finally
            {
                Behavior.TryUnlock();
            }
        }
        
    }
}
