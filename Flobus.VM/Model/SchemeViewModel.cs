using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using GazRouter.Application;
using GazRouter.Common.ViewModel;
using GazRouter.DTO.Appearance;
using GazRouter.DTO.ObjectModel.CompShops;
using GazRouter.DTO.ObjectModel.Pipelines;
using GazRouter.Flobus.Interfaces;
using GazRouter.Flobus.Model;
using GazRouter.Flobus.UiEntities.FloModel;
using GazRouter.Flobus.VM.Serialization;
using Newtonsoft.Json;
using Telerik.Windows.Diagrams.Core;

namespace GazRouter.Flobus.VM.Model
{
    public class SchemeViewModel : PropertyChangedBase, ISchemaSource
    {
        private SchemeVersion _schemeInfo;
        private bool _isChanged;


        public FloModel.FloModelHelper fmHelper = null;

        public SchemeViewModel(SchemeModelDTO schemeModelDto)
        {
            Dto = schemeModelDto;

            ((ObservableCollection<IPipeline>) Pipelines).CollectionChanged += CollectionChanged;

            Valves = new List<Valve>();
            ((ObservableCollection<DistributingStation>)DistributingStations).CollectionChanged += CollectionChanged;
            PipelineConnections = new List<PipelineConnection>();
            MeasuringLines = new List<MeasuringLine>();
            ReducingStations = new List<ReducingStation>();
            PipelineDiameterSegments = new List<PipelineDiameterSegment>();
            ((ObservableCollection<CompressorShop>) CompressorShops).CollectionChanged += CollectionChanged;

            var observable_text_Collection = new ObservableCollection<ITextBlock>();
            observable_text_Collection.CollectionChanged += CollectionChanged;
            TextBlocks = observable_text_Collection;

            var observable_line_Collection = new ObservableCollection<IPolyLine>();
            observable_line_Collection.CollectionChanged += CollectionChanged;
            PolyLines = observable_line_Collection;

            var observable_check_valve_Collection = new ObservableCollection<ICheckValve>();
            observable_check_valve_Collection.CollectionChanged += CollectionChanged;
            CheckValves = observable_check_valve_Collection;

            PipelineMarkers = new List<PipelineMarker>();

            _schemeInfo = new SchemeVersion
            {
                SchemeName = "Новая схема",
                VersionId = 1,
                CreationDate = DateTime.Now,
                Creator = UserProfile.Current.FullName,
                IsPublished = false
            };
        }

        public SchemeModelDTO Dto { get; set; }

        /// <summary>
        ///     Список газопроводов
        /// </summary>
        IList<IPipeline> ISchemaSource.Pipelines => Pipelines;

        /// <summary>
        /// Список сегментов разного диаметра
        /// </summary>
        public List<PipelineDiameterSegment> PipelineDiameterSegments { get; set; }

        /// <summary>
        ///     Список кранов
        /// </summary>
        public List<Valve> Valves { get; }

        /// <summary>
        ///     Список ГРС
        /// </summary>
        public IList<DistributingStation> DistributingStations { get; } = new ObservableCollection<DistributingStation>();

        /// <summary>
        ///     Список подключений газопроводов
        /// </summary>
        public List<PipelineConnection> PipelineConnections { get; private set; }

        /// <summary>
        ///     Список точек измерения газа (ГИС)
        /// </summary>
        public List<MeasuringLine> MeasuringLines { get; private set; }

        /// <summary>
        ///     Список КЦ
        /// </summary>
        public IList<CompressorShop> CompressorShops { get; } = new ObservableCollection<CompressorShop>();

        public IList<ITextBlock> TextBlocks { get; }

        public IList<IPolyLine> PolyLines { get; }

        public IList<ICheckValve> CheckValves { get; }

        /// <summary>
        ///     Список ПРГ
        /// </summary>
        public List<ReducingStation> ReducingStations { get; private set; }

        /// <summary>
        ///     Список маркеров газопровода
        /// </summary>
        public List<PipelineMarker> PipelineMarkers { get; }

/*
        /// <summary>
        /// Актуальный размер схемы.
        /// Расчитывается как максимальное значение координаты X и Y среди всех элементов схемы
        /// </summary>
        public Rect ActualRect
        {
            get
            {
                int cnt = 0;
                double right = 0;
                double bottom = 0;
                double left = double.MaxValue;
                double top = double.MaxValue;

                // поиск максимальной и минимальной координаты среди газопроводов
                foreach (Pipeline pipe in Pipelines)
                {
                    
                    Rect? prct = pipe.PipelineRect;
                    if (prct.HasValue)
                    {
                        if (prct.Value.Right > right) right = prct.Value.Right;
                        if (prct.Value.Bottom > bottom) bottom = prct.Value.Bottom;
                        if (prct.Value.Left < left) left = prct.Value.Left;
                        if (prct.Value.Top < top) top = prct.Value.Top;
                        cnt++;
                    }
                }
                if (cnt == 0) left = top = 0;
                
                return new Rect(left, top, right - left, bottom - top);
            }
        }
*/

        /// <summary>
        ///     Инфрормация о схеме
        /// </summary>
        public SchemeVersion SchemeInfo
        {
            get { return _schemeInfo; }
            internal set
            {
                _schemeInfo = value;
                OnPropertyChanged(() => SchemeInfo);
            }
        }

        public Dictionary<Guid, PipelineDTO> PipelinesDict { get; set; }

        public bool IsChanged
        {
            get { return _isChanged; }
            set { SetProperty(ref _isChanged, value); }
        }

        /// <summary>
        ///     Список компрессорных цехов (КЦ)
        /// </summary>
        IEnumerable<ICompressorShop> ISchemaSource.CompressorShops => CompressorShops.OfType<ICompressorShop>().ToList();

        IEnumerable<IDistributingStation> ISchemaSource.DistributingStations => DistributingStations.OfType<IDistributingStation>().ToList();

        IEnumerable<ITextBlock> ISchemaSource.TextBlocks => TextBlocks;
        IEnumerable<IPolyLine> ISchemaSource.PolyLines => PolyLines;
        IEnumerable<ICheckValve> ISchemaSource.CheckValves => CheckValves;
        public IList<IPipeline> Pipelines { get; } = new ObservableCollection<IPipeline>();

        public void AddPipelineMarker(Guid pipelineId, double kmBegining, double kmEnd, Color color, string description,
            object data, List<MenuItemCommand> menuCommands)
        {
            throw new NotImplementedException();
            /*

             var pipeline = Pipelines.FirstOrDefault(p => p.Id == pipelineId);
             if (pipeline == null) return;

             PipelineMarkers.Add(new PipelineMarker(pipeline, kmBegining, kmEnd, color, description, data) { MenuCommands = menuCommands});
         */
        }

        public ICompressorShop AddCompressorShops(CompShopDTO dto, Point position)
        {
            var compShop = new CompressorShop(dto, position);
            CompressorShops.Add(compShop);
            IsChanged = true;
            return compShop;
        }

        public ITextBlock AddTextBlock(string text, Point position)
        {
            var textBlock = new TextBlock {Position = position, Text = text};
            TextBlocks.Add(textBlock);
            IsChanged = true;
            return textBlock;
        }
        public IPolyLine AddPolyLine(Point position)
        {
            var polyline = new PolyLine{ Position = position };
            PolyLines.Add(polyline);
            IsChanged = true;
            return polyline;
        }
        public ICheckValve AddCheckValve(Point position)
        {
            var checkvalve = new CheckValve { Position = position, Angle = 90 };
            CheckValves.Add(checkvalve);
            IsChanged = true;
            return checkvalve;
        }


        public IPipeline AddPipeline(PipelineDTO dto, Point position)
        {
            return AddPipeline(dto, position.Add(new Point(-100, 0)), position.Add(new Point(100, 0)));
        }

        public IPipeline AddPipeline(PipelineDTO dto, Point startPoint, Point endPoint)
        {
            var pipeLine = new Pipeline(dto)
            {
                StartPoint = startPoint,
                EndPoint = endPoint //По умолчанию газопровод горизонтальный
            };

            foreach (var valveDTO in Dto.ValveList.Where(v => v.ParentId == pipeLine.Id))
            {
                Valves.Add(pipeLine.AddValve(valveDTO));
            }

            foreach (var measLine in Dto.MeasLineList.Where(v => v.PipelineId == pipeLine.Id))
            {
                pipeLine.AddMeasuringLine(measLine);
            }
            foreach (var reducingStationDTO in Dto.ReducingStationList.Where(v => v.PipelineId == pipeLine.Id))
            {
                pipeLine.AddReducingStation(reducingStationDTO);
            }

            foreach (var connection in Dto.PipelineConnList.Where(d => d.PipelineId == pipeLine.Id))
            {
                if (connection.DistrStationId != null)
                {
                    pipeLine.AddDistributingStation(Dto.DistrStationList.Single(ds => ds.Id == connection.DistrStationId));
                }
                else
                {
                    var connectedPipeline =
                        Pipelines.SingleOrDefault(p => p.Id == connection.DestPipelineId) as Pipeline;
                    if (connectedPipeline != null)
                    {
                        pipeLine.AddPipelineConnection(connection, connectedPipeline);
                    }
                }
            }

            Pipelines.Add(pipeLine);
            IsChanged = true;
            return pipeLine;
        }

        public void RemovePipeline(IPipeline data)
        {
            Pipelines.Remove(data);
            IsChanged = true;
        }

        private bool SetHidden(object v)
        {
            if (v != null)
            {
                IsChanged = true;
                return true;
            }
            else return false;
        }

        public bool RemoveMeasuringLine(Guid id)
        {
            var v = MeasuringLines.First(o => o.Id == id);
            return SetHidden(v) ? v.Hidden = true : false;
        }
        public bool RemoveDistributionStation(Guid id)
        {
            var v = DistributingStations.First(o => o.Id == id);
            return SetHidden(v) ? v.Hidden = true : false;
        }
        public bool RemoveReducingStation(Guid id)
        {
            var v = ReducingStations.First(o => o.Id == id);
            return SetHidden(v) ? v.Hidden = true : false;
        }

        void ISchemaSource.RemoveTextBlock(ITextBlock data)
        {
            TextBlocks.Remove(data);
            IsChanged = true;
        }

        void ISchemaSource.RemovePolyLine(IPolyLine data)
        {
            PolyLines.Remove(data);
            IsChanged = true;
        }
        void ISchemaSource.RemoveCheckValve(ICheckValve data)
        {
            CheckValves.Remove(data);
            IsChanged = true;
        }

        public void RemoveCompressorShops(Guid id)
        {
            CompressorShops.Remove(CompressorShops.First(c => c.Id == id));
            IsChanged = true;
        }

        public string Save()
        {
            IsChanged = false;
            return JsonConvert.SerializeObject(SchemeJson.FromModel(this));
        }

        public SchemeJson Load(string content)
        {
            if (string.IsNullOrEmpty(content))
            {
                return null;
            }
            var json = JsonConvert.DeserializeObject<SchemeJson>(content,
                new JsonSerializerSettings() {MissingMemberHandling = MissingMemberHandling.Ignore});
            return json;
        }

        public void FindObject(ISearchable value)
        {
            throw new NotImplementedException();
        }

        private void CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                foreach (var propertyChanged in e.OldItems.OfType<INotifyPropertyChanged>())
                {
                    propertyChanged.PropertyChanged -= SchemeItemPropertyChanged;
                }
            }

            if (e.NewItems != null)
            {
                foreach (var propertyChanged in e.NewItems.OfType<INotifyPropertyChanged>())
                {
                    propertyChanged.PropertyChanged += SchemeItemPropertyChanged;
                }
            }
        }

        private void SchemeItemPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            IsChanged = true;
        }

        public bool IsMeasuringLineHidden(Guid id)
        {
            var v = MeasuringLines.FirstOrDefault(o => o.Id == id);
            if (v != null)
            {
                return v.Hidden;
            }
            return false;
        }

        public bool IsDistributionStationHidden(Guid id)
        {
            var v = DistributingStations.FirstOrDefault(o => o.Id == id);
            if (v != null)
            {
                return v.Hidden;
            }
            return false;
        }

        public bool IsReducingStationHidden(Guid id)
        {
            var v = ReducingStations.FirstOrDefault(o => o.Id == id);
            if (v != null)
            {
                return v.Hidden;
            }
            return false;
        }
        


        public Dictionary<IPipelineOmElement, string> GetHiddenMeasuringLines(Guid id)
        {
            List<MeasuringLine> temp = null;
            if (id != Guid.Empty)
            {
                temp = MeasuringLines.Where(ml => ml.Pipe.Id == id && ml.Hidden).ToList();
            }
            else
            {
                temp = MeasuringLines.Where(ml => ml.Hidden).ToList();
            }
            Dictionary<IPipelineOmElement, string> result = new Dictionary<IPipelineOmElement, string>();
            if (temp != null)
                foreach (MeasuringLine ml in temp)
                {
                    result.Add(ml, ml.Name);
                }
            return result;
        }

        public Dictionary<IDistrStation, string> GetHiddenDistributionStations(Guid id)
        {
            List<DistributingStation> temp = null;
            if (id != Guid.Empty)
            {
                temp = DistributingStations.Where(ml => ml.Pipe.Id == id && ml.Hidden).ToList();
            }
            else
            {
                temp = DistributingStations.Where(ml => ml.Hidden).ToList();
            }
            Dictionary<IDistrStation, string> result = new Dictionary<IDistrStation, string>();
            if (temp != null)
                foreach (DistributingStation ml in temp)
                {
                    result.Add(ml, ml.Name);
                }
            return result;
        }

        public Dictionary<IPipelineOmElement, string> GetHiddenReducingStations(Guid id)
        {
            List<ReducingStation> temp = null;
            if (id != Guid.Empty)
            {
                temp = ReducingStations.Where(ml => ml.Pipe.Id == id && ml.Hidden).ToList();
            }
            else
            {
                temp = ReducingStations.Where(ml => ml.Hidden).ToList();
            }
            Dictionary<IPipelineOmElement, string> result = new Dictionary<IPipelineOmElement, string>();
            if (temp != null)
                foreach (ReducingStation ml in temp)
                {
                    result.Add(ml, ml.Name);
                }
            return result;
        }
        


        public Guid RestoreMeasuringLine(Guid id)
        {
            var v = MeasuringLines.First(o => o.Id == id);
            if (v != null)
            {
                v.Hidden = false;
                IsChanged = true;
                return v.Pipe.Id;
            }
            else return Guid.Empty;
        }

        public Guid RestoreDistributionStation(Guid id)
        {
            var v = DistributingStations.First(o => o.Id == id);
            if (v != null)
            {
                v.Hidden = false;
                IsChanged = true;
                return v.Pipe.Id;
            }
            else return Guid.Empty;
        }

        public Guid RestoreReducingStation(Guid id)
        {
            var v = ReducingStations.First(o => o.Id == id);
            if (v != null)
            {
                v.Hidden = false;
                IsChanged = true;
                return v.Pipe.Id;
            }
            else return Guid.Empty;
        }
    }
}