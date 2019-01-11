using System;
using System.Collections.Generic;
using System.Linq;
using GazRouter.DataProviders.ObjectModel;
using GazRouter.DTO.ObjectModel.Pipelines;
using GazRouter.DTO.ObjectModel.Segment;
using GazRouter.ObjectModel.Model.Dialogs;

namespace GazRouter.ObjectModel.Model.Tabs.Segments.Group
{
    public class GroupSegmentViewModel : EditableDtoListViewModel<GroupSegmentDTO, int>
    {
        public override string Header
        {
            get { return "Сегменты по группам газопроводов"; }
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

        protected override bool GetListCallback(IEnumerable<GroupSegmentDTO> dtos, Exception ex)
        {
            var result = base.GetListCallback(dtos, ex);

            OnPropertyChanged(() => SegmentList);

            return result;
        }


        protected override void Add()
        {
            DialogHelper.AddGroupSegment(id => Refresh(), (PipelineDTO)ParentEntity);
        }

        protected override void Edit()
        {
            DialogHelper.EditGroupSegment(id => Refresh(), SelectedItem, (PipelineDTO)ParentEntity);
        }

        protected async override void Delete()
        {
            try
            {
                Behavior.TryLock();
                await new ObjectModelServiceProxy().DeleteGroupSegmentAsync(SelectedItem.Id);
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
                var list = await new ObjectModelServiceProxy().GetGroupSegmentListAsync(Pipeline.Id);
                GetListCallback(list, null);
            }
            finally 
            {
                Behavior.TryUnlock();
            }
        }
    }
}