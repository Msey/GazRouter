using System;
using System.Collections.Generic;
using System.Linq;
using GazRouter.DataProviders.ObjectModel;
using GazRouter.DTO.ObjectModel.Pipelines;
using GazRouter.DTO.ObjectModel.Segment;
using GazRouter.ObjectModel.Model.Dialogs;

namespace GazRouter.ObjectModel.Model.Tabs.Segments.Diameter
{
	public class DiameterSegmentViewModel : EditableDtoListViewModel<DiameterSegmentDTO, int>
    {
        public override string Header
        {
            get { return "Сегменты по диаметру"; }
        }


        /// <summary>
        /// Идентификатор газопровода
        /// </summary>
	    public PipelineDTO Pipeline
	    {
	        get { return (PipelineDTO) ParentEntity; }
	    }

        /// <summary>
        /// Список сегментов
        /// </summary>
	    public List<BaseSegmentDTO> SegmentList
	    {
	        get { return Items.Select(s => (BaseSegmentDTO)s).ToList(); }
	    }

	    protected override bool GetListCallback(IEnumerable<DiameterSegmentDTO> dtos, Exception ex)
	    {
	        var result = base.GetListCallback(dtos, ex);
            OnPropertyChanged(() => SegmentList);
            return result;
	    }


	    protected override void Add()
        {
			DialogHelper.AddPipeDiameterSegment(id => Refresh(), (PipelineDTO)ParentEntity);
        }

        protected override void Edit()
        {
            DialogHelper.EditPipeDiameterSegment(id => Refresh(), SelectedItem, (PipelineDTO)ParentEntity);
        }

        protected async override void Delete()
        {
			await new ObjectModelServiceProxy().DeleteDiameterSegmentAsync(SelectedItem.Id);
            DeleteCallback(null);
        }

        protected async override void GetList()
        {
            try
            {
                Behavior.TryLock();
                var list = await new ObjectModelServiceProxy().GetDiameterSegmentListAsync(ParentEntity.Id);
                GetListCallback(list, null);
            }
            finally 
            {
                Behavior.TryUnlock();
            }
            
        }
        
	    
    }
}