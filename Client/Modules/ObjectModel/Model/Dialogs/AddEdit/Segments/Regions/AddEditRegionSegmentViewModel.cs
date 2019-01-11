using GazRouter.DataProviders.ObjectModel;
using GazRouter.DTO.Dictionaries.Regions;
using GazRouter.DTO.ObjectModel.Pipelines;
using GazRouter.DTO.ObjectModel.Segment;
using System;
using System.Collections.Generic;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Linq;
using System.Threading.Tasks;

namespace GazRouter.ObjectModel.Model.Dialogs.AddEdit.Segments.Regions
{
    public class AddEditRegionSegmentViewModel : AddEditSegmentViewModelBase<RegionSegmentDTO>
    {
        private int _regionID;
        

        private RegionDTO _selectedRegion;
        public RegionDTO SelectedRegion
        {
            get { return _selectedRegion; }
            set
            {
                if (_selectedRegion == value) return;
                _selectedRegion = value;
                _regionID = _selectedRegion.Id;
                SaveCommand.RaiseCanExecuteChanged();
                OnPropertyChanged(() => SelectedRegion);
            }
        }

        public List<RegionDTO> RegionsList => ClientCache.DictionaryRepository.Regions;

        public AddEditRegionSegmentViewModel(Action<int> actionBeforeClosing, RegionSegmentDTO model, PipelineDTO pipeline)
            : base(actionBeforeClosing, model, pipeline)
        {
            KilometerOfEndPoint = model.KilometerOfEndPoint;
            KilometerOfStartPoint = model.KilometerOfStartPoint;
            Id = model.Id;
            _regionID = model.RegionID;
            SelectedRegion = RegionsList.First(p => p.Id == _regionID);
            SetValidationRules();
        }

        public AddEditRegionSegmentViewModel(Action<int> actionBeforeClosing, PipelineDTO pipeline)
            : base(actionBeforeClosing, pipeline)
        {
            
            SetValidationRules();
        }

        protected async override void LoadSegmentList()
        {
            var list = await new ObjectModelServiceProxy().GetRegionSegmentListAsync(Pipeline.Id);
            GetSegmentListCallback(list);
        }

        protected override Task<int> CreateTask
        {
            get
            {
                return new ObjectModelServiceProxy()
                        .AddRegionSegmentAsync(new AddRegionSegmentParameterSet
                        {
                            RegionId = _regionID,
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
                            .EditRegionSegmentAsync(
                                new EditRegionSegmentParameterSet
                                {
                                    SegmentId = Model.Id,
                                    RegionId = _regionID,
                                    PipelineId = Pipeline.Id,
                                    KilometerOfStartPoint = KilometerOfStartPoint.Value,
                                    KilometerOfEndPoint = KilometerOfEndPoint.Value
                                });
                }
                return null;
            }
        }

        

        protected override void SetValidationRules()
        {
            AddValidationFor(() => SelectedRegion)
                .When(() => SelectedRegion == null)
                .Show("Укажите регион");

            base.SetValidationRules();

        }


    }
}
