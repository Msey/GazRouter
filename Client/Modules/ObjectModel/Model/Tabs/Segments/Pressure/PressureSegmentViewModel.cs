using System.Collections.Generic;
using System.Linq;
using GazRouter.DataProviders.ObjectModel;
using GazRouter.DTO.ObjectModel.Pipelines;
using GazRouter.DTO.ObjectModel.Segment;
using GazRouter.ObjectModel.Model.Dialogs;

namespace GazRouter.ObjectModel.Model.Tabs.Segments.Pressure
{
    public class PressureSegmentViewModel : EditableDtoListViewModel<PressureSegmentDTO, int>
    {
        public override string Header
        {
            get { return "Сегменты по давлению"; }
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
            DialogHelper.AddPressureSegment(id => Refresh(), (PipelineDTO)ParentEntity);
        }

        protected override void Edit()
        {
            DialogHelper.EditPressureSegment(id => Refresh(), SelectedItem, (PipelineDTO)ParentEntity);
        }

        protected async override void Delete()
        {
            try
            {
                Behavior.TryLock();
                await new ObjectModelServiceProxy().DeletePressureSegmentAsync(SelectedItem.Id);
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
                var list = await new ObjectModelServiceProxy().GetPressureSegmentListAsync(ParentEntity.Id);
                GetListCallback(list, null);
                OnPropertyChanged(() => SegmentList);
            }
            finally 
            {
                Behavior.TryUnlock();
            }
            
        }
    }
}