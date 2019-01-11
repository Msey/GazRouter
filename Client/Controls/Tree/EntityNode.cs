using System;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using GazRouter.Application;
using GazRouter.Application.Wrappers.Entity;
using GazRouter.Common.Cache;
using GazRouter.DTO.Dictionaries.BalanceSigns;
using GazRouter.DTO.Dictionaries.EntityTypes;
using GazRouter.DTO.ObjectModel;
using GazRouter.DTO.ObjectModel.Boilers;
using GazRouter.DTO.ObjectModel.CompStations;
using GazRouter.DTO.ObjectModel.MeasStations;
using GazRouter.DTO.ObjectModel.Pipelines;
using GazRouter.DTO.ObjectModel.PowerUnits;
using GazRouter.DTO.ObjectModel.Valves;
using Microsoft.Practices.ServiceLocation;

namespace GazRouter.Controls.Tree
{
    public class EntityNode : NodeBase
    {
        #region Icons

        private static readonly ImageSource _image1 =
            (ImageSource)
                new ImageSourceConverter().ConvertFromString(@"/ObjectModel;component/Model/Pipelines/Images/1.png");

        private static readonly ImageSource _image2 =
            (ImageSource)
                new ImageSourceConverter().ConvertFromString(@"/ObjectModel;component/Model/Pipelines/Images/2.png");

        private static readonly ImageSource _image3 =
            (ImageSource)
                new ImageSourceConverter().ConvertFromString(@"/ObjectModel;component/Model/Pipelines/Images/3.png");

        private static readonly ImageSource _image4 =
            (ImageSource)
                new ImageSourceConverter().ConvertFromString(@"/ObjectModel;component/Model/Pipelines/Images/4.png");

        private static readonly ImageSource _powerUnitImage =
            (ImageSource)
                new ImageSourceConverter().ConvertFromString(
                    "/Common;component/Images/16x16/EntityTypes/power_unit3.png");

        private static readonly ImageSource _measLineImage =
            (ImageSource)
                new ImageSourceConverter().ConvertFromString("/Common;component/Images/16x16/EntityTypes/meas_line.png");

        private static readonly ImageSource _measLineInImage =
            (ImageSource)
                new ImageSourceConverter().ConvertFromString(
                    "/Common;component/Images/16x16/EntityTypes/meas_line_in.png");

        private static readonly ImageSource _measLineOutImage =
            (ImageSource)
                new ImageSourceConverter().ConvertFromString(
                    "/Common;component/Images/16x16/EntityTypes/meas_line_out.png");

        private static readonly ImageSource _measPointImage =
            (ImageSource)
                new ImageSourceConverter().ConvertFromString("/Common;component/Images/16x16/EntityTypes/meas_point.png");

        private static readonly ImageSource _boilerPlantImage =
            (ImageSource)
                new ImageSourceConverter().ConvertFromString("/Common;component/Images/16x16/EntityTypes/group.png");

        private static readonly ImageSource _coolingStationImage =
            (ImageSource)
                new ImageSourceConverter().ConvertFromString(
                    "/Common;component/Images/16x16/EntityTypes/cooling_station.png");

        private static readonly ImageSource _valveImage =
            (ImageSource)
                new ImageSourceConverter().ConvertFromString("/Common;component/Images/16x16/EntityTypes/valve.png");

        private static readonly ImageSource _distStationImage =
            (ImageSource)
                new ImageSourceConverter().ConvertFromString(
                    "/Common;component/Images/16x16/EntityTypes/distr_station.png");

        #endregion

        public EntityNode(CommonEntityDTO entity)
        {
            _entity = entity;
        }

        public static ImageSource SelectIcon(bool hasBegin, bool hasEnd)
        {
            ImageSource icon = null;
            if (!hasBegin && !hasEnd)
            {
                icon = _image1;
            }
            if (hasBegin && !hasEnd)
            {
                icon = _image2;
            }
            if (!hasBegin && hasEnd)
            {
                icon = _image3;
            }
            if (hasBegin && hasEnd)
            {
                icon = _image4;
            }
            return icon;
        }

        public override string Name
        {
            get
            {
                switch (Entity.EntityType)
                {
                    case EntityType.Valve:
                    {
                        var valve = (ValveDTO) Entity;
                        return
                            $"{valve.Name} ({valve.Kilometer}; {ClientCache.DictionaryRepository.ValveTypes.Single(v => v.Id == valve.ValveTypeId).Name})";
                    }
                    case EntityType.Boiler:
                    {
                        var boiler = (BoilerDTO) Entity;
                        if (boiler.ParentEntityType == EntityType.Pipeline)
                        {
                            return
                                $"{boiler.Name} ({boiler.Kilometr}; {ClientCache.DictionaryRepository.BoilerTypes.Single(v => v.Id == boiler.BoilerTypeId).Name})";
                        }
                        break;
                    }
                    case EntityType.PowerUnit:
                    {
                        var powerUnit = (PowerUnitDTO) Entity;
                        if (powerUnit.ParentEntityType == EntityType.Pipeline)
                        {
                            return
                                $"{powerUnit.Name} ({powerUnit.Kilometr}; {ClientCache.DictionaryRepository.PowerUnitTypes.Single(v => v.Id == powerUnit.PowerUnitTypeId).Name})";
                        }
                        break;
                    }
                    case EntityType.Pipeline:
                    {
                        var pipeline = (PipelineDTO) Entity;
                        return $"{pipeline.Name} ({pipeline.KilometerOfStartPoint}; {pipeline.KilometerOfEndPoint})";
                    }
                }

                return Entity.Name;
            }
        }

        private static IClientCache ClientCache => ServiceLocator.Current.GetInstance<IClientCache>();

        public override ImageSource ImageSource
        {
            //todo здесь должны быть прописаны иконки в зависимости от типа сущности
            get
            {
                switch (Entity.EntityType)
                {
                    case EntityType.Consumer:
                        break;

                    case EntityType.DistrStation:
                        return _distStationImage;

                    case EntityType.Enterprise:
                        break;

                    case EntityType.Valve:
                        return _valveImage;

                    case EntityType.MeasLine:
                        return _measLineImage;

                    case EntityType.Site:
                        break;
                    case EntityType.CompStation:
                        break;
                    case EntityType.CompShop:
                        break;
                    case EntityType.CompUnit:
                        break;
                    case EntityType.CngFillingStation:
                        break;
                    case EntityType.ReducingStation:
                        break;
                    case EntityType.Boiler:
                        break;
                    case EntityType.BoilerPlant:
                        return _boilerPlantImage;

                    case EntityType.MeasStation:
                    {
                        var station = (MeasStationDTO) Entity;
                        if (station.BalanceSignId == Sign.In) return _measLineInImage;
                        if (station.BalanceSignId == Sign.Out) return _measLineOutImage;
                        return _measLineImage;
                    }


                    case EntityType.PowerUnit:
                        return _powerUnitImage;

                    case EntityType.DistrStationOutlet:
                        break;
                    case EntityType.Pipeline:
                    {
                        var pipeline = (PipelineDTO) Entity;
                        return SelectIcon(pipeline.BeginEntityId.HasValue, pipeline.EndEntityId.HasValue);
                    }
                    case EntityType.PipelineGroup:
                        break;
                    case EntityType.MeasPoint:
                        return _measPointImage;

                    case EntityType.CoolingStation:
                        return _coolingStationImage;
                }

                return base.ImageSource;
            }
        }

        public override string ToolTipType
        {
            get
            {
                return ClientCache.DictionaryRepository.EntityTypes.
                    Where(e => e.EntityType == Entity.EntityType).
                    Select(e => e.Name).FirstOrDefault();
            }
        }

        public int SortOrder
        {
            get { return Entity.SortOrder; }
            set
            {
                Entity.SortOrder = value;
                OnPropertyChanged(() => SortOrder);
            }
        }

        public void UpdateSortOrder()
        {
            OnPropertyChanged(() => SortOrder);
        }


        private readonly CommonEntityDTO _entity;

        public override CommonEntityDTO Entity => _entity;

        public IEntityWrapper EntityWrapper => Entity.GetWrapper(true);

        public override bool IsVirtual => Entity.IsVirtual;

        public override bool HasComment => !string.IsNullOrEmpty(Entity.Description);
    }
}