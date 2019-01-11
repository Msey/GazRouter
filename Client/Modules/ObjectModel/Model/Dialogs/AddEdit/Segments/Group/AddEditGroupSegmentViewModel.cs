using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GazRouter.Application;
using GazRouter.DataProviders.ObjectModel;
using GazRouter.DTO.Dictionaries.PipelineGroups;
using GazRouter.DTO.ObjectModel.Pipelines;
using GazRouter.DTO.ObjectModel.Segment;

namespace GazRouter.ObjectModel.Model.Dialogs.AddEdit.Segments.Group
{
    public class AddEditGroupSegmentViewModel : AddEditSegmentViewModelBase<GroupSegmentDTO>
    {
        #region Constr

        public AddEditGroupSegmentViewModel(Action<int> actionBeforeClosing, GroupSegmentDTO model, PipelineDTO pipeline)
            : base(actionBeforeClosing, model, pipeline)
        {
	        Id = model.Id;
			KilometerOfEndPoint = model.KilometerOfEndPoint;
			KilometerOfStartPoint = model.KilometerOfStartPoint;
            LoadPipelineGroupList();
            SelectedPipelineGroup = PipelineGroupList.FirstOrDefault(p => p.Id == model.PipelineGroupId);

            SetValidationRules();
        }

        public AddEditGroupSegmentViewModel(Action<int> actionBeforeClosing, PipelineDTO pipeline)
            : base(actionBeforeClosing, pipeline)
        {
            LoadPipelineGroupList();
            SetValidationRules();
        }

        #endregion

        #region PipelineGroupId

        public List<PipelineGroupDTO> PipelineGroupList { get; set; }

        private void LoadPipelineGroupList()
        {
            PipelineGroupList = ClientCache.DictionaryRepository.PipelineGroups;
            OnPropertyChanged(() => PipelineGroupList);
        }

        private PipelineGroupDTO _selectedPipelineGroup;

        public PipelineGroupDTO SelectedPipelineGroup
        {
            get { return _selectedPipelineGroup; }
            set
            {
                if (SetProperty(ref _selectedPipelineGroup, value))
                    SaveCommand.RaiseCanExecuteChanged();
            }
        }

        #endregion

        #region SaveCommand
        
        protected async override void LoadSegmentList()
        {
            var list = await new ObjectModelServiceProxy().GetGroupSegmentListAsync(Pipeline.Id);
            GetSegmentListCallback(list);
        }

        protected override Task<int> CreateTask
        {
            get
            {
                return new ObjectModelServiceProxy()
                       .AddGroupSegmentAsync(new AddGroupSegmentParameterSet
                       {
                           PipelineId = Pipeline.Id,
                           KilometerOfStartPoint = KilometerOfStartPoint.Value,
                           KilometerOfEndPoint = KilometerOfEndPoint.Value,
                           PipelineGroupId = SelectedPipelineGroup.Id
                       });
            }
        }

        protected override Task UpdateTask
        {
            get
            {
                ValidateAll();
                if (!HasErrors)
                {
                    return new ObjectModelServiceProxy()
                            .EditGroupSegmentAsync(
                            new EditGroupSegmentParameterSet
                            {
                                SegmentId = Model.Id,
                                PipelineId = Pipeline.Id,
                                KilometerOfStartPoint = KilometerOfStartPoint.Value,
                                KilometerOfEndPoint = KilometerOfEndPoint.Value,
                                PipelineGroupId = SelectedPipelineGroup.Id
                            });
                }
                return null;
            }
        }

        #endregion SaveCommand

        protected override void SetValidationRules()
        {
            AddValidationFor(() => SelectedPipelineGroup)
                .When(() => SelectedPipelineGroup == null)
                .Show("Не выбрана группа газопроводов");

            base.SetValidationRules();

        }
    }
}