using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using GazRouter.DTO.Dictionaries.PipelineTypes;
using GazRouter.DTO.ObjectModel.DistrStations;
using GazRouter.DTO.ObjectModel.MeasLine;
using GazRouter.DTO.ObjectModel.PipelineConns;
using GazRouter.DTO.ObjectModel.Pipelines;
using GazRouter.DTO.ObjectModel.ReducingStations;
using GazRouter.DTO.ObjectModel.Valves;
using GazRouter.Flobus.Interfaces;
using GazRouter.Flobus.Model;
using GazRouter.Flobus.UiEntities.FloModel;
using JetBrains.Annotations;
using System;
using GazRouter.Flobus.VM.FloModel;

namespace GazRouter.Flobus.VM.Model
{
    /// <summary>
    ///     Класс описывает газопровод
    /// </summary>
    public class Pipeline : EntityBase<PipelineDTO>, IPipeline, ISearchable
    {
        private readonly List<MeasuringLine> _measuringLines = new List<MeasuringLine>();
        private readonly List<IGeometryPoint> _pipelinePointList;
        private readonly List<Valve> _valves = new List<Valve>();
        private readonly List<ReducingStation> _reducinsStations = new List<ReducingStation>();
        private readonly List<DistributingStation> _distributingStation = new List<DistributingStation>();
        private Color _color = Colors.Orange;
        private double _thickness = 4;
        private bool _isFound;
        private Point _startPoint;
        private Point _endPoint;

        public Pipeline(PipelineDTO dto) : base(dto)
        {
            _pipelinePointList = new List<IGeometryPoint>();
        }

        public IList<PipelineConnection> PipelineConnections { get; } = new List<PipelineConnection>();

        public string PipeTypeName => ClientCache.DictionaryRepository.PipelineTypes[Dto.Type].Name;

        [UsedImplicitly]
        public string Tooltip => (Color == BreakColor ? "Пропуск информации по диаметрам сегментов\n" : $"{Name}\n({PipeTypeName})\n") + $"км начала: {KmBegining}\nкм конца: {KmEnd}" + _segmentsTooltip;
        

        private string _segmentsTooltip
        {
            get {
                string result = "";
                if (DiameterSegments != null && DiameterSegments.Count> 0)
                {
                    result += "\nСегменты:\n(начало) -> (конец) : (диаметр)";
                    foreach (var s in DiameterSegments)
                    {
                        result += "\n" + s.KmBegining + " -> " + s.KmEnd + " : " + s.ExternalDiameter;
                    }
                }
                return result;
            }
        }

        /// <summary>
        ///     Цвет линии, которым будет рисоваться газопровод
        /// </summary>
        public Color Color
        {
            get { return _color; }
            set { SetProperty(ref _color, value); }
        }

        /// <summary>
        ///     Стандартный цвет линии газопровода. Зависит от его типа.
        ///     Необходим для отображения схемы стандартными цветами для нанесения маркеров на участки
        /// </summary>
        public Color StandardColor
        {
            get
            {
                switch (Dto.Type)
                {
                    case PipelineType.Main:
                        return Color.FromArgb(0xff, 0x27, 0x2D, 0x37); // Магистральный
                    case PipelineType.Distribution:
                        return Color.FromArgb(0xff, 0x27, 0x2D, 0x37); // Распределительный
                    case PipelineType.Looping:
                        return Color.FromArgb(0xff, 0x27, 0x2D, 0x37); // Лупинг
                    case PipelineType.Bridge:
                        return Color.FromArgb(0xff, 0x80, 0x80, 0x80); // Перемычка
                    case PipelineType.Booster:
                        return Color.FromArgb(0xff, 0x27, 0x2D, 0x37); // Резервная нитка
                    case PipelineType.Branch:
                        return Color.FromArgb(0xff, 0x80, 0x80, 0x80); // Отвод
                    case PipelineType.Inlet:
                        return Color.FromArgb(0xff, 0x27, 0x2D, 0x37); // Газопровод подключения
                    case PipelineType.CompressorShopInlet:
                        return Color.FromArgb(0xff, 0x80, 0x80, 0x80); // Входной КЦ
                    case PipelineType.CompressorShopOutlet:
                        return Color.FromArgb(0xff, 0x80, 0x80, 0x80); // Выходной КЦ
                    default:
                        return Color.FromArgb(0xff, 0x27, 0x2D, 0x37);
                }
            }
        }

        public Color BreakColor
        {
            get
            {
                return Color.FromArgb(0xFF, 0xF5, 0xF5, 0xF5);//WhiteSmoke
            }
        }

        /// <summary>
        ///     Толщина линии, которым будет рисоваться газопровод
        /// </summary>
        public double Thickness
        {
            get { return _thickness; }
            set
            {
                if (value < 1)
                {
                    return;
                }
                SetProperty(ref _thickness, value);
            }
        }

        /// <summary>
        ///     Z-индекс при рисовании газопроводов
        /// </summary>
        public int ZIndex
        {
            get
            {
                switch (Dto.Type)
                {
                    case PipelineType.Main:
                        return 1; // Магистральный
                    case PipelineType.Distribution:
                        return 4; // Распределительный
                    case PipelineType.Looping:
                        return 2; // Лупинг
                    case PipelineType.Bridge:
                        return 3; // Перемычка
                    case PipelineType.Booster:
                        return 3; // Резервная нитка
                    case PipelineType.Branch:
                        return 5; // Отвод
                    case PipelineType.Inlet:
                        return 4; // Газопровод подключения
                    case PipelineType.CompressorShopInlet:
                        return 5; // Входной КЦ
                    case PipelineType.CompressorShopOutlet:
                        return 5; // Выходной КЦ
                    default:
                        return 6;
                }
            }
        }

        /// <summary>
        ///     Список крановых узлов на газопроводе
        /// </summary>
        public IEnumerable<Valve> Valves => _valves;

        /// <summary>
        ///     Список километров, по которым газопровод разбивается на технические участки
        /// </summary>
        public List<double> SegmentKilometers
        {
            get
            {
                var lst = new List<double> {KmBegining, KmEnd};
                lst.AddRange(Valves.Select(v => v.Km));
                lst.Sort();
                return lst;
            }
        }

        /// <summary>
        ///     Список сегментов по диаметру
        /// </summary>
        public List<PipelineDiameterSegment> DiameterSegments { get; set; }

        /// <summary>
        ///     Список точек измерения газа (ГИС)
        /// </summary>
        public IEnumerable<MeasuringLine> MeasuringLines => _measuringLines;

        /// <summary>
        ///     Список ПРГ
        /// </summary>
        public IEnumerable<ReducingStation> ReducingStations => _reducinsStations;

        /// <summary>
        ///     Список ГРС
        /// </summary>
        public IEnumerable<DistributingStation> DistributingStations => _distributingStation;

        public ICollection<PipelineMarkup> Markups { get; } = new ObservableCollection<PipelineMarkup>();

        /// <summary>
        ///     Пикетный километр начала
        /// </summary>
        public double KmBegining => Dto.KilometerOfStartPoint;

        /// <summary>
        ///     Пикетный километр конца
        /// </summary>
        public double KmEnd => Dto.KilometerOfEndPoint;

        /*   public double MinAllowedKm(IPipelinePoint pipelinePoint)
        {
          return  (_pipelinePointList.PreviousPoint(pipelinePoint)?.Km ?? KmBegining) + 0.01;
        }

        public double MaxAllowedKm(IPipelinePoint pipelinePoint)
        {
            return (_pipelinePointList.NextPoint(pipelinePoint)?.Km ?? KmEnd) - 0.01;
        }*/

        /// <summary>
        ///     Координаты точек газопровода для рисования
        /// </summary>
        public IEnumerable<IGeometryPoint> IntermediatePoints => _pipelinePointList.AsReadOnly();

        public Point StartPoint
        {
            get { return _startPoint; }
            set { SetProperty(ref _startPoint, value); }
        }

        public Point EndPoint
        {
            get { return _endPoint; }
            set { SetProperty(ref _endPoint, value); }
        }

        IEnumerable<IValve> IPipeline.Valves => Valves;

        IEnumerable<IDistrStation> IPipeline.DistributingStations => DistributingStations;

        IEnumerable<IPipelineOmElement> IPipeline.MeasuringLines => MeasuringLines;

        IEnumerable<IPipelineOmElement> IPipeline.ReducingStations => ReducingStations;

        IEnumerable<IPipelineConnectionHint> IPipeline.PipelineConnections => PipelineConnections;

        public bool IsFound
        {
            get { return _isFound; }
            set { SetProperty(ref _isFound, value); }
        }


        /// <summary>
        ///     Список сегментов-пропусков
        /// </summary>
        public List<Segment> OverlaySegments => GetGapsKm();

        private List<Segment> GetGapsKm()
        {
            List<Segment> gaps = new List<Segment>();

            var segments = DiameterSegments;
            if (segments != null && segments.Count > 0)
            {
                for (int i = 0; i < segments.Count - 1; i++)
                {
                    bool add = true;
                    double minvalue = 10000;
                    int overlay = 0;
                    for (int j = i + 1; j < segments.Count; j++)
                    {
                        if (segments[i].KmEnd >= segments[j].KmBegining)
                        {
                            add = false;
                            break;
                        }
                        else if ((segments[j].KmBegining > segments[i].KmEnd) && (segments[j].KmBegining - segments[i].KmEnd) < minvalue)
                        {
                            minvalue = segments[i].KmBegining - segments[j].KmEnd;
                            overlay = j;
                        }
                    }
                    if (add)
                    {
                        gaps.Add(new Segment(segments[i].KmEnd, segments[overlay].KmBegining));
                    }
                }
            }

            return gaps;
        }

        public IGeometryPoint AddPoint(double km, Point point)
        {
            var geometryPoint = _pipelinePointList.FirstOrDefault(c => c.Km == km);

            if (geometryPoint == null)
            {
                geometryPoint = new GeometryPoint(km, point);
                ((INotifyPropertyChanged) geometryPoint).PropertyChanged += OnPointPropertyChanged;
                _pipelinePointList.Add(geometryPoint);
            }
            return geometryPoint;
        }

        public Valve AddValve(ValveDTO valveDTO)
        {
            var valve = new Valve(valveDTO, this);
            valve.PropertyChanged += (sender, args) => OnPropertyChanged(() => Valves);

            _valves.Add(valve);
            return valve;
        }

        IGeometryPoint IPipeline.AddPoint(double km, Point point)
        {
            var newPoint = new GeometryPoint(km, point);

            newPoint.PropertyChanged += OnPointPropertyChanged;
            for (var index = 0; index < _pipelinePointList.Count; index++)
            {
                var geometryPoint = _pipelinePointList[index];
                if (newPoint.Km < geometryPoint.Km)
                {
                    _pipelinePointList.Insert(index, newPoint);
                    return newPoint;
                }
            }
            _pipelinePointList.Add(newPoint);
            return newPoint;
        }

        public PipelineConnection AddPipelineConnection(PipelineConnDTO pipelineConnDTO, Pipeline connectedPipeline)
        {
            var conn = new PipelineConnection(pipelineConnDTO, this, connectedPipeline);
            PipelineConnections.Add(conn);
            return conn;
        }

        public ReducingStation AddReducingStation(ReducingStationDTO reducingStationDTO)
        {
            var station = new ReducingStation(reducingStationDTO, this);
            station.PropertyChanged += (sender, args) => OnPropertyChanged(() => ReducingStations);
            _reducinsStations.Add(station);
            return station;
        }

        public MeasuringLine AddMeasuringLine(MeasLineDTO measLine)
        {
            var measuringLine = new MeasuringLine(measLine, this);
            measuringLine.PropertyChanged += (sender, args) => OnPropertyChanged(() => MeasuringLines);
            _measuringLines.Add(measuringLine);
            //                 AttachPoint(measuringLine);
            return measuringLine;
        }

        public DistributingStation AddDistributingStation(DistrStationDTO distrStation)
        {
            var distributingStation = new DistributingStation(distrStation, this);
            distributingStation.PropertyChanged += (sender, args) => OnPropertyChanged(() => DistributingStations);
            _distributingStation.Add(distributingStation);
            return distributingStation;
        }

        /// <summary>
        ///     Возвращает объект PipelinePoint для указанного значения километра
        /// </summary>
        /// <param name="km">Километр газопровода</param>
        /// <returns>Возвращает PipelinePoint, если точка с указанным километром не существует, то null</returns>
        public IGeometryPoint FindPoint(double km)
        {
            return _pipelinePointList.SingleOrDefault(d => d.Km == km);
        }

        public void RemovePoint(double km)
        {
            var geometryPoint = _pipelinePointList.FirstOrDefault(c => c.Km == km);

            if (geometryPoint != null)
            {
                var notifyPropertyChanged = geometryPoint as INotifyPropertyChanged;
                if (notifyPropertyChanged != null)
                {
                    notifyPropertyChanged.PropertyChanged -= OnPointPropertyChanged;
                }
                _pipelinePointList.Remove(geometryPoint);
            }
        }
        
        public void RemoveMeasuringLine(Guid id)
        {
            var v = _measuringLines.First(o => o.Id == id);
            if (v != null)
            {
                //_measuringLines.Remove(v);
                v.Hidden = true;
                OnPropertyChanged(() => MeasuringLines);
            }
        }

        private void OnPointPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged(() => IntermediatePoints);
        }
    }
}