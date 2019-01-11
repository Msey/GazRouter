using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using GazRouter.Application.Helpers;
using GazRouter.Common.ViewModel;
using GazRouter.DataProviders.ManualInput;
using GazRouter.DTO.ManualInput.CompUnitTests;
using GazRouter.ManualInput.CompUnitTests.ChartDigitizer;
using Microsoft.Practices.Prism.Commands;
using System.Linq;


namespace GazRouter.ManualInput.CompUnitTests
{
    public class AddEditCompUnitTestViewModel :  AddEditViewModelBase<CompUnitTestDTO, int>
    {
        private string _testDescription;
        private DateTime _selectedDate;
        private double? _density;
        private double? _temperatureIn;
        private double? _pressureIn;
        private double? _pumpingMin;
        private double? _pumpingMax;

        

        #region constructors

        public AddEditCompUnitTestViewModel(Action<int> closeCallback, CompUnitTestDTO model)
            : base(closeCallback, model)
        {
            SelectedDate = Model.CompUnitTestDate;
            TestDescription = Model.Description;
            Density = Model.Density;
            TemperatureIn = Model.TemperatureIn;
            PressureIn = Model.PressureIn;
            PumpingMin = Model.QMin;
            PumpingMax = Model.QMax;


            Items = new List<Item>
                        {
                            new Item
                                {
                                    Name = "Степень сжатия"
                                },
                            new Item
                                {
                                    Name = "Политр КПД"
                                },
                            new Item
                                {
                                    Name = "Мощность"
                                }
                        };

            
            
            Model.ChartPoints.Where(p => p.LineType == 1)
                 .ToList()
                 .ForEach(cp => Items[0].Children.Add(new Item { X = cp.X, Y = cp.Y }));

            Model.ChartPoints.Where(p => p.LineType == 2)
                .ToList()
                .ForEach(cp => Items[1].Children.Add(new Item { X = cp.X, Y = cp.Y }));

            if (Model.ChartPoints.Any(p => p.LineType == 3))
                Model.ChartPoints.Where(p => p.LineType == 3)
                     .ToList()
                     .ForEach(cp => Items[2].Children.Add(new Item {X = cp.X, Y = cp.Y}));
            
            
            
            SetValidationRules();
            ValidateAll();
        }

        private Guid _unitId;

        public AddEditCompUnitTestViewModel(Action<int> actionBeforeClosing, Guid unitId)
            : base(actionBeforeClosing)
        {
            _unitId = unitId;
            _selectedDate = DateTime.Today;

            Items = new List<Item>
                        {
                            new Item
                                {
                                    Name = "Степень сжатия"
                                },
                            new Item
                                {
                                    Name = "Политр КПД"
                                },
                            new Item
                                {
                                    Name = "Мощность"
                                }
                        };
            
            SetValidationRules();
            ValidateAll();
        }

        #endregion


        protected override bool OnSaveCommandCanExecute()
        {
            ValidateAll();
            return !HasErrors;
        }

        protected override string CaptionEntityTypeName
        {
            get { return " испытания"; }
        }

        
        #region Properties

        /// <summary>
        /// Краткое описание испытания
        /// </summary>
        public string TestDescription
        {
            get { return _testDescription; }
            set
            {
                if(SetProperty(ref _testDescription, value))
                    SaveCommand.RaiseCanExecuteChanged();
            }
        }
        
        
        /// <summary>
        /// Плотность
        /// </summary>
        public double? Density
        {
            get { return _density; }
            set
            {
                if (SetProperty(ref _density, value))
                    SaveCommand.RaiseCanExecuteChanged();
            }
        }

        /// <summary>
        /// Температура на входе, гр. С
        /// </summary>
        public double? TemperatureIn
        {
            get { return _temperatureIn; }
            set
            {
                if (SetProperty(ref _temperatureIn, value))
                    SaveCommand.RaiseCanExecuteChanged();
            }
        }

        /// <summary>
        /// Давление на входе, кг/см2
        /// </summary>
        public double? PressureIn
        {
            get { return _pressureIn; }
            set
            {
                if (SetProperty(ref _pressureIn, value))
                    SaveCommand.RaiseCanExecuteChanged();
            }
        }

        /// <summary>
        /// Минимальный объем
        /// </summary>
        public double? PumpingMin
        {
            get { return _pumpingMin; }
            set
            {
                if (SetProperty(ref _pumpingMin, value))
                    SaveCommand.RaiseCanExecuteChanged();
            }
        }

        /// <summary>
        /// Максимальный объем
        /// </summary>
        public double? PumpingMax
        {
            get { return _pumpingMax; }
            set
            {
                if (SetProperty(ref _pumpingMax, value))
                    SaveCommand.RaiseCanExecuteChanged();
            }
        }

        public DateTime SelectedDate
        {
            get { return _selectedDate; }
            set 
            {
                if (SetProperty(ref _selectedDate, value))
                    SaveCommand.RaiseCanExecuteChanged();
            }
        }
        #endregion




        public List<Item> Items { get; set; }


        /// <summary>
        /// Добавление нового испытания
        /// </summary>
        protected override Task<int> CreateTask
        {
            get
            {
                var pSet = new AddCompUnitTestParameterSet
                {
                    CompUnitId = _unitId,
                    CompUnitTestDate = SelectedDate,
                    Description = TestDescription,
                    Qmin = PumpingMin,
                    Qmax = PumpingMax,
                    Density = Density,
                    TemperatureIn = TemperatureIn,
                    PressureIn = PressureIn
                };

                return new ManualInputServiceProxy().AddCompUnitTestAsync(pSet);
            }
        }

        /// <summary>
        /// Редактирование
        /// </summary>
        protected override Task UpdateTask
        {
            get
            {
                var pSet = new EditCompUnitTestParameterSet
                {
                    CompUnitTestId = Model.Id,
                    CompUnitId = Model.CompUnitId,
                    CompUnitTestDate = SelectedDate,
                    Description = TestDescription,
                    QMin = PumpingMin,
                    QMax = PumpingMax,
                    Density = Density,
                    TemperatureIn = TemperatureIn,
                    PressureIn = PressureIn
                };

                return new ManualInputServiceProxy().EditCompUnitTestAsync(pSet);
            }
        }


        protected void SetValidationRules()
        {
            AddValidationFor(() => SelectedDate)
                .When(() => SelectedDate > DateTime.Now)
                .Show("Дата проведения испытания не может быть больше текущей даты");

            AddValidationFor(() => Density)
                .When(() => Density < ValueRangeHelper.DensityRange.Min || Density > ValueRangeHelper.DensityRange.Max)
                .Show(ValueRangeHelper.DensityRange.ViolationMessage);

            AddValidationFor(() => PressureIn)
                .When(() => PressureIn < ValueRangeHelper.OldPressureRange.Min || PressureIn > ValueRangeHelper.OldPressureRange.Max)
                .Show(ValueRangeHelper.OldPressureRange.ViolationMessage);

            AddValidationFor(() => TemperatureIn)
                .When(() => TemperatureIn < ValueRangeHelper.OldTemperatureRange.Min || TemperatureIn > ValueRangeHelper.OldTemperatureRange.Max)
                .Show(ValueRangeHelper.OldTemperatureRange.ViolationMessage);
            
            
            AddValidationFor(() => PumpingMin)
                .When(() => PumpingMin < 0)
                .Show("Недопустимое значение. Должно быть больше 0.");

            AddValidationFor(() => PumpingMin)
                .When(() => PumpingMin > PumpingMax)
                .Show("Минимально допустимая величина объема не может быть больше максимально допустимой.");

            AddValidationFor(() => PumpingMax)
                .When(() => PumpingMax < 0)
                .Show("Недопустимое значение. Должно быть больше 0.");

            AddValidationFor(() => PumpingMax)
                .When(() => PumpingMax < PumpingMin)
                .Show("Максимально допустимая величина объема не может быть меньше минимально допустимой.");
        }

        
        public DelegateCommand DigitizeCommand
        {
            get
            {
                return new DelegateCommand(() =>
                {
                    var vm = new ChartDigitizerViewModel(null);
                    var v = new ChartDigitizerView { DataContext = vm };
                    v.ShowDialog();
                });
            }
        }

    }


    public class Item
    {
        public Item()
        {
            Children = new List<Item>();
        }

        [Display(AutoGenerateField = false)]
        public List<Item> Children { get; set; }

        public virtual string Name { get; set; }

        public double? X { get; set; }

        public double? Y { get; set; }


        public virtual bool IsExpanded
        {
            get { return true; }
        }

    }


}