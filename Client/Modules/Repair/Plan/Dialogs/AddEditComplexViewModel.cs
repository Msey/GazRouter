using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using GazRouter.Common.ViewModel;
using GazRouter.DataProviders.Repairs;
using GazRouter.DTO.Repairs.Complexes;
using Telerik.Windows.Controls;

namespace GazRouter.Repair.Plan.Dialogs
{
    public class AddEditComplexViewModel : AddEditViewModelBase<ComplexDTO, int>
    {
        private int _defaultDateDelta = 1;
        private bool _datesChanged;

        private DateTime _allowedDateRangeStart;
        private DateTime _allowedDateRangeEnd;

        public AddEditComplexViewModel(Action<int> actionBeforeClosing, ComplexDTO model, int year)
            : base(actionBeforeClosing, model)
        {
            AllowedDateRangeStart = new DateTime(year, 1, 1);
            AllowedDateRangeEnd = new DateTime(year, 12, 31);
            ComplexName = model.ComplexName;
            EndDate = model.EndDate;
            Id = model.Id;
            StartDate = model.StartDate;
            _defaultDateDelta = (int) (EndDate - StartDate).TotalDays;

            SetValidationRules();
        }

        public AddEditComplexViewModel(Action<int> actionBeforeClosing, int year, int systemId)
            : base(actionBeforeClosing)
        {
            AllowedDateRangeStart = new DateTime(year, 1, 1);
            AllowedDateRangeEnd = new DateTime(year, 12, 31);

            Model.StartDate = new DateTime(year, 1, 1);
            Model.EndDate = StartDate.AddDays(_defaultDateDelta);
            Model.SystemId = systemId;

            SetValidationRules();
            RefreshCommands();
        }

        /// <summary>
        ///     Наименование комплекса
        /// </summary>
        public string ComplexName
        {
            get { return Model.ComplexName; }
            set
            {
                if (Model.ComplexName != value)
                {
                    Model.ComplexName = value;
                    OnPropertyChanged(() => ComplexName);
                    RefreshCommands();
                }
            }
        }

        /// <summary>
        ///     Плановая дата начала проведения ремонтных работ
        /// </summary>
        public DateTime StartDate
        {
            get { return Model.StartDate; }
            set
            {
                if (Model.StartDate == value)
                {
                    return;
                }

                Model.StartDate = value.Date;
                if (Model.StartDate == _allowedDateRangeEnd)
                {
                    Model.StartDate = Model.StartDate.AddDays(-1);
                }

                OnPropertyChanged(() => StartDate);

                if (Model.StartDate >= Model.EndDate)
                {
                    EndDate = Model.StartDate.AddDays(_defaultDateDelta) > AllowedDateRangeEnd
                        ? AllowedDateRangeEnd
                        : Model.StartDate.AddDays(_defaultDateDelta);
                }
                else
                {
                    _defaultDateDelta = (int) (EndDate - StartDate).TotalDays;
                }

                OnPropertyChanged(() => DurationString);

                _datesChanged = true;
            }
        }

        /// <summary>
        ///     Плановая дата завершения ремонтных работ
        /// </summary>
        public DateTime EndDate
        {
            get { return Model.EndDate; }
            set
            {
                if (Model.EndDate == value)
                {
                    return;
                }

                Model.EndDate = value.Date;
                if (Model.EndDate == _allowedDateRangeStart)
                {
                    Model.EndDate = Model.EndDate.AddDays(1);
                }

                OnPropertyChanged(() => EndDate);

                if (Model.EndDate <= Model.StartDate)
                {
                    StartDate = Model.EndDate.AddDays(-_defaultDateDelta) < AllowedDateRangeStart
                        ? AllowedDateRangeStart
                        : Model.EndDate.AddDays(-_defaultDateDelta);
                }
                else
                {
                    _defaultDateDelta = (int) (EndDate - StartDate).TotalDays;
                }

                OnPropertyChanged(() => DurationString);

                _datesChanged = true;
            }
        }

        /// <summary>
        ///     Начало допустимого диапазона выбора дат
        /// </summary>
        public DateTime AllowedDateRangeStart
        {
            get { return _allowedDateRangeStart; }
            set
            {
                _allowedDateRangeStart = value;
                OnPropertyChanged(() => AllowedDateRangeStart);
            }
        }

        /// <summary>
        ///     Конец допустимого диапазона выбора дат
        /// </summary>
        public DateTime AllowedDateRangeEnd
        {
            get { return _allowedDateRangeEnd; }
            set
            {
                _allowedDateRangeEnd = value;
                OnPropertyChanged(() => AllowedDateRangeEnd);
            }
        }

        public string DurationString => $"({(EndDate - StartDate).TotalDays} дн.)";

        /// <summary>
        ///     В комплекс включены ремонты
        /// </summary>
        public bool HasRepairs { get; set; }

        protected async override void UpdateCurrent()
        {
            var paramSet = new EditComplexParameterSet
            {
                Id = Model.Id,
                ComplexName = ComplexName,
                StartDate = StartDate,
                EndDate = EndDate
            };

            if (_datesChanged && HasRepairs)
            {
                var dp = new DialogParameters
                {
                    Closed = async (s, e) =>
                    {
                        if (e.DialogResult.HasValue && e.DialogResult.Value)
                            await new RepairsServiceProxy().MoveComplexAsync(paramSet);
                        else
                            await new RepairsServiceProxy().EditComplexAsync(paramSet);
                        
                        DialogResult = true;
                    },
                    Header = "Внимание",
                    Content = new TextBlock
                    {
                        Text =
                            "Изменены даты комплекса, при этом в комплекс уже включены ремонты. Изменить даты ремонтов (в т.ч. продолжительность), включенных в комплекс, таким образом, чтобы они соответствовали датам проведения комплекса?",
                        TextWrapping = TextWrapping.Wrap,
                        Width = 250
                    },
                    CancelButtonContent = "Нет",
                    OkButtonContent = "Да"
                };

                RadWindow.Confirm(dp);
            }
            else
            {
                await new RepairsServiceProxy().EditComplexAsync(paramSet);
                DialogResult = true;
            }
        }

    

        protected override Task<int> CreateTask => new RepairsServiceProxy().AddComplexAsync(
            new AddComplexParameterSet
            {
                ComplexName = ComplexName,
                StartDate = StartDate,
                EndDate = EndDate,
                IsLocal = true,
                SystemId = Model.SystemId
            });

        protected override string CaptionEntityTypeName => "комплекса";

        protected override bool OnSaveCommandCanExecute()
        {
            ValidateAll();
            return !HasErrors;
        }

        private void RefreshCommands()
        {
            SaveCommand.RaiseCanExecuteChanged();
        }

        private void SetValidationRules()
        {
            AddValidationFor(() => ComplexName)
                .When(() => string.IsNullOrEmpty(ComplexName))
                .Show("Введите наименование комплекса");
        }
    }
}