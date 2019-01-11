using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GazRouter.DataProviders.ObjectModel;
using GazRouter.DTO.ObjectModel.Pipelines;
using GazRouter.DTO.ObjectModel.Segment;
using GazRouter.DTO.ObjectModel.Sites;

namespace GazRouter.ObjectModel.Model.Dialogs.AddEdit.Segments.Site
{
    public class AddEditSiteSegmentViewModel : AddEditSegmentViewModelBase<SiteSegmentDTO>
    {
        public AddEditSiteSegmentViewModel(Action<int> actionBeforeClosing, SiteSegmentDTO model, PipelineDTO pipeline)
            : base(actionBeforeClosing, model, pipeline)
        {
	        Name = model.SiteName;
			KilometerOfEndPoint= model.KilometerOfEndPoint;
			KilometerOfStartPoint = model.KilometerOfStartPoint;
	        Id = model.Id;
            SiteId = model.SiteId;
            LoadSite();
            SetValidationRules();
        }

        public AddEditSiteSegmentViewModel(Action<int> actionBeforeClosing, PipelineDTO pipeline)
            : base(actionBeforeClosing, pipeline)
        {
            LoadSite();
            SetValidationRules();
        }

        private async void LoadSite()
        {
            ListSite = await new ObjectModelServiceProxy().GetSiteListAsync(null);
            OnPropertyChanged(() => ListSite);
            SelectedSite = ListSite.FirstOrDefault(p => Equals(p.Id, SiteId));
                                                           
                                                       
        }
        
        protected async override void LoadSegmentList()
        {
            var list = await new ObjectModelServiceProxy().GetSiteSegmentListAsync(Pipeline.Id);
            GetSegmentListCallback(list);
        }

        protected override Task<int> CreateTask
        {
            get
            {
                return new ObjectModelServiceProxy()
                        .AddSiteSegmentAsync(new AddSiteSegmentParameterSet
                        {
                            SiteId = SiteId,
                            PipelineId = Pipeline.Id,
                            KilometerOfStartPoint = KilometerOfStartPoint.Value,
                            KilometerOfEndPoint = KilometerOfEndPoint.Value
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
                            .EditSiteSegmentAsync(
                                new EditSiteSegmentParameterSet
                                {
                                    SegmentId = Model.Id,
                                    SiteId = SiteId,
                                    PipelineId = Pipeline.Id,
                                    KilometerOfStartPoint = KilometerOfStartPoint.Value,
                                    KilometerOfEndPoint = KilometerOfEndPoint.Value
                                });
                }
                return null;
            }
        }

        #region ListSite

        public List<SiteDTO> ListSite { get; set; }

        #endregion

        #region SelectedSite

        private SiteDTO _selectedSite;

        public SiteDTO SelectedSite
        {
            get { return _selectedSite; }
            set
            {
                if (_selectedSite == value) return;
                _selectedSite = value;
                SiteId = _selectedSite.Id;
                SaveCommand.RaiseCanExecuteChanged();
                OnPropertyChanged(() => SelectedSite);
            }
        }

        protected Guid SiteId { get; set; }

        #endregion

        protected override void SetValidationRules()
        {
            AddValidationFor(() => SelectedSite)
                .When(() => SelectedSite == null)
                .Show("Укажите ЛПУ");
            
            base.SetValidationRules();

        }
    }
}