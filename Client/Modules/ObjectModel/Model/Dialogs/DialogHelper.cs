using System;
using GazRouter.DTO.Dictionaries.EngineClasses;
using GazRouter.DTO.ObjectModel;
using GazRouter.DTO.ObjectModel.BoilerPlants;
using GazRouter.DTO.ObjectModel.Boilers;
using GazRouter.DTO.ObjectModel.CompShops;
using GazRouter.DTO.ObjectModel.CompStations;
using GazRouter.DTO.ObjectModel.CompUnits;
using GazRouter.DTO.ObjectModel.Consumers;
using GazRouter.DTO.ObjectModel.CoolingStations;
using GazRouter.DTO.ObjectModel.CoolingUnit;
using GazRouter.DTO.ObjectModel.DistrStationOutlets;
using GazRouter.DTO.ObjectModel.DistrStations;
using GazRouter.DTO.ObjectModel.MeasLine;
using GazRouter.DTO.ObjectModel.MeasPoint;
using GazRouter.DTO.ObjectModel.MeasStations;
using GazRouter.DTO.ObjectModel.OperConsumers;
using GazRouter.DTO.ObjectModel.Pipelines;
using GazRouter.DTO.ObjectModel.PowerPlants;
using GazRouter.DTO.ObjectModel.PowerUnits;
using GazRouter.DTO.ObjectModel.ReducingStations;
using GazRouter.DTO.ObjectModel.Segment;
using GazRouter.DTO.ObjectModel.Sites;
using GazRouter.DTO.ObjectModel.Valves;
using GazRouter.ObjectModel.Model.Dialogs.AddEdit.Objects.Boiler;
using GazRouter.ObjectModel.Model.Dialogs.AddEdit.Objects.BoilerPlant;
using GazRouter.ObjectModel.Model.Dialogs.AddEdit.Objects.CompShop;
using GazRouter.ObjectModel.Model.Dialogs.AddEdit.Objects.CompStation;
using GazRouter.ObjectModel.Model.Dialogs.AddEdit.Objects.CompUnit;
using GazRouter.ObjectModel.Model.Dialogs.AddEdit.Objects.Consumer;
using GazRouter.ObjectModel.Model.Dialogs.AddEdit.Objects.CoolingStation;
using GazRouter.ObjectModel.Model.Dialogs.AddEdit.Objects.CoolingUnit;
using GazRouter.ObjectModel.Model.Dialogs.AddEdit.Objects.DistrStation;
using GazRouter.ObjectModel.Model.Dialogs.AddEdit.Objects.DistrStationOutlet;
using GazRouter.ObjectModel.Model.Dialogs.AddEdit.Objects.MeasLine;
using GazRouter.ObjectModel.Model.Dialogs.AddEdit.Objects.MeasPoint;
using GazRouter.ObjectModel.Model.Dialogs.AddEdit.Objects.MeasStation;
using GazRouter.ObjectModel.Model.Dialogs.AddEdit.Objects.OperConsumer;
using GazRouter.ObjectModel.Model.Dialogs.AddEdit.Objects.PowerPlant;
using GazRouter.ObjectModel.Model.Dialogs.AddEdit.Objects.PowerUnit;
using GazRouter.ObjectModel.Model.Dialogs.AddEdit.Objects.ReducingStation;
using GazRouter.ObjectModel.Model.Dialogs.AddEdit.Objects.Site;
using GazRouter.ObjectModel.Model.Dialogs.AddEdit.Objects.Valve;
using GazRouter.ObjectModel.Model.Dialogs.AddEdit.Pipelines;
using GazRouter.ObjectModel.Model.Dialogs.AddEdit.Pipelines.Booster;
using GazRouter.ObjectModel.Model.Dialogs.AddEdit.Pipelines.Branch;
using GazRouter.ObjectModel.Model.Dialogs.AddEdit.Pipelines.Bridge;
using GazRouter.ObjectModel.Model.Dialogs.AddEdit.Pipelines.InletOutletCompShop;
using GazRouter.ObjectModel.Model.Dialogs.AddEdit.Segments.Diameter;
using GazRouter.ObjectModel.Model.Dialogs.AddEdit.Segments.Group;
using GazRouter.ObjectModel.Model.Dialogs.AddEdit.Segments.Pressure;
using GazRouter.ObjectModel.Model.Dialogs.AddEdit.Segments.Site;
using GazRouter.ObjectModel.Model.Dialogs.AddEdit.Segments.Regions;
using GazRouter.ObjectModel.Model.Dialogs.AddEdit.Pipelines.Chamber;

namespace GazRouter.ObjectModel.Model.Dialogs
{
    public static class DialogHelper
    {
        public static void AddSite(Action<Guid> callback, Guid parentEnterpriseId, int gasTransportSystemId, int sortorder)
        {
			var viewModel = new AddEditSiteViewModel(callback, parentEnterpriseId, gasTransportSystemId, sortorder);
            var view = new AddEditSiteView {DataContext = viewModel};
            view.ShowDialog();
        }

		public static void EditSite(Action<Guid> callback, SiteDTO site)
        {
            var viewModel = new AddEditSiteViewModel(callback, site);
            var view = new AddEditSiteView {DataContext = viewModel};
            view.ShowDialog();
        }

		public static void AddCompStation(Action<Guid> callback, Guid parentSiteId, int sortorder)
        {
			var viewModel = new AddEditCompStationViewModel(callback, parentSiteId, sortorder);
            var view = new AddEditCompStationView {DataContext = viewModel};
            view.ShowDialog();
        }

		public static void EditCompStation(Action<Guid> callback, CompStationDTO compStation)
        {
			var viewModel = new AddEditCompStationViewModel(callback, compStation);
            var view = new AddEditCompStationView {DataContext = viewModel};
            view.ShowDialog();
        }

		public static void AddCompShop(Action<Guid> callback, Guid parentCompStationId, int sortorder)
        {
			var viewModel = new AddEditCompShopViewModel(callback, parentCompStationId, sortorder);
            var view = new AddEditCompShopView {DataContext = viewModel};
            view.ShowDialog();
        }

		public static void EditCompShop(Action<Guid> callback, CompShopDTO compShop)
        {
			var viewModel = new AddEditCompShopViewModel(callback, compShop);
            var view = new AddEditCompShopView {DataContext = viewModel};
            view.ShowDialog();
        }

        public static void AddCompUnit(Action<Guid> callback, Guid parentCompUnitId, EngineClass engineClass)
        {
            var viewModel = new AddEditCompUnitViewModel(callback, parentCompUnitId, engineClass);
            var view = new AddEditCompUnitView {DataContext = viewModel};
            view.ShowDialog();
        }

        public static void EditCompUnit(Action<Guid> callback, CompUnitDTO compUnit)
        {
            var viewModel = new AddEditCompUnitViewModel(callback, compUnit);
            var view = new AddEditCompUnitView {DataContext = viewModel};
            view.ShowDialog();
        }

		public static void AddMeasStation(Action<Guid> callback, Guid parentSiteId, int sortorder, int gasTransportSystemId)
        {
			var viewModel = new AddEditMeasStationViewModel(callback, parentSiteId, sortorder, gasTransportSystemId);
            var view = new AddEditMeasStationView {DataContext = viewModel};
            view.ShowDialog();
        }

		public static void EditMeasStation(Action<Guid> callback, MeasStationDTO measStation)
        {
			var viewModel = new AddEditMeasStationViewModel(callback, measStation);
            var view = new AddEditMeasStationView {DataContext = viewModel};
            view.ShowDialog();
        }

		public static void AddMeasLine(Action<Guid> callback, Guid parentMeasStationId, int sortorder)
        {
			var viewModel = new AddEditMeasLineViewModel(callback, parentMeasStationId, sortorder);
            var view = new AddEditMeasLineView {DataContext = viewModel};
            view.ShowDialog();
        }

        public static void EditMeasLine(Action<Guid> callback, MeasLineDTO measLine)
        {
            var viewModel = new AddEditMeasLineViewModel(callback, measLine);
            var view = new AddEditMeasLineView {DataContext = viewModel};
            view.ShowDialog();
        }

        public static void EditPipeline(Action<Guid> callback, PipelineDTO pipeline)
        {
            var viewModel = new AddEditPipelineViewModel(callback, pipeline);
            var view = new AddEditPipelineView { DataContext = viewModel };
            view.ShowDialog();
        }

        public static void EditInletOutletPipelineCompShop(Action<Guid> callback, PipelineDTO pipeline, PipelineDTO parentPipeline)
        {
            var viewModel = new AddEditPipelineInletOutletCompShopViewModel(callback, pipeline, parentPipeline);
            var view = new AddEditPipelineInletOutletCompShopView { DataContext = viewModel };
            view.ShowDialog();
        }

        public static void EditLinearValve(Action<Guid> callback, ValveDTO valve, PipelineDTO parentPipeline)
        {
            var viewModel = new AddEditValveViewModel(callback, valve, parentPipeline);
            var view = new AddEditValveView {DataContext = viewModel};
            view.ShowDialog();
        }

		public static void AddDistrStation(Action<Guid> callback, Guid parentSiteId, int systemId)
        {
            var viewModel = new AddEditDistrStationViewModel(callback, parentSiteId, systemId);
            var view = new AddEditDistrStationView {DataContext = viewModel};
            view.ShowDialog();
        }

        public static void EditDistrStation(Action<Guid> callback, DistrStationDTO distrStation)
        {
            var viewModel = new AddEditDistrStationViewModel(callback, distrStation);
            var view = new AddEditDistrStationView {DataContext = viewModel};
            view.ShowDialog();
        }

		public static void AddOutlet(Action<Guid> callback, Guid distrStationId, int sortorder)
        {
            var viewModel = new AddEditDistrStationOutletViewModel(callback, distrStationId, sortorder);
            var view = new AddEditDistrStationOutletView {DataContext = viewModel};
            view.ShowDialog();
        }

        public static void EditOutlet(Action<Guid> callback, DistrStationOutletDTO outlet)
        {
            var viewModel = new AddEditDistrStationOutletViewModel(callback, outlet);
            var view = new AddEditDistrStationOutletView {DataContext = viewModel};
            view.ShowDialog();
        }

        public static void AddConsumer(Action<Guid> callback, Guid parentId)
        {
            var viewModel = new AddEditConsumerViewModel(callback, parentId);
            var view = new AddEditConsumerView { DataContext = viewModel };
            view.ShowDialog();
        }

        public static void EditConsumer(Action<Guid> callback, ConsumerDTO consumer)
        {
            var viewModel = new AddEditConsumerViewModel(callback, consumer);
            var view = new AddEditConsumerView { DataContext = viewModel };
            view.ShowDialog();
        }

        public static void AddReducingStationTree(Action<Guid> callback, Guid siteId, int sortorder)
        {
            var viewModel = new AddEditReducingStationViewModel(callback, siteId, sortorder);
            var view = new AddEditReducingStationView {DataContext = viewModel};
            view.ShowDialog();
        }

        public static void EditReducingStationTree(Action<Guid> callback, ReducingStationDTO siteId)
        {
            var viewModel = new AddEditReducingStationViewModel(callback, siteId);
            var view = new AddEditReducingStationView {DataContext = viewModel};
            view.ShowDialog();
        }

        public static void AddSiteSegment(Action<int> callback, PipelineDTO pipeline)
        {
            var viewModel = new AddEditSiteSegmentViewModel(callback, pipeline);
            var view = new AddEditSiteSegmentView {DataContext = viewModel};
            view.ShowDialog();
        }

        public static void EditSiteSegment(Action<int> callback, SiteSegmentDTO siteSegmentId, PipelineDTO pipeline)
        {
            var viewModel = new AddEditSiteSegmentViewModel(callback, siteSegmentId, pipeline);
            var view = new AddEditSiteSegmentView {DataContext = viewModel};
            view.ShowDialog();
        }

        public static void AddRegionSegment(Action<int> callback, PipelineDTO pipeline)
        {
            var viewModel = new AddEditRegionSegmentViewModel(callback, pipeline);
            var view = new AddEditRegionSegmentView { DataContext = viewModel };
            view.ShowDialog();
        }

        public static void EditRegionSegment(Action<int> callback, RegionSegmentDTO RegionSegmentId, PipelineDTO pipeline)
        {
            var viewModel = new AddEditRegionSegmentViewModel(callback, RegionSegmentId, pipeline);
            var view = new AddEditRegionSegmentView { DataContext = viewModel };
            view.ShowDialog();
        }

        public static void AddGroupSegment(Action<int> callback, PipelineDTO pipeline)
        {
            var viewModel = new AddEditGroupSegmentViewModel(callback, pipeline);
            var view = new AddEditGroupSegmentView { DataContext = viewModel };
            view.ShowDialog();
        }

        public static void EditGroupSegment(Action<int> callback, GroupSegmentDTO segment, PipelineDTO pipeline)
        {
            var viewModel = new AddEditGroupSegmentViewModel(callback, segment, pipeline);
            var view = new AddEditGroupSegmentView { DataContext = viewModel };
            view.ShowDialog();
        }

        public static void AddPressureSegment(Action<int> callback, PipelineDTO pipeline)
        {
            var viewModel = new AddEditPressureSegmentViewModel(callback, pipeline);
            var view = new AddEditPressureSegmentView {DataContext = viewModel};
            view.ShowDialog();
        }

        public static void EditPressureSegment(Action<int> callback, PressureSegmentDTO segment, PipelineDTO pipeline)
        {
            var viewModel = new AddEditPressureSegmentViewModel(callback, segment, pipeline);
            var view = new AddEditPressureSegmentView {DataContext = viewModel};
            view.ShowDialog();
        }

        public static void AddPipeDiameterSegment(Action<int> callback, PipelineDTO pipeline)
		{
			var viewModel = new AddEditDiameterSegmentViewModel(callback, pipeline);
			var view = new AddEditDiameterSegmentView { DataContext = viewModel };
			view.ShowDialog();
		}

        public static void EditPipeDiameterSegment(Action<int> callback, DiameterSegmentDTO segmentId, PipelineDTO pipeline)
		{
			var viewModel = new AddEditDiameterSegmentViewModel(callback, segmentId, pipeline);
			var view = new AddEditDiameterSegmentView { DataContext = viewModel };
			view.ShowDialog();
		}

		public static void AddCoolingStation(Action<Guid> callback, Guid compStationId, int sortorder)
        {
            var viewModel = new AddEditCoolingStationViewModel(callback, compStationId, sortorder);
            var view = new AddEditCoolingStationView { DataContext = viewModel };
            view.ShowDialog();
        }

        public static void EditCoolingStation(Action<Guid> callback, CoolingStationDTO dto)
        {
            var viewModel = new AddEditCoolingStationViewModel(callback, dto);
            var view = new AddEditCoolingStationView { DataContext = viewModel };
            view.ShowDialog();
        }
        
		public static void AddBoiler(Action<Guid> callback, CommonEntityDTO parentEntity)
		{
			var viewModel = new AddEditBoilerViewModel(callback, parentEntity);
			var view = new AddEditBoilerView { DataContext = viewModel };
			view.ShowDialog();
		}

		public static void EditBoiler(Action<Guid> callback, BoilerDTO boiler, CommonEntityDTO parentEntity)
		{
            var viewModel = new AddEditBoilerViewModel(callback, boiler, parentEntity);
			var view = new AddEditBoilerView { DataContext = viewModel };
			view.ShowDialog();
		}
		public static void AddPowerUnit(Action<Guid> callback, CommonEntityDTO parentEntity)
		{
			var viewModel = new AddEditPowerUnitViewModel(callback, parentEntity);
			var view = new AddEditPowerUnitView { DataContext = viewModel };
			view.ShowDialog();
		}

		public static void EditPowerUnit(Action<Guid> callback, PowerUnitDTO powerUnit, CommonEntityDTO parentEntity)
		{
            var viewModel = new AddEditPowerUnitViewModel(callback, powerUnit, parentEntity);
			var view = new AddEditPowerUnitView { DataContext = viewModel };
			view.ShowDialog();
		}

        public static void AddBoilerPlant(Action<Guid> callback, Guid parentCompStationId, int sortorder)
        {
            var viewModel = new AddEditBoilerPlantViewModel(callback, parentCompStationId, sortorder);
            var view = new AddEditBoilerPlantView { DataContext = viewModel };
            view.ShowDialog();
        }

        public static void EditBoilerPlant(Action<Guid> callback, BoilerPlantDTO boilerPlant)
        {
            var viewModel = new AddEditBoilerPlantViewModel(callback, boilerPlant);
            var view = new AddEditBoilerPlantView { DataContext = viewModel };
            view.ShowDialog();
        }

        public static void AddPowerPlant(Action<Guid> callback, Guid parentCompStationId, int sortorder)
        {
            var viewModel = new AddEditPowerPlantViewModel(callback, parentCompStationId, sortorder);
            var view = new AddEditPowerPlantView { DataContext = viewModel };
            view.ShowDialog();
        }

        public static void EditPowerPlant(Action<Guid> callback, PowerPlantDTO powerPlant)
        {
            var viewModel = new AddEditPowerPlantViewModel(callback, powerPlant);
            var view = new AddEditPowerPlantView { DataContext = viewModel };
            view.ShowDialog();
        }

        public static void AddCoolingUnit(Action<Guid> callback, Guid coolingStationId, int sortorder)
		{
			var viewModel = new AddEditCoolingUnitViewModel(callback, coolingStationId);
			var view = new AddEditCoolingUnitView { DataContext = viewModel };
			view.ShowDialog();
		}

		public static void EditCoolingUnit(Action<Guid> callback, CoolingUnitDTO dto)
		{
			var viewModel = new AddEditCoolingUnitViewModel(callback, dto);
			var view = new AddEditCoolingUnitView { DataContext = viewModel };
			view.ShowDialog();
		}

        public static void EditBoosterPipeline(Action<Guid> callback, PipelineDTO pipeline, PipelineDTO parentPipeline)
        {
            var viewModel = new AddEditPipelineBoosterViewModel(callback, pipeline, parentPipeline);
            var view = new AddEditPipelineBoosterView { DataContext = viewModel };
            view.ShowDialog();
        }

        public static void EditPipelineBranch(Action<Guid> callback, PipelineDTO pipeline, PipelineDTO parentPipeline)
        {
            var viewModel = new AddEditPipelineBranchViewModel(callback, pipeline, parentPipeline);
            var view = new AddEditPipelineBranchView { DataContext = viewModel };
            view.ShowDialog();
        }

        public static void EditPipelineBridge(Action<Guid> callback, PipelineDTO pipeline, PipelineDTO parentPipeline)
        {
            var viewModel = new AddEditPipelineBridgeViewModel(callback, pipeline, parentPipeline);
            var view = new AddEditPipelineBridgeView { DataContext = viewModel };
            view.ShowDialog();
        }

        public static void EditPipelineChamber(Action<Guid> callback, PipelineDTO pipeline, PipelineDTO parentPipeline)
        {
            var viewModel = new AddEditPipelineChamberViewModel(callback, pipeline);
            var view = new AddEditPipelineChamberView { DataContext = viewModel };
            view.ShowDialog();
        }

        public static void AddMeasPoint(Action<Guid> callback, CommonEntityDTO parent, int sortOrder)
        {
            var viewModel = new AddEditMeasPointViewModel(callback, parent, sortOrder);
            var view = new AddEditMeasPointView { DataContext = viewModel };
            view.ShowDialog();
        }

        public static void EditMeasPoint(Action<Guid> callback, MeasPointDTO point)
        {
            var viewModel = new AddEditMeasPointViewModel(callback, point);
            var view = new AddEditMeasPointView { DataContext = viewModel };
            view.ShowDialog();
        }

        public static void AddOperConsumer(Action<Guid> callback, Guid parentSiteId, int systemId)
        {
            var vm = new AddEditOperConsumersViewModel(callback, parentSiteId, systemId);
            var v = new AddEditOperConsumersView {DataContext = vm};
            v.ShowDialog();
        }

        public static void EditOperConsumer(Action<Guid> callback, OperConsumerDTO consumer)
        {
            var vm = new AddEditOperConsumersViewModel(callback, consumer);
            var v = new AddEditOperConsumersView { DataContext = vm };
            v.ShowDialog();
        }

    }
}