using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media;
using GazRouter.Application;
using GazRouter.Common.Cache;
using GazRouter.Common.ViewModel;
using GazRouter.DTO.Dictionaries.PeriodTypes;
using GazRouter.DTO.Dictionaries.PhisicalTypes;
using GazRouter.DTO.Dictionaries.PropertyTypes;
using GazRouter.DTO.ObjectModel;
using GazRouter.DTO.SeriesData.PropertyValues;
using Microsoft.Practices.ServiceLocation;


namespace GazRouter.Controls.Trends
{
    public class Trend : PropertyChangedBase
    {
        private readonly Action<Trend> _reloadTrend;
        private readonly Action _update;


        public Trend(Action<Trend> reloadTrend,
            Action update,
            CommonEntityDTO entity,
            PropertyType propertyType,
            PeriodType periodType,
            Color color,
            int thickness, 
            DateTime periodBegin,
            DateTime periodEnd)
        {
            _reloadTrend = reloadTrend;
            _update = update;
            Entity = entity;
            _propertyType = propertyType;
            _periodType = periodType;
            _color = color;
            _thickness = thickness;
            _periodBegin = periodBegin;
            _periodEnd = periodEnd;
            
            reloadTrend(this);
        }

        


        /// <summary>
        /// Выбранная сущность (объект)
        /// </summary>
        public CommonEntityDTO Entity { get; private set; }
        



        private PropertyType _propertyType;

        /// <summary>
        /// Тип свойства
        /// </summary>
        public PropertyType PropertyType
        {
            get { return _propertyType; }
            set
            {
                if (SetProperty(ref _propertyType, value))
                {
                    OnPropertyChanged(() => PropertyTypeDto);
                    _reloadTrend(this);
                }
            }
        }

        /// <summary>
        /// DTO - для типа свойства тренда, чтобы можно было быстро получать
        /// всю необходимую информацию по типу свойства
        /// </summary>
        public PropertyTypeDTO PropertyTypeDto
        {
            get
            {
                return
                    ClientCache.DictionaryRepository.PropertyTypes.SingleOrDefault(
                        pt => pt.PropertyType == _propertyType);
            }
        }


        /// <summary>
        /// Список типов свойств для выбранного объекта,
        /// позволяется в гриде быстро сменить типа свойства и перестроить график
        /// </summary>
        public List<PropertyTypeDTO> PropertyTypeList
        {
            get
            {
                return
                    ClientCache.DictionaryRepository.EntityTypes.Single(et => et.EntityType == Entity.EntityType)
                        .EntityProperties.Where(p => p.PhysicalType.TrendAllowed).ToList();
            }
        }
        private static IClientCache ClientCache => ServiceLocator.Current.GetInstance<IClientCache>();





        private PeriodType _periodType;

        /// <summary>
        /// Тип периода
        /// </summary>
        public PeriodType PeriodType
        {
            get { return _periodType; }
            set
            {
                if (SetProperty(ref _periodType, value))
                    _reloadTrend(this);
            }
        }

        /// <summary>
        /// Список типов периода
        /// </summary>
        public IEnumerable<PeriodType> PeriodTypeList
        {
            get
            {
                yield return PeriodType.Twohours;
                yield return PeriodType.Day;
            }
        }
        

        #region Period

        private DateTime _periodBegin;

        /// <summary>
        /// Начало период за который будет отображаться тренд
        /// </summary>
        public DateTime PeriodBegin
        {
            get { return _periodBegin; }
            set
            {
                if (SetProperty(ref _periodBegin, value))
                {
                    NotifyUpdate();
                }
            }
        }


        private DateTime _periodEnd;

        /// <summary>
        /// Окончание период за который будет отображаться тренд
        /// </summary>
        public DateTime PeriodEnd
        {
            get { return _periodEnd; }
            set
            {
                if (SetProperty(ref _periodEnd, value))
                {
                    NotifyUpdate();
                }
            }
        }

        #endregion





        private List<PropertyValue> _alldata = new List<PropertyValue>();
        
        public void SetData(List<PropertyValueDoubleDTO> data)
        {

            data.ForEach(
                v =>
                    v.Value =
                        Math.Round(UserProfile.ToUserUnits(v.Value, PropertyTypeDto.PhysicalType.PhysicalType),
                            PropertyTypeDto.PhysicalType.DefaultPrecision));
            _alldata = data.Select(v => new PropertyValue(v, this)).ToList();
            NotifyUpdate();
        }


        public List<PropertyValue> AllData
        {
            get { return _alldata; }
        }


        /// <summary>
        /// Данные для отображения на тренде
        /// </summary>
        public List<PropertyValue> TrendData
        {
            get { return _alldata.Where(p => p.Date >= PeriodBegin && p.Date <= PeriodEnd).ToList(); }
        }


        /// <summary>
        /// Максимальное значение за выбранный период
        /// </summary>
        public double? TrendMax
        {
            get
            {
                if (TrendData.Count == 0) return null;

                var max = TrendData.Max(v => v.Value);
                if (double.IsNaN(max)) return null;
                return Math.Round(max, PropertyTypeDto.PhysicalType.DefaultPrecision);
            }
        }


        /// <summary>
        /// Минимальное значение тренда за весь период
        /// </summary>
        public double? Min
        {
            get
            {
                var list = _alldata.Where(v => !double.IsNaN(v.Value)).ToList();
                if (list.Count == 0) return null;
                var min = list.Min(v => v.Value);
                return Math.Round(min, PropertyTypeDto.PhysicalType.DefaultPrecision);
            }
        }

        /// <summary>
        /// Максимальное значение тренда за весь период
        /// </summary>
        public double? Max
        {
            get
            {
                var list = _alldata.Where(v => !double.IsNaN(v.Value)).ToList();
                if (list.Count == 0) return null;
                var max = list.Max(v => v.Value);
                return Math.Round(max, PropertyTypeDto.PhysicalType.DefaultPrecision);
            }
        }

        /// <summary>
        /// Среднее значение тренда за весь период
        /// </summary>
        public double? Avg
        {
            get
            {
                 var list =    _alldata.Where(v => !double.IsNaN(v.Value)).ToList();
                if (list.Count == 0) return null;
                var avg = list.Average(v => v.Value);
                return Math.Round(avg, PropertyTypeDto.PhysicalType.DefaultPrecision);
            }
        }


        private void NotifyUpdate()
        {
            OnPropertyChanged(() => AllData);
            OnPropertyChanged(() => TrendData);
            OnPropertyChanged(() => TrendMax);
            OnPropertyChanged(() => Max);
            OnPropertyChanged(() => Min);
            OnPropertyChanged(() => Avg);


            if (_update != null) _update();
        }


        public string UnitName => UserProfile.UserUnitName(PropertyTypeDto.PhysicalType.PhysicalType);



        private bool _isVisible = true;
        /// <summary>
        /// Отображать тренд или нет
        /// </summary>
        public bool IsVisible
        {
            get { return _isVisible; }
            set
            {
                if(SetProperty(ref _isVisible, value))
                    NotifyUpdate();

            }
        }
        

        private Color _color;

        /// <summary>
        /// Цвет линии тренда
        /// </summary>
        public Color Color
        {
            get { return _color; }
            set
            {
                SetProperty(ref _color, value);
                //_reloadTrend(this);
            }
        }


        private int _thickness;

        /// <summary>
        /// Толщина линии тренда
        /// </summary>
        public int Thickness
        {
            get { return _thickness; }
            set
            {
                SetProperty(ref _thickness, value);
                //_reloadTrend(this);
            }
        }

        /// <summary>
        /// Список возможных толщин линии тренда
        /// </summary>
        public List<int> ThicknessList { get; set; } = new List<int> {1, 2, 3, 4, 5, 6, 7};

        


        



        

       //public void ProcessSeries()
        //{
        //    var dataTemplateString = @"
        //          <DataTemplate
        //             xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
        //             xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"">
        //           <StackPanel Orientation=""Horizontal"">                        
        //                <TextBlock Text=""{Binding DataPoint.DisplayCurrentValue, StringFormat='Значение: {0:N" + 2 +
        //                             @"}'}"" Foreground=""" + Color + @""" />
        //           </StackPanel>
        //         </DataTemplate>";

        //    var tmpTemplate = XamlReader.Load(dataTemplateString) as DataTemplate;
        //    Series = new LineSeries
        //    {
        //        ItemsSource = this,
        //        CategoryBinding = new PropertyNameDataPointBinding("Date"),
        //        ValueBinding = new PropertyNameDataPointBinding("DisplayCurrentValue"),
        //        VerticalAxis = new LinearAxis
        //        {
        //            Title = PropertyTypeDto.PhysicalType.UnitName,
        //            ElementBrush = new SolidColorBrush(Color),
        //            HorizontalLocation = AxisHorizontalLocation.Right
        //        },
        //        Stroke = new SolidColorBrush(Color),
        //        StrokeThickness = StrokeThickness,
        //        ClipToPlotArea = false,
        //        TrackBallInfoTemplate = tmpTemplate,
        //    };
        //    _dataPointTemplate = Series.PointTemplate;
        //}

       
        #region Style

        private DataTemplate _dataPointTemplate;
        private static readonly DataTemplate _signalDataPointTemplate = (DataTemplate) XamlReader.Load(@"
                                            <DataTemplate 
                                                xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
                                                xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""
												xmlns:telerik=""http://schemas.telerik.com/2008/xaml/presentation\""
												xmlns:chart=""clr-namespace:Telerik.Windows.Controls.Charting;assembly=Telerik.Windows.Controls.Charting"">
                                                <Canvas Background=""AliceBlue"">
                <Path x:Name=""PART_PointMarkPath""  Fill=""{Binding DataItem.Fill}""
                                                                  Stretch=""Fill"">
                    <Path.Data>
                        <PathGeometry x:Name=""PART_PointMarkPathGeometry"" />
                    </Path.Data>
                </Path>
                <Border BorderBrush=""{Binding DataItem.Fill}"" BorderThickness=""2"" Canvas.Top=""-38"" CornerRadius=""5"" Visibility=""{Binding DataItem.Visibility}"" Background=""White"">
                    <TextBlock Text=""{Binding DataItem.Label}"" Visibility=""{Binding DataItem.Visibility}""></TextBlock>
                </Border>
                <Ellipse  Width=""8""  Canvas.Top=""-4"" Canvas.Left=""-4"" Stroke=""{Binding DataItem.Fill}"" StrokeThickness=""1"" Height=""8"" Fill=""{Binding DataItem.StrokeFill}""/>
            </Canvas>
</DataTemplate>");

        #endregion
        
    }
}