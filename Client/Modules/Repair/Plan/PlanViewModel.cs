using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using GazRouter.Application;
using GazRouter.Common;
using GazRouter.Common.ViewModel;
using GazRouter.DataProviders.Repairs;
using GazRouter.DTO.Dictionaries.GasTransportSystems;
using GazRouter.DTO.Repairs.Complexes;
using GazRouter.DTO.Repairs.Plan;
using GazRouter.Repair.Dialogs;
using GazRouter.Repair.Gantt;
using GazRouter.Repair.Plan.Gantt;
using JetBrains.Annotations;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Regions;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.GanttView;
using Telerik.Windows.Controls.Scheduling;
using Telerik.Windows.Controls.GridView;
using Telerik.Windows.Core;
using Telerik.Windows.Data;
using Telerik.Windows.Diagrams.Core;
using Telerik.Windows.Documents.Spreadsheet.Model;
using Telerik.Windows.Documents.Spreadsheet.PropertySystem;
using Telerik.Windows.Documents.Fixed.Model;
using Telerik.Windows.Documents.Fixed.Model.Editing;
using Telerik.Windows.Documents.Fixed.FormatProviders.Pdf.Export;
using Telerik.Windows.Documents.Fixed.FormatProviders.Pdf;
using Telerik.Windows.Documents.Spreadsheet.FormatProviders.OpenXml.Xlsx;

using AddEditComplexView = GazRouter.Repair.Plan.Dialogs.AddEditComplexView;
using AddEditComplexViewModel = GazRouter.Repair.Plan.Dialogs.AddEditComplexViewModel;
using AddEditRepairView = GazRouter.Repair.Plan.Dialogs.AddEditRepairView;
using AddEditRepairViewModel = GazRouter.Repair.Plan.Dialogs.AddEditRepairViewModel;
using DelegateCommand = Microsoft.Practices.Prism.Commands.DelegateCommand;
using GazRouter.DTO.Repairs.Workflow;
using GazRouter.Repair.RepWorks;

namespace GazRouter.Repair.Plan
{
    [RegionMemberLifetime(KeepAlive = false)]
    public class PlanViewModel : MainViewModelBase
    {
        private int? _selectedYear;
        private GasTransportSystemDTO _selectedSystem;
        private Repair _selectedRepair;
        private Complex _selectedComplex;
        private PlanningStage _planningStage = PlanningStage.Filling;

        public PlanSchemeViewModel RepairSchemeViewModel { get; set; }

        private bool _isEditPermission;
        public bool IsEditPermission
        {
            get { return _isEditPermission; }
            set
            {
                _isEditPermission = value;
                OnPropertyChanged(() => IsEditPermission);
            }
        }
        public PlanViewModel()
        {
            MonthColorList.ColorChanged += MonthColorListOnColorChanged;
            IsEditPermission = Authorization2.Inst.IsEditable(LinkType.RepairDva);
            if (IsEditPermission == false) _ganttRange = 1;
            //
            AddRepairCommand = new DelegateCommand(AddRepair, () => IsPlanSelected && IsChangesAllowed && IsEditPermission);
            EditRepairCommand = new DelegateCommand(EditRepair, () => SelectedRepair != null && IsChangesAllowed && IsEditPermission);
            RemoveRepairCommand = new DelegateCommand(RemoveRepair, () => SelectedRepair != null && IsChangesAllowed && IsEditPermission);

            AddComplexCommand = new DelegateCommand(AddComplex,
                () => IsPlanSelected && UserProfile.Current.Site.IsEnterprise && IsChangesAllowed && IsEditPermission);
            EditComplexCommand = new DelegateCommand(EditComplex,
                () => SelectedComplex != null && UserProfile.Current.Site.IsEnterprise && IsChangesAllowed && IsEditPermission);
            RemoveComplexCommand = new DelegateCommand(RemoveComplex,
                () => SelectedComplex != null && UserProfile.Current.Site.IsEnterprise && IsChangesAllowed && IsEditPermission);
            FilterByComplexCommand = new DelegateCommand(FilterByComplex, () => SelectedComplex != null);


            RemoveFromComplexCommand = new DelegateCommand(RemoveFromComplex, () => SelectedRepair != null && IsChangesAllowed && IsEditPermission);

            _selectedYear = IsolatedStorageManager.Get<int?>("RepairPlanLastSelectedYear") ?? DateTime.Today.Year;
            _selectedSystem = SystemList.First();

           

            InitGantt();
            LoadData();

            RepairSchemeViewModel = new PlanSchemeViewModel(this);

            SetStatusCommand = new DelegateCommand<WorkStateDTO>(SetStatus);

            ExportExcelCommand = new DelegateCommand(ExportToExcel, () => RepairList!=null &&  RepairList.Count() > 0/*true*/);
            ExportGanttCommand = new DelegateCommand(ExportGanttToExcel, () => SelectedYear.HasValue && GanttTaskList != null && GanttTaskList.Count() > 0/*true*/);
        }

        public MonthToColorList MonthColorList { get; set; } = new MonthToColorList();

        private void MonthColorListOnColorChanged(object sender, EventArgs eventArgs)
        {
            foreach(Repair rep in RepairList)
            {
                rep.Dto.StartDate = rep.Dto.StartDate;
            }
            foreach (Complex com in ComplexList)
            {
                com.Dto.StartDate = com.Dto.StartDate;
            }
        }

        /// <summary>
        /// Список годов, для элемента выбора
        /// </summary>
        public IEnumerable<int> YearList => Enumerable.Range(2013, DateTime.Now.Year - 2013 + 1).Reverse();

        public int? SelectedYear
        {
            get { return _selectedYear; }
            set
            {
                if (SetProperty(ref _selectedYear, value))
                {
                    IsolatedStorageManager.Set("RepairPlanLastSelectedYear", value);
                    LoadData();
                    OnPropertyChanged(() => IsPlanSelected);


                    VisibleRange = new VisibleRange(new DateTime(SelectedYear.Value, 1, 1),
                        new DateTime(SelectedYear.Value + 1, 1, 1));
                }
            }
        }

        /// <summary>
        /// Список ГТС
        /// </summary>
        public List<GasTransportSystemDTO> SystemList => ClientCache.DictionaryRepository.GasTransportSystems;

        /// <summary>
        /// Выбранная ГТС
        /// </summary>
        public GasTransportSystemDTO SelectedSystem
        {
            get { return _selectedSystem; }
            set
            {
                if (SetProperty(ref _selectedSystem, value))
                {
                    LoadData();
                    OnPropertyChanged(() => IsPlanSelected);
                }
            }
        }


        public bool IsPlanSelected => _selectedSystem != null && _selectedYear.HasValue;


        public async void LoadData(int? repairId = null, int? complexId = null)
        {
            if (!IsPlanSelected) return;

            Lock();

            var param = new GetRepairPlanParameterSet
            {
                Year = _selectedYear.Value,
                SystemId = _selectedSystem.Id,
                
            };
            if (!UserProfile.Current.Site.IsEnterprise)
            {
                param.SiteId = UserProfile.Current.Site.Id;
            }
            

            var plan = await new RepairsServiceProxy().GetRepairPlanAsync(param);

            RepairList = plan.RepairList.Select(Repair.Create).OrderBy(r => r.Id).ToList();
            //RepairList = plan.RepairList.Select(Repair.Create).OrderBy(r => r.SortOrder).ToList();
            OnPropertyChanged(() => RepairList);

            ComplexList =
                plan.ComplexList.Select(c => new Complex(c, AddToComplex)).OrderBy(c => c.Dto.StartDate).ToList();


            // Выставляем признак ошибки у комплекса
            ComplexList.ForEach(
                c =>
                    c.HasErrors =
                        RepairList.Where(r => r.HasComplex && r.Dto.Complex.Id == c.Dto.Id)
                            .Any(r => r.Dto.StartDate < c.Dto.StartDate || r.Dto.EndDate > c.Dto.EndDate));
            OnPropertyChanged(() => ComplexList);
            OnPropertyChanged(() => LocalComplexList);
            OnPropertyChanged(() => EnterpriseComplexList);

            // Обновление ганта
            RefreshGantt(RepairList);


            // Выставляем этап планирования
            _planningStage = plan.PlanningStage.Stage;
            OnPropertyChanged(() => PlanningStage);


            RefreshCommands();

            Unlock();

            if (repairId.HasValue)
            {
                _selectedRepair = RepairList.FirstOrDefault(r => r.Id == repairId.Value);
                OnPropertyChanged(() => SelectedRepair);
            }
            if (complexId.HasValue)
            {
                _selectedComplex = ComplexList.FirstOrDefault(c => c.Dto.Id == complexId.Value);
                OnPropertyChanged(() => SelectedComplex);
            }

            RefreshCommands();
        }


        public DelegateCommand<WorkStateDTO> SetStatusCommand { get; set; }
        private List<SetStatusItem> _setStatusItemList = new List<SetStatusItem>();
        public List<SetStatusItem> SetStatusItemList
        {
            get { return _setStatusItemList; }
            set
            {
                if (SetProperty(ref _setStatusItemList, value))
                    OnPropertyChanged(() => SetStatusItemList);
            }
        }
        private void SetStatus(WorkStateDTO targetStatus)
        {
            RadWindow.Confirm(new DialogParameters
            {
                Header = "Изменение статуса ремонтных работ",
                Content = "Подтвердите пожалуйста установку статуса \"" +
                    targetStatus.Name
                + "\"",
                Closed = async (o, even) =>
                {
                    if (even.DialogResult.HasValue && even.DialogResult.Value)
                    {
                        await new RepairsServiceProxy().ChangeWorkflowStateAsync(new ChangeRepairWfParametrSet
                        {
                            RepairId = SelectedRepair.Id,
                            WFState = targetStatus.WFState,
                            WState = targetStatus.WState,
                            repair = SelectedRepair.Dto
                        });

                        LoadData(SelectedRepair.Id);
                    }
                }
            });
        }
                
        public DelegateCommand ExportExcelCommand { get; }
        public DelegateCommand ExportGanttCommand { get; }

        private void RefreshCommands()
        {
            AddRepairCommand.RaiseCanExecuteChanged();
            EditRepairCommand.RaiseCanExecuteChanged();
            RemoveRepairCommand.RaiseCanExecuteChanged();
            AddComplexCommand.RaiseCanExecuteChanged();
            EditComplexCommand.RaiseCanExecuteChanged();
            RemoveComplexCommand.RaiseCanExecuteChanged();
            FilterByComplexCommand.RaiseCanExecuteChanged();
            RemoveFromComplexCommand.RaiseCanExecuteChanged();
            ExportExcelCommand.RaiseCanExecuteChanged();
            ExportGanttCommand.RaiseCanExecuteChanged();
            RefreshStatusMenu();
        }

        private void RefreshStatusMenu()
        {
            SetStatusItemList = SelectedRepair?.Dto.WFWState.GetTransfers(UserProfile.Current.Site.IsEnterprise).Select(s => new SetStatusItem(SetStatusCommand, s)).ToList();
        }
                        
        private const string INFO_HEADER_CELL_STYLE = "InfoHeaderCellStyle";
        private const string CUSTOM_HEADER_CELL_STYLE = "CustomHeaderCellStyle";
        private const string CUSTOM_CELL_STYLE = "CustomCellStyle";
        private const string MONTH_CUSTOM_CELL_STYLE_PART = "MonthCustomCellStyle_";
        private void ExportToExcel()
        {
                    var dialog = new SaveFileDialog
                    {
                        DefaultExt = "xlsx",
                        Filter = "Excel files (*.xlsx)|*.xlsx|All files (*.*)|*.*",
                        FilterIndex = 1,
                        //DefaultFileName = Header
                    };

            if (dialog.ShowDialog() == true)
            {
                CellStyle InfoHeaderCellStyle = CellStyle.CreateTempStyle();
                InfoHeaderCellStyle.VerticalAlignment = RadVerticalAlignment.Center;
                InfoHeaderCellStyle.HorizontalAlignment = RadHorizontalAlignment.Left;
                MonthToColorList MonthColorsList = new MonthToColorList();
                Dictionary<string, CellStyle> CellStylesDic = new Dictionary<string, CellStyle>();

                var border = new CellBorder(CellBorderStyle.Thin, new ThemableColor(Colors.Black));
                CellStyle CustomHeaderCellStyle = CellStyle.CreateTempStyle();
                CustomHeaderCellStyle.VerticalAlignment = RadVerticalAlignment.Center;
                CustomHeaderCellStyle.HorizontalAlignment = RadHorizontalAlignment.Left;
                CustomHeaderCellStyle.IsBold = true;
                CustomHeaderCellStyle.IsWrapped = true;
                CustomHeaderCellStyle.BottomBorder = border;
                CustomHeaderCellStyle.TopBorder = border;
                CustomHeaderCellStyle.LeftBorder = border;
                CustomHeaderCellStyle.RightBorder = border;
                CustomHeaderCellStyle.Fill = new PatternFill(PatternType.Solid, Colors.LightGray, Colors.LightGray);

                CellStyle CustomCellStyle = CellStyle.CreateTempStyle();
                CustomCellStyle.VerticalAlignment = RadVerticalAlignment.Center;
                CustomCellStyle.HorizontalAlignment = RadHorizontalAlignment.Left;
                CustomCellStyle.BottomBorder = border;
                CustomCellStyle.TopBorder = border;
                CustomCellStyle.LeftBorder = border;
                CustomCellStyle.RightBorder = border;
                CustomCellStyle.IsWrapped = true;
                CustomCellStyle.Format = new CellValueFormat("@");

                for (int i = 1; i < 13; i++)
                {
                    CellStyle MonthCellStyle = CellStyle.CreateTempStyle();
                    MonthCellStyle.VerticalAlignment = RadVerticalAlignment.Center;
                    MonthCellStyle.HorizontalAlignment = RadHorizontalAlignment.Center;
                    MonthCellStyle.BottomBorder = border;
                    MonthCellStyle.TopBorder = border;
                    MonthCellStyle.LeftBorder = border;
                    MonthCellStyle.RightBorder = border;
                    MonthCellStyle.IsWrapped = true;
                    MonthCellStyle.Format = new CellValueFormat("@");
                    Color C = MonthColorsList.GetColor(i);
                    MonthCellStyle.Fill = new PatternFill(PatternType.Solid, C, C);
                    CellStylesDic.Add($"{MONTH_CUSTOM_CELL_STYLE_PART}{i}", MonthCellStyle);
                }

                CellStylesDic.Add(INFO_HEADER_CELL_STYLE, InfoHeaderCellStyle);
                CellStylesDic.Add(CUSTOM_HEADER_CELL_STYLE, CustomHeaderCellStyle);
                CellStylesDic.Add(CUSTOM_CELL_STYLE, CustomCellStyle);
                var excelReport = new ExcelReport(CellStylesDic, "ППР");
                var date = DateTime.Now;
                excelReport.WriteCell("Дата:", INFO_HEADER_CELL_STYLE).WriteCell(date.Date, INFO_HEADER_CELL_STYLE).NewRow();
                excelReport.WriteCell("Время:", INFO_HEADER_CELL_STYLE).WriteCell(date.ToString("HH:mm"), INFO_HEADER_CELL_STYLE).NewRow();
                excelReport.WriteCell("ФИО:", INFO_HEADER_CELL_STYLE).WriteCell(UserProfile.Current.UserName, INFO_HEADER_CELL_STYLE).NewRow();
                excelReport.WriteCell("План ремонтных работ:", INFO_HEADER_CELL_STYLE).WriteCell(_selectedSystem.Name + ", " + _selectedYear, INFO_HEADER_CELL_STYLE).NewRow();
                excelReport.NewRow();
                int DataTableFirstRow = excelReport.currentRow;
                excelReport.WriteHeader("Влияет на транспортировку", 150, CUSTOM_HEADER_CELL_STYLE);
                excelReport.WriteHeader("Газопровод", 175, CUSTOM_HEADER_CELL_STYLE);
                excelReport.WriteHeader("Объект", 175, CUSTOM_HEADER_CELL_STYLE);
                excelReport.WriteHeader("Работы", 250, CUSTOM_HEADER_CELL_STYLE);
                excelReport.WriteHeader("ЛПУ", 100, CUSTOM_HEADER_CELL_STYLE);
                excelReport.WriteHeader("Вид", 120, CUSTOM_HEADER_CELL_STYLE);
                excelReport.WriteHeader("Начало", 65, CUSTOM_HEADER_CELL_STYLE);
                excelReport.WriteHeader("Окончание", 65, CUSTOM_HEADER_CELL_STYLE);
                excelReport.WriteHeader("Длительность", 65, CUSTOM_HEADER_CELL_STYLE);
                excelReport.WriteHeader("Дата поставки МТР", 80, CUSTOM_HEADER_CELL_STYLE);
                excelReport.WriteHeader("Описание", 175, CUSTOM_HEADER_CELL_STYLE);
                excelReport.WriteHeader("Технологический коридор", 125, CUSTOM_HEADER_CELL_STYLE);
                excelReport.WriteHeader("Комплекс", 120, CUSTOM_HEADER_CELL_STYLE);
                excelReport.WriteHeader("Способ ведения работ", 100, CUSTOM_HEADER_CELL_STYLE);
                excelReport.WriteHeader("Объем стравливаемого газа, млн.м³", 75, CUSTOM_HEADER_CELL_STYLE);
                excelReport.WriteHeader("Объем выработанного газа, млн.м³", 75, CUSTOM_HEADER_CELL_STYLE);
                excelReport.WriteHeader("Достигнутый объем транспорта газа на участке, млн.м³/сут", 130, CUSTOM_HEADER_CELL_STYLE);
                excelReport.WriteHeader("Расчетная пропускная способность участка, млн.м³/сут", 130, CUSTOM_HEADER_CELL_STYLE);
                excelReport.WriteHeader("Расчетный объем транспорта газа на период проведения работ, млн.м³/сут", 130, CUSTOM_HEADER_CELL_STYLE);
                excelReport.WriteHeader("Примечание", 400, CUSTOM_HEADER_CELL_STYLE);
                //excelReport.WriteHeader("Изменено", 200,CUSTOM_HEADER_CELL_STYLE);
                excelReport.WriteHeader("Статус согласования работ", 120, CUSTOM_HEADER_CELL_STYLE);
                excelReport.WriteHeader("Стутус ведения работ", 100, CUSTOM_HEADER_CELL_STYLE);
                int DataTableLastColumn = excelReport.currentColumn;
                foreach (Repair r in RepairList)
                {
                    excelReport.NewRow();
                    excelReport.WriteCell(r.Dto.IsCritical ? "Да" : "Нет", CUSTOM_CELL_STYLE, false).SetHorizontaAlignment(RadHorizontalAlignment.Center).MoveNextCell();
                    excelReport.WriteCell(r.PipelineName, CUSTOM_CELL_STYLE);
                    excelReport.WriteCell(r.ObjectName, CUSTOM_CELL_STYLE);
                    excelReport.WriteCell(GetWorksText(r), CUSTOM_CELL_STYLE);
                    excelReport.WriteCell(r.Dto.SiteName, CUSTOM_CELL_STYLE);
                    excelReport.WriteCell(r.RepairTypeName, CUSTOM_CELL_STYLE);
                    excelReport.WriteCell(r.Dto.StartDate.ToString("dd.MM"), $"{MONTH_CUSTOM_CELL_STYLE_PART}{r.Dto.StartDate.Month}");
                    excelReport.WriteCell(r.Dto.EndDate.ToString("dd.MM"), $"{MONTH_CUSTOM_CELL_STYLE_PART}{r.Dto.EndDate.Month}");
                    excelReport.WriteCell(GetTimeSpanHouresText(r.DurationPlan), CUSTOM_CELL_STYLE, false).SetHorizontaAlignment(RadHorizontalAlignment.Center).MoveNextCell();
                    excelReport.WriteCell(r.Dto.PartsDeliveryDate.ToString("dd.MM"), $"{MONTH_CUSTOM_CELL_STYLE_PART}{r.Dto.PartsDeliveryDate.Month}");
                    excelReport.WriteCell(r.Dto.Description, CUSTOM_CELL_STYLE);
                    excelReport.WriteCell(r.Dto.PipelineGroupName, CUSTOM_CELL_STYLE);
                    excelReport.WriteCell(r.Dto.Complex == null ? "" : r.Dto.Complex.ComplexName, CUSTOM_CELL_STYLE);
                    excelReport.WriteCell(r.ExecutionMeansName, CUSTOM_CELL_STYLE);
                    excelReport.WriteCell(r.Dto.BleedAmount, CUSTOM_CELL_STYLE);
                    excelReport.WriteCell(r.Dto.SavingAmount, CUSTOM_CELL_STYLE);
                    excelReport.WriteCell($"Зима: {r.Dto.MaxTransferWinter}\nЛето: {r.Dto.MaxTransferSummer}\nМежсезонье: {r.Dto.MaxTransferTransition}", CUSTOM_CELL_STYLE);
                    excelReport.WriteCell($"Зима: {r.Dto.CapacityWinter}\nЛето: {r.Dto.CapacitySummer}\nМежсезонье: {r.Dto.CapacityTransition}", CUSTOM_CELL_STYLE);
                    excelReport.WriteCell(r.Dto.CalculatedTransfer, CUSTOM_CELL_STYLE);
                    excelReport.WriteCell(r.Dto.DescriptionGtp, CUSTOM_CELL_STYLE);
                    //excelReport.WriteCell(String.Format("{0:dd.MM.yyyy} {0:HH:mm}", r.Dto.LastUpdateDate) + "\n" + r.Dto.UserName + "\n" + r.Dto.UserSiteName, CUSTOM_CELL_STYLE);
                    excelReport.WriteCell(r.WorkflowState, CUSTOM_CELL_STYLE);
                    excelReport.WriteCell(r.RepairState, CUSTOM_CELL_STYLE);
                }

                int DataTableLastRow = excelReport.currentRow;
                excelReport.SetAutoFilterRange(DataTableFirstRow, 0, DataTableLastRow, DataTableLastColumn);
                excelReport.NewRow();
                excelReport.NewRow();
                CellSelection MergedCells;
                if (excelReport.MergeCells(2, 0, out MergedCells))
                {
                    MergedCells.SetStyleName(CUSTOM_HEADER_CELL_STYLE);
                    MergedCells.SetValue("Легенда");
                    MergedCells.SetHorizontalAlignment(RadHorizontalAlignment.Center);
                    excelReport.NewRow();
                }
                for (int i = 1; i < 13; i++)
                {
                    excelReport.WriteCell(i, CUSTOM_CELL_STYLE,false).SetHorizontaAlignment(RadHorizontalAlignment.Right).MoveNextCell();
                    excelReport.WriteCell(System.Globalization.DateTimeFormatInfo.CurrentInfo.MonthNames[i - 1], CUSTOM_CELL_STYLE, false).SetHorizontaAlignment(RadHorizontalAlignment.Center).MoveNextCell();
                    excelReport.WriteCell("", CUSTOM_CELL_STYLE, false).SetSolidFill(MonthColorsList.GetColor(i));
                    excelReport.NewRow();
                }

                using (var stream = dialog.OpenFile())
                    excelReport.Save(stream);
            }
        }
        private string GetTimeSpanHouresText(TimeSpan timeSpan)
        {
            var result = "";
            if (timeSpan.Hours > 0 || timeSpan.Days > 0)
                result += ((int)timeSpan.TotalHours).ToString() + "ч. ";
            if (timeSpan.Minutes > 0)
                result += timeSpan.ToString("mm'м.'");

            return result;
        }
        private string GetWorksText(Repair Context)
        {
            if (Context is PipelineRepair)
            {
                List<string> WorksText = new List<string>();
                foreach (var work in Context.Dto.Works)
                {
                    string str = "";
                    if (work.KilometerStart.HasValue)
                        str += work.KilometerStart;
                    if (work.KilometerStart.HasValue && work.KilometerEnd.HasValue)
                        str += " - " + work.KilometerEnd+" ";
                    else
                        if (work.KilometerEnd.HasValue)
                        str += work.KilometerEnd +" ";
                    str += work.WorkTypeName;
                    WorksText.Add(str);
                }
                return String.Join("\n", WorksText);
            }
            else
                return String.Join("\n", Context.Dto.Works.Select(w => w.WorkTypeName));

        }

        /// <summary>
        /// Экспорт диаграммы Ганта в эксель.
        /// </summary>
        private async void ExportGanttToExcel()
        {
            var dialog = new SaveFileDialog
            {
                DefaultExt = "xlsx",
                Filter = "Excel files (*.xlsx)|*.xlsx|All files (*.*)|*.*",
                FilterIndex = 1,
                //DefaultFileName = Header
            };

            if (dialog.ShowDialog() == true)
            {
                Lock();
                try
                {
                    Workbook workbook = await Task.Factory.StartNew(CreateWorkBook);

                    // Сохранение в файл
                    using (var stream = dialog.OpenFile())
                        new XlsxFormatProvider().Export(workbook, stream);
                }
                finally
                {
                    Unlock();
                }
            }
        }

        private Workbook CreateWorkBook()
        {
            var workbook = new Workbook();
            var worksheet = workbook.Worksheets.Add();
            int currYear = SelectedYear.Value;
            worksheet.Name = $"ППР {currYear}";
            int currColIndex = 0;
            CellBorder DefaultBorder = new CellBorder(CellBorderStyle.Thin, new ThemableColor(Colors.Black));
            CellBorders DefaultBorders = new CellBorders(DefaultBorder, DefaultBorder, DefaultBorder, DefaultBorder, DefaultBorder, DefaultBorder, CellBorder.Default, CellBorder.Default);
            CellSelection Selection;

            // Отрисовка верхней шапки
            worksheet.Rows[0].SetHeight(new RowHeight(30, true));
            worksheet.Rows[1].SetHeight(new RowHeight(30, true));
            worksheet.Columns[currColIndex].SetWidth(new ColumnWidth(400, true));
            for (int i = 1; i < 13; i++)
            {
                int days = DateTime.DaysInMonth(currYear, i);
                int startIndex = currColIndex + 1;
                for (int d = 1; d <= days; d++)
                {
                    currColIndex++;
                    worksheet.Columns[currColIndex].SetWidth(new ColumnWidth(5, true));
                    worksheet.Columns[currColIndex + 1].SetWidth(new ColumnWidth(5, true));
                    worksheet.Columns[currColIndex + 2].SetWidth(new ColumnWidth(20, true));
                    Selection = worksheet.Cells[1, currColIndex, 1, currColIndex + 2];
                    Selection.Merge();
                    Selection.SetHorizontalAlignment(RadHorizontalAlignment.Center);
                    Selection.SetVerticalAlignment(RadVerticalAlignment.Center);
                    Selection.SetValueAsText(d.ToString("0#"));
                    Selection.SetFill(PatternFill.CreateSolidFill(Colors.LightGray));
                    currColIndex += 2;
                }
                Selection = worksheet.Cells[0, startIndex, 0, currColIndex];
                Selection.Merge();
                Selection.SetValueAsText(System.Globalization.DateTimeFormatInfo.CurrentInfo.MonthNames[i - 1]);
                Selection.SetHorizontalAlignment(RadHorizontalAlignment.Center);
                Selection.SetFill(PatternFill.CreateSolidFill(Colors.LightGray));
            }
            worksheet.Cells[0, 1, 1, currColIndex].SetBorders(DefaultBorders);

            int currRowIndex = 1;
            Selection = worksheet.Cells[currRowIndex, 0];
            Selection.SetValueAsText("Объекты");
            Selection.SetFill(PatternFill.CreateSolidFill(Colors.LightGray));
            Selection.SetHorizontalAlignment(RadHorizontalAlignment.Center);
            Selection.SetVerticalAlignment(RadVerticalAlignment.Center);
            //

            // Отрисовка строк диаграммы
            DateTime FirstDate = new DateTime(currYear, 1, 1);
            DateTime LastDate = new DateTime(currYear, 12, 31);
            bool IsEven = false;
            int firstSaturdayInYear = 0;
            while (FirstDate.AddDays(firstSaturdayInYear).DayOfWeek != DayOfWeek.Saturday)
                firstSaturdayInYear++;
            foreach (var GanttTask in GanttTaskList)
            {
                AddTaskRow(worksheet, ref currRowIndex, currColIndex, GanttTask, 0, FirstDate, LastDate, IsEven, firstSaturdayInYear);
                IsEven = !IsEven;
                foreach (var Child in GanttTask.Children)
                {
                    AddTaskRow(worksheet, ref currRowIndex, currColIndex, Child, 1, FirstDate, LastDate, IsEven, firstSaturdayInYear);
                    IsEven = !IsEven;
                }
            }
            //

            // Отрисовка границ ячеек
            worksheet.Cells[new CellIndex(1, 0), new CellIndex(currRowIndex, 0)].SetBorders(DefaultBorders);
            worksheet.Cells[new CellIndex(2, 1), new CellIndex(currRowIndex, currColIndex)].SetBorders(new CellBorders(DefaultBorder, DefaultBorder, DefaultBorder, DefaultBorder, CellBorder.Default, CellBorder.Default, CellBorder.Default, CellBorder.Default));
            //
            
            // Отрисовка легенды
            currRowIndex += 2;
            worksheet.Rows[currRowIndex].SetHeight(new RowHeight(30, true));
            Selection = worksheet.Cells[currRowIndex, 0];
            Selection.SetHorizontalAlignment(RadHorizontalAlignment.Right);
            Selection.SetVerticalAlignment(RadVerticalAlignment.Center);
            Selection.SetValueAsText("ЛЕГЕНДА:");
            Selection = worksheet.Cells[currRowIndex, 1 + 3 * 1];
            Selection.SetFill(PatternFill.CreateSolidFill(Colors.Orange));
            Selection = worksheet.Cells[currRowIndex, 3 + 3 * 1];
            Selection.SetHorizontalAlignment(RadHorizontalAlignment.Left);
            Selection.SetVerticalAlignment(RadVerticalAlignment.Center);
            Selection.SetValueAsText(" Даты начала и окончания комплекса");
            currRowIndex++;
            worksheet.Rows[currRowIndex].SetHeight(new RowHeight(10, true));
            currRowIndex++;
            worksheet.Rows[currRowIndex].SetHeight(new RowHeight(30, true));
            Selection = worksheet.Cells[currRowIndex, 1 + 3 * 1];
            Selection.SetFill(PatternFill.CreateSolidFill(Colors.Red));
            Selection = worksheet.Cells[currRowIndex, 3 + 3 * 1];
            Selection.SetHorizontalAlignment(RadHorizontalAlignment.Left);
            Selection.SetVerticalAlignment(RadVerticalAlignment.Center);
            Selection.SetValueAsText(" Дата поставки МТР");
            //

            // Установка фиксированных областей (не участвующих в прокрутке листа)
            worksheet.ViewState.FreezePanes(2, 1);
            return workbook;
        }

        /// <summary>
        /// //Отрисовка строки диаграммы Ганта
        /// </summary>
        /// <param name="worksheet">Лиск экселя с которым, но котором рисуем</param>
        /// <param name="currRowIndex">Посленяя строка, на которой осуществлялась отрисовка</param>
        /// <param name="lastColIndex">Индекс поледней колонки для строки</param>
        /// <param name="Task">Контекст данных для отрисовки</param>
        /// <param name="outlineLevel">Уровень группировки. 0 - группа, 1 - ремонтная работа.</param>
        /// <param name="FirstDate">Начальная дата диаграммы</param>
        /// <param name="LastDate">Конечная дата диаграммы</param>
        /// <param name="IsEven">Чётная/Нечётная строка</param>
        private void AddTaskRow(Worksheet worksheet, ref int currRowIndex, int lastColIndex, IGanttTask Task, int outlineLevel, DateTime FirstDate, DateTime LastDate, bool IsEven, int firstSaturdayInYear)
        {
            currRowIndex++;
            for (int r = 0; r < 5; r++)
            {
                var Row = worksheet.Rows[currRowIndex+r];
                Row.SetHeight(new RowHeight(8, true));
                Row.SetOutlineLevel(outlineLevel);
            }
            CellSelection Selection = worksheet.Cells[currRowIndex, 0, currRowIndex+4, 0];
            Selection.Merge();
            Selection.SetIsWrapped(true);
            Selection.SetValueAsText((outlineLevel==0?"":"     ") +Task.Title);
            Selection.SetVerticalAlignment(RadVerticalAlignment.Center);
            Selection = worksheet.Cells[currRowIndex , 1, currRowIndex+4, lastColIndex];
            Selection.SetFill(PatternFill.CreateSolidFill(IsEven ? Color.FromArgb(0xFF,0xFF,0xF0,0xF5) : Colors.White));
            int totalDays = (LastDate - FirstDate).Days;
            totalDays++;
            for (int s= firstSaturdayInYear; s< totalDays;s+=7)
            {
                Selection = worksheet.Cells[currRowIndex, s*3+1, currRowIndex + 4, (s+1)<totalDays? (s+1)*3+3 : s*3+3];
                Selection.SetFill(PatternFill.CreateSolidFill(IsEven ? Color.FromArgb(0xFF, 0xE0, 0xD1, 0xD6) : Color.FromArgb(0xFF, 0xE0, 0xE0, 0xE0)));
            }
            DateTime RoundedStartDate = new DateTime(Task.Start.Year, Task.Start.Month, Task.Start.Day);
            DateTime RoundeEndDate = new DateTime(Task.End.Year, Task.End.Month, Task.End.Day);
            int start = ((RoundedStartDate < FirstDate ? FirstDate : RoundedStartDate) - FirstDate).Days * 3 + 1;
            int end = ((RoundeEndDate > LastDate ? LastDate : RoundeEndDate) - FirstDate).Days* 3 + 1;
            int delta = 2- outlineLevel;
            Selection = worksheet.Cells[currRowIndex+ delta, start, currRowIndex+4-delta, end];
            Selection.SetFill(PatternFill.CreateSolidFill(outlineLevel == 0 ? Color.FromArgb(0xFF,0x32,0xCD,0x32) : Color.FromArgb(0xFF, 0x46, 0x82, 0xB4)));

            //Выставление отметок о датах начала/завершения комплекса и поставки МТР.
            if (Task is RepairTask)
            {
                var RepTask = Task as RepairTask;
                int complexStartCol=0;
                int complexEndCol = 0;
                if (RepTask.RepairItem.HasComplex)
                {
                    DateTime RoundedComplexStartDate = new DateTime(RepTask.RepairItem.Dto.Complex.StartDate.Year, RepTask.RepairItem.Dto.Complex.StartDate.Month, RepTask.RepairItem.Dto.Complex.StartDate.Day);
                    DateTime RoundedComplexEndDate = new DateTime(RepTask.RepairItem.Dto.Complex.EndDate.Year, RepTask.RepairItem.Dto.Complex.EndDate.Month, RepTask.RepairItem.Dto.Complex.EndDate.Day);
                    if (RoundedComplexStartDate >= FirstDate && RoundedComplexStartDate <= LastDate)
                    {
                        complexStartCol = (RoundedComplexStartDate - FirstDate).Days * 3 + 1;
                        worksheet.Cells[currRowIndex, complexStartCol, currRowIndex + 4, complexStartCol].SetFill(PatternFill.CreateSolidFill(Colors.Orange));
                    }
                    if (RoundedComplexEndDate >= FirstDate && RoundedComplexEndDate <= LastDate)
                    {
                        complexEndCol = (RoundedComplexEndDate - FirstDate).Days * 3 + 1;
                        worksheet.Cells[currRowIndex, complexEndCol, currRowIndex + 4, complexEndCol].SetFill(PatternFill.CreateSolidFill(Colors.Orange));
                    }
                }

                DateTime RoundedPartsDeliveryDate = new DateTime(RepTask.RepairItem.Dto.PartsDeliveryDate.Year, RepTask.RepairItem.Dto.PartsDeliveryDate.Month, RepTask.RepairItem.Dto.PartsDeliveryDate.Day);
                if (RoundedPartsDeliveryDate >= FirstDate && RoundedPartsDeliveryDate <= LastDate)
                {
                    int partsCol = (RoundedPartsDeliveryDate - FirstDate).Days * 3 + 1;
                    if (partsCol == complexStartCol || partsCol == complexEndCol) partsCol++;
                    worksheet.Cells[currRowIndex, partsCol, currRowIndex + 4, partsCol].SetFill(PatternFill.CreateSolidFill(Colors.Red));
                }
            }
            //
            currRowIndex += 4;
        }

        #region REPAIRS

        private bool _isTableVisible;

        /// <summary>
        /// Активна вкладка с таблицей
        /// </summary>
        public bool IsTableActive
        {
            get { return _isTableVisible; }
            set { SetProperty(ref _isTableVisible, value); }
        }

        /// <summary>
        /// Список ремонтных работ
        /// </summary>
        public List<Repair> RepairList { get; set; }


        /// <summary>
        /// Выбранный ремонт
        /// </summary>
        public Repair SelectedRepair
        {
            get { return _selectedRepair; }
            set
            {
                if (SetProperty(ref _selectedRepair, value))
                {
                    RefreshCommands();
                    if (_selectedRepair != null && _selectedRepair.HasComplex)
                        SelectedComplex = ComplexList.SingleOrDefault(c => c.Dto.Id == _selectedRepair.Dto.Complex?.Id);
                }
            }
        }


        public DelegateCommand AddRepairCommand { get; }
        public DelegateCommand EditRepairCommand { get; }
        public DelegateCommand RemoveRepairCommand { get; }


        private void AddRepair()
        {
            var viewModel = new AddEditRepairViewModel(id => LoadData(id, null), SelectedYear.Value);
            var view = new AddEditRepairView {DataContext = viewModel};
            view.ShowDialog();
        }

        private void EditRepair()
        {
            var viewModel = new AddEditRepairViewModel(id => LoadData(id, null), SelectedRepair.Dto, SelectedYear.Value);
            var view = new AddEditRepairView {DataContext = viewModel};
            view.ShowDialog();
        }

        private void RemoveRepair()
        {
            var dp = new DialogParameters
            {
                Closed = async (s, e) =>
                {
                    if (!e.DialogResult.HasValue || !e.DialogResult.Value) return;
                    await new RepairsServiceProxy().DeleteRepairAsync(SelectedRepair.Id);
                    LoadData();
                },
                Content = "Вы уверены что хотите удалить ремонт?",
                Header = "Удаление ремонта",
                OkButtonContent = "Да",
                CancelButtonContent = "Нет"
            };

            RadWindow.Confirm(dp);
        }

        internal async void ChangeRepairDates(Repair repair, DateTime start, DateTime end)
        {
            await new RepairsServiceProxy().EditRepairDatesAsync(
                new EditRepairDatesParameterSet
                {
                    DateStart = start,
                    DateEnd = end,
                    DateType = DateTypes.Plan,
                    RepairId = repair.Id
                });
            LoadData(repair.Id);
        }

        #endregion

        #region COMPLEXES

        /// <summary>
        /// Список комплексов
        /// </summary>
        public List<Complex> ComplexList { get; set; }

        /// <summary>
        /// Список локальных комплексов
        /// </summary>
        public List<Complex> LocalComplexList => ComplexList.Where(c => c.Dto.IsLocal).ToList();

        /// <summary>
        /// Список комплексов ГАЗПРОМ
        /// </summary>
        public List<Complex> EnterpriseComplexList => ComplexList.Where(c => !c.Dto.IsLocal).ToList();


        /// <summary>
        /// Выбранный комплекс
        /// </summary>
        public Complex SelectedComplex
        {
            get { return _selectedComplex; }
            set
            {
                if (SetProperty(ref _selectedComplex, value))
                    RefreshCommands();
            }
        }


        public DelegateCommand AddComplexCommand { get; }
        public DelegateCommand EditComplexCommand { get; }
        public DelegateCommand RemoveComplexCommand { get; }


        private void AddComplex()
        {
            var viewModel = new AddEditComplexViewModel(id => LoadData(SelectedRepair?.Id, id), SelectedYear.Value,
                SelectedSystem.Id);
            var view = new AddEditComplexView {DataContext = viewModel};
            view.ShowDialog();
        }

        private void RemoveComplex()
        {
            var msg = string.Empty;

            if (RepairList.Any(r => r.Dto.Complex?.Id == SelectedComplex.Dto.Id))
                msg +=
                    "Вы пытаетесь удалить комплекс, в котором есть работы. При удалении такого комплекса все работы, входящие в него, будут исключены из этого комплекса. ";

            msg += "Вы уверены что хотите удалить комплекс?";
            RadWindow.Confirm(
                new DialogParameters
                {
                    Closed = async (s, e) =>
                    {
                        if (!e.DialogResult.HasValue || !e.DialogResult.Value) return;
                        await new RepairsServiceProxy().DeleteComplexAsync(SelectedComplex.Dto.Id);
                        LoadData(SelectedRepair?.Id, null);
                    },
                    Content = new TextBlock
                    {
                        Text = msg,
                        TextWrapping = TextWrapping.Wrap,
                        Width = 250
                    },
                    Header = "Удаление комплекса",
                    OkButtonContent = "Да",
                    CancelButtonContent = "Нет"
                });
        }

        private void EditComplex()
        {
            var viewModel = new AddEditComplexViewModel(id => LoadData(SelectedRepair?.Id, id), SelectedComplex.Dto,
                SelectedYear.Value)
            {
                HasRepairs = RepairList.Any(r => r.Dto.Complex?.Id == SelectedComplex.Dto.Id)
            };
            var view = new AddEditComplexView {DataContext = viewModel};
            view.ShowDialog();
        }
        

        
        internal void MoveComplex(Complex complex, DateTime start, DateTime end)
        {
            // Дата поставки МТР не согласуется с датой начала комплекса
            if (
                RepairList.Where(r => r.HasComplex)
                    .Any(r => r.Dto.Complex.Id == complex.Dto.Id && start < r.Dto.PartsDeliveryDate))
            {
                MessageBoxProvider.Alert(
                    "Комплекс невозможно передвинуть, т.к. даты поставки МТР одного или нескольких ремонтов включенных в комплекс не согласуются с новыми датами проведения комплекса.",
                    "Ошибка");
                return;
            }

            new RepairsServiceProxy().MoveComplexAsync(new EditComplexParameterSet
            {
                Id = complex.Dto.Id,
                ComplexName = complex.Dto.ComplexName,
                IsLocal = complex.Dto.IsLocal,
                SystemId = complex.Dto.SystemId,
                StartDate = start,
                EndDate = end
            });

            LoadData(SelectedRepair.Id);
        }

        // Вот этот метод вызывается командой добавления работы в комплекс
        private void AddToComplex(Complex complex)
        {
            // Если выбран тот же комплекс
            if (SelectedRepair.HasComplex && SelectedRepair.Dto.Complex.Id == complex.Dto.Id) return;

            // При попытке включить в комплекс, если работа уже включена в другой комплекс, 
            // просто сообщаем об этом и ничего не делаем
            if (SelectedRepair.HasComplex)
            {
                MessageBoxProvider.Alert(
                    "Внимание!Ремонт уже включен в другой комплекс.Сначала работу нужно исключить из комплекса, а затем включать в новый комплекс.",
                    "Ограничение по дате поставки МТР");
                return;
            }

            // Дата поставки МТР не согласуется с датой начала комплекса
            if (SelectedRepair.Dto.PartsDeliveryDate > complex.Dto.EndDate)
            {
                MessageBoxProvider.Alert(
                    "Внимание!Ремонт нельзя добавить в выбранный комплекс, так как даты проведения комплекса не согласуются с датой поставки МТР.",
                    "Ремонт уже включен в комплекс");
                return;
            }

            if (complex.Dto.IsLocal)
            {
                AddToLocalComplex(complex);
            }
            else
            {
                AddToEnterpriseComplex(complex);
            }
        }


        private async void AddToLocalComplex(Complex complex)
        {
            if (SelectedRepair.Dto.StartDate < complex.Dto.StartDate || SelectedRepair.Dto.EndDate > complex.Dto.EndDate)
            {
                var msg = "Внимание! Вы добавляете ремонт в комплекс, при этом сроки проведения ремонта не соответсвуют срокам проведения комплекса."
                          + Environment.NewLine + Environment.NewLine
                          + "Что делать?";

                var answerList = new List<Answer>
                {
                    new Answer {Num = 1, Text = "Только добавить"},
                    new Answer {Num = 2, Text = "Добавить и изменить даты ремонта"},
                    new Answer {Num = 3, Text = "Добавить и изменить даты комплекса"},
                    new Answer {Num = 4, Text = "Не добавлять"}
                };

                var vm = new QuestionViewModel(
                    async num =>
                    {
                        switch (num)
                        {
                            case 1:
                                await new RepairsServiceProxy().AddRepairToComplexAsync(
                                    new AddRepairToComplexParameterSet
                                    {
                                        ComplexId = complex.Dto.Id,
                                        RepairId = SelectedRepair.Id
                                    });
                                LoadData(SelectedRepair.Id);
                                break;

                            case 2:
                            {
                                // Изменить даты ремонта
                                var start = SelectedRepair.Dto.StartDate >= complex.Dto.StartDate &&
                                            SelectedRepair.Dto.StartDate < complex.Dto.EndDate
                                    ? SelectedRepair.Dto.StartDate
                                    : complex.Dto.StartDate;

                                var end = SelectedRepair.Dto.EndDate > complex.Dto.StartDate &&
                                          SelectedRepair.Dto.EndDate <= complex.Dto.EndDate
                                    ? SelectedRepair.Dto.EndDate
                                    : complex.Dto.EndDate;

                                await new RepairsServiceProxy().EditRepairDatesAsync(
                                    new EditRepairDatesParameterSet
                                    {
                                        DateStart = start,
                                        DateEnd = end,
                                        DateType = DateTypes.Plan,
                                        RepairId = SelectedRepair.Id
                                    });

                                    // Добавить ремонт в комплекс
                                await new RepairsServiceProxy().AddRepairToComplexAsync(
                                    new AddRepairToComplexParameterSet
                                    {
                                        ComplexId = complex.Dto.Id,
                                        RepairId = SelectedRepair.Id
                                    });

                                    LoadData(SelectedRepair.Id);
                                }
                                break;

                            case 3:
                            {
                                var start = SelectedRepair.Dto.StartDate;
                                var end = SelectedRepair.Dto.EndDate - SelectedRepair.Dto.StartDate >
                                          complex.Dto.EndDate - complex.Dto.StartDate
                                    ? SelectedRepair.Dto.EndDate
                                    : start + (complex.Dto.EndDate - complex.Dto.StartDate);

                                MoveComplex(complex, start, end);

                                // Добавить ремонт в комплекс
                                await new RepairsServiceProxy().AddRepairToComplexAsync(
                                    new AddRepairToComplexParameterSet
                                    {
                                        ComplexId = complex.Dto.Id,
                                        RepairId = SelectedRepair.Id
                                    });
                                LoadData(SelectedRepair.Id);
                                break;
                            }
                        }
                    })
                {
                    Header = "Добавление ремонта в комплекс",
                    Question = msg,
                    AnswerList = answerList
                };

                var view = new QuestionView {DataContext = vm};
                view.ShowDialog();
            }
            else
            {
                await new RepairsServiceProxy().AddRepairToComplexAsync(
                    new AddRepairToComplexParameterSet
                    {
                        ComplexId = complex.Dto.Id,
                        RepairId = SelectedRepair.Id
                    });
                LoadData(SelectedRepair.Id);
            }
        }


        private async void AddToEnterpriseComplex(Complex complex)
        {
            // Если комплекс ПАО Газпром и даты ремонта не соответсвуют датам проведения комплекса - изменяем даты ремонта
            if (SelectedRepair.Dto.StartDate < complex.Dto.StartDate || SelectedRepair.Dto.EndDate > complex.Dto.EndDate)
            {
                var msg = "Вы добавляете ремонт в комплекс ПАО \"Газпром\". "
                          + "При этом сроки проведения ремонта не соответсвуют срокам проведения комплекса. "
                          + "В случае добавления дата начала ремонта и при необходимости его продолжительность "
                          + "будут автоматически изменены в соответсвии со сроками проведения комплекса."
                          + Environment.NewLine + Environment.NewLine
                          + "Добавить ремонт в комплекс?";

                RadWindow.Confirm(
                    new DialogParameters
                    {
                        Closed = async (s, e) =>
                        {
                            if (!e.DialogResult.HasValue || !e.DialogResult.Value)
                            {
                                return;
                            }

                            // Изменить даты ремонта
                            var start = SelectedRepair.Dto.StartDate >= complex.Dto.StartDate &&
                                        SelectedRepair.Dto.StartDate < complex.Dto.EndDate
                                ? SelectedRepair.Dto.StartDate
                                : complex.Dto.StartDate;
                            var end = SelectedRepair.Dto.EndDate > complex.Dto.StartDate &&
                                      SelectedRepair.Dto.EndDate <= complex.Dto.EndDate
                                ? SelectedRepair.Dto.EndDate
                                : complex.Dto.EndDate;
                            await new RepairsServiceProxy().EditRepairDatesAsync(
                                new EditRepairDatesParameterSet
                                {
                                    DateStart = start,
                                    DateEnd = end,
                                    DateType = DateTypes.Plan,
                                    RepairId = SelectedRepair.Id
                                });

                            // Добавить ремонт в комплекс
                            await new RepairsServiceProxy().AddRepairToComplexAsync(
                                new AddRepairToComplexParameterSet
                                {
                                    ComplexId = complex.Dto.Id,
                                    RepairId = SelectedRepair.Id
                                });
                            LoadData(SelectedRepair.Id);
                        },
                        Content = new TextBlock
                        {
                            TextWrapping = TextWrapping.Wrap,
                            Width = 300,
                            Text = msg
                        },
                        Header = "Добавление ремонта в комплекс",
                        OkButtonContent = "Да",
                        CancelButtonContent = "Нет"
                    });
            }
            else
            {
                // просто добавить и все
                await new RepairsServiceProxy().AddRepairToComplexAsync(
                    new AddRepairToComplexParameterSet
                    {
                        ComplexId = complex.Dto.Id,
                        RepairId = SelectedRepair.Id
                    });
                LoadData(SelectedRepair.Id);
            }
        }


        public DelegateCommand RemoveFromComplexCommand { get; }

        private void RemoveFromComplex()
        {
            RadWindow.Confirm(
                new DialogParameters
                {
                    Closed = async (s, e) =>
                    {
                        if (e.DialogResult.HasValue && e.DialogResult.Value)
                        {
                            await new RepairsServiceProxy().AddRepairToComplexAsync(
                                new AddRepairToComplexParameterSet
                                {
                                    ComplexId = null,
                                    RepairId = SelectedRepair.Id
                                });
                            LoadData(SelectedRepair.Id);
                        }
                    },
                    Content = "Вы уверены что хотите исключить работу из комплекса?",
                    Header = "Исключить из комплекса",
                    OkButtonContent = "Да",
                    CancelButtonContent = "Нет"
                });
        }
        #endregion

        #region PLANNING STAGES

        /// <summary>
        /// Список этапов планирования
        /// </summary>
        public IEnumerable<PlanningStage> PlanningStageList
        {
            get
            {
                yield return PlanningStage.Filling;
                yield return PlanningStage.Optimization;
                yield return PlanningStage.Approved;
            }
        }


        /// <summary>
        ///     Этап планирования
        /// </summary>
        public PlanningStage PlanningStage
        {
            get { return _planningStage; }
            set
            {
                if (SetProperty(ref _planningStage, value))
                {
                    OnPropertyChanged(() => IsChangesAllowed);
                    UpdatePlanningStage();
                }
            }
        }

        /// <summary>
        ///     Информация о дате и пользователе изменившем этап планирования
        /// </summary>
        //public string PlanningStageUpdateInfo
        //{
        //    get { return _planningStageUpdateInfo; }
        //    set { SetProperty(ref _planningStageUpdateInfo, value); }
        //}



        /// <summary>
        ///     Разрешено ли изменение плана в зависимости от этапа планирования и профиля пользователя
        ///     (используется для блокировки ввода работ на ЛПУ)
        /// </summary>
        public bool IsChangesAllowed
        {
            get
            {
                if (!IsEditPermission) return false;
                //
                switch (PlanningStage)
                {
                    case PlanningStage.Filling:
                        return true;

                    case PlanningStage.Optimization:
                        return UserProfile.Current.Site.IsEnterprise;

                    case PlanningStage.Approved:
                        return false;

                    default:
                        return false;
                }
            }
        }

        /// <summary>
        ///     Сохранить выбранный этап планнирования в БД
        /// </summary>
        private async void UpdatePlanningStage()
        {
            if (!SelectedYear.HasValue) return;

            await new RepairsServiceProxy().SetPlanningStageAsync(
                new SetPlanningStageParameterSet
                {
                    Year = SelectedYear.Value,
                    SystemId = SelectedSystem.Id,
                    Stage = PlanningStage
                });

            LoadData(SelectedRepair?.Id, SelectedComplex?.Dto.Id);
        }

        #endregion

        public DelegateCommand FilterByComplexCommand { get; set; }

        public string ComplexFilter { get; set; }

        private void FilterByComplex()
        {
            if (SelectedComplex == null) return;

            ComplexFilter = SelectedComplex.Dto.ComplexName;
            OnPropertyChanged(() => ComplexFilter);
        }

        #region GANTT

        private bool _isGanttVisible;

        /// <summary>
        /// Активна вкладка с Гантом
        /// </summary>
        public bool IsGanttActive
        {
            get { return _isGanttVisible; }
            set { SetProperty(ref _isGanttVisible, value); }
        }

        public List<GanttTask> GanttTaskList { get; set; }

        public GanttTask SelectedGanttTask { get; set; }


        public void RefreshGantt(List<Repair> repairs)
        {
            if (!SelectedYear.HasValue) return;
            VisibleRange = new VisibleRange(new DateTime(SelectedYear.Value, 1, 1),
                new DateTime(SelectedYear.Value, 12, 31));
            OnPropertyChanged(() => VisibleRange);


            GanttTaskList = new List<GanttTask>();
            var groups = repairs.Select(r => r.PipelineName).Distinct();
            foreach (var group in groups)
            {
                var groupRepairs = repairs.Where(r => r.PipelineName == group).ToList();
                var groupItem = new GroupTask(groupRepairs.Min(r => r.Dto.StartDate),
                    groupRepairs.Max(r => r.Dto.EndDate), group);
                groupItem.Children.AddRange(groupRepairs.Select(r => new RepairTask(r)));
                GanttTaskList.Add(groupItem);
            }
            OnPropertyChanged(() => GanttTaskList);
        }

        private int _ganttRange = 0;

        public int GanttRange
        {
            get { return _ganttRange; }
            set
            {
                if (SetProperty(ref _ganttRange, value))
                {
                    switch (_ganttRange)
                    {
                        case 0:
                            PixelLength = TimeSpan.FromTicks(47000000000);
                            break;
                        case 1:
                            PixelLength =
                                TimeSpan.FromTicks(VisibleRange.End.Subtract(VisibleRange.Start).Ticks/VisibleArea);
                            break;
                    }
                }
            }
        }

        public VisibleRange VisibleRange { get; set; }
        public long VisibleArea { get; set; }


        private TimeSpan _pixelLength = TimeSpan.FromTicks(47000000000);

        public TimeSpan PixelLength
        {
            get { return _pixelLength; }
            set { SetProperty(ref _pixelLength, value); }
        }

        private void InitGantt()
        {
            DatesEditedCommand = new Telerik.Windows.Controls.DelegateCommand(OnTaskEdited);
        }

        public Telerik.Windows.Controls.DelegateCommand DatesEditedCommand { get; private set; }

        private void OnTaskEdited(object obj)
        {
            //var cp = (TaskEditedEventArgs)obj;
            //var gt = (GanttRepairTask)cp.Task;
            //if (gt.RepairItem != null &&
            //    (gt.RepairItem.StartDatePlan != gt.Start || gt.RepairItem.EndDatePlan != gt.End))
            //{
            //    // Если ремонт включен в комплекс и после изменения даты ремонта не соответсвуют датам комплекса  
            //    if (gt.RepairItem.ComplexId > 0 &&
            //        (gt.Start < gt.RepairItem.Dto.Complex.StartDate || gt.End > gt.RepairItem.Dto.Complex.EndDate) &&
            //        gt.RepairItem.StartDatePlan >= gt.RepairItem.Dto.Complex.StartDate &&
            //        gt.RepairItem.EndDatePlan <= gt.RepairItem.Dto.Complex.EndDate)
            //    {
            //        var vm = new QuestionViewModel(num =>
            //        {
            //            switch (num)
            //            {
            //                case 1:
            //                    //UpdateRepairDates(gt.RepairItem, gt.Start, gt.End);
            //                    break;

            //                case 2:
            //                    //UpdateRepairDates(gt.RepairItem, gt.Start, gt.End);
            //                    // Исключить работу из комплекса
            //                    //AddRepairToComplex(gt.RepairItem, null);
            //                    break;

            //                case 3:
            //                    EditRepairDates(new EditRepairDatesParameterSet
            //                    {
            //                        DateStart = gt.Start,
            //                        DateEnd = gt.End,
            //                        DateType = DateTypes.Plan,
            //                        RepairId = gt.RepairItem.Id
            //                    }, () =>
            //                    {
            //                        var shift = gt.Start - gt.RepairItem.StartDatePlan;

            //                        if (gt.End - gt.Start ==
            //                            gt.RepairItem.EndDatePlan - gt.RepairItem.StartDatePlan)
            //                        {
            //                            // Если работа просто передвинута
            //                            // Сдвигаем комплекс и все работы внутри, на тоже значение
            //                            MoveComplex(gt.RepairItem.Dto.Complex,
            //                                gt.RepairItem.Dto.Complex.StartDate + shift,
            //                                gt.RepairItem.Dto.Complex.EndDate + shift);
            //                        }
            //                        else
            //                        {
            //                            //Если у работы увеличилась продолжительность
            //                            if (gt.End - gt.Start >
            //                                gt.RepairItem.EndDatePlan - gt.RepairItem.StartDatePlan)
            //                            {
            //                                // Просто увеличиваем продолжительность комплекса
            //                                MoveComplex(gt.RepairItem.Dto.Complex,
            //                                    gt.Start < gt.RepairItem.Dto.Complex.StartDate
            //                                        ? gt.Start
            //                                        : gt.RepairItem.Dto.Complex.StartDate,
            //                                    gt.End > gt.RepairItem.Dto.Complex.EndDate
            //                                        ? gt.End
            //                                        : gt.RepairItem.Dto.Complex.EndDate);
            //                            }
            //                        }
            //                    },
            //                        Behavior);

            //                    break;

            //                default:
            //                    gt.Start = gt.RepairItem.Dto.StartDate;
            //                    gt.End = gt.RepairItem.Dto.EndDate;
            //                    break;
            //            }
            //        })
            //        {
            //            Header = "Дата ремонта не соответствуют дате комплекса",
            //            Question =
            //                "Новые даты проведения ремонта не соответствуют датам проведения комплекса. Что делать?",
            //            AnswerList = new List<Answer>
            //            {
            //                new Answer {Num = 1, Text = "Только передвинуть"},
            //                new Answer {Num = 2, Text = "Передвинуть и исключить из комплекса"},
            //                new Answer
            //                {
            //                    Num = 3,
            //                    Text = "Передвинуть ремонт вместе с комплексом (включая все добавленные ремонты)"
            //                },
            //                new Answer {Num = 4, Text = "Ничего не делать"}
            //            }
            //        };
            //        var v = new QuestionView { DataContext = vm };
            //        v.ShowDialog();
            //    }
            //    else
            //    {
            //        UpdateRepairDates(gt.RepairItem, gt.Start, gt.End);
            //    }
            //}
        }

        public void ExportToPdf_(FrameworkElement content)
        {
            var ganttView = FindChild(content, x => x is Telerik.Windows.Controls.RadGanttView) as Telerik.Windows.Controls.RadGanttView;
            var dlg = new SaveFileDialog() { Filter = "Pdf files (*.pdf)|*.pdf|All files (*.*)|*.*", FilterIndex = 1, /*DefaultFileName = Header*/ };
            if (dlg.ShowDialog() == true)
            {
                var stream = dlg.OpenFile();
                Lock("Экспорт в PDF...");
                var bw = new System.ComponentModel.BackgroundWorker();
                bw.DoWork += (s, e) => {
                    System.Threading.Thread.Sleep(1000);
                    ganttView.Dispatcher.BeginInvoke(() => {
                        try
                        {
                            using (stream)
                            {
                                var document = new RadFixedDocument();
                                var pageSize = new Size(1056, 1488.58);// new Size(815, 1023);
                                
                                using (var export = ganttView.ExportingService.BeginExporting(new ImageExportSettings(pageSize, true, GanttArea.AllAreas)))
                                {
                                    foreach (var img in export.ImageInfos.Select(info => info.Export()))
                                    {
                                        var page = new RadFixedPage();
                                        page.Size = pageSize;
                                        page.Content.AddImage(new Telerik.Windows.Documents.Fixed.Model.Resources.ImageSource(img, ImageQuality.High));
                                        document.Pages.Add(page);
                                    }
                                }
                                new PdfFormatProvider().Export(document, stream);
                                stream.Flush();
                            }
                        }
                        finally
                        {
                            Unlock();
                        }
                    });
                };
                bw.RunWorkerAsync();
            }
        }

        public async void ExportToPdf(FrameworkElement content)
        {
            var ganttView = FindChild(content, x => x is RadGanttView) as RadGanttView;
            var dlg = new SaveFileDialog() { Filter = "Pdf files (*.pdf)|*.pdf|All files (*.*)|*.*", FilterIndex = 1,  /*DefaultFileName = Header*/ };
            if (dlg.ShowDialog() == true)
            {
                try
                {
                    Lock("Экспорт в PDF...");
                    var stream = dlg.OpenFile();
                    using (stream)
                    {
                        await Task.Factory.StartNew(() =>
                        {
                            System.Threading.Thread.Sleep(1000);
                        });
                        var pageSize = new Size(1056, 1488.58);// new Size(815, 1023); 
                        var Bitmaps = await ganttView.Dispatcher.InvokeAsync(() =>
                        {                           
                            using (var export = ganttView.ExportingService.BeginExporting(new ImageExportSettings(pageSize, true, GanttArea.AllAreas)))
                            {
                                return export.ImageInfos.Select(info => info.Export()).ToList();
                            }
                        });
                        await Task.Factory.StartNew(() =>
                        {
                            var document = new RadFixedDocument();
                            foreach (var img in Bitmaps)
                            {
                                var page = new RadFixedPage();
                                page.Size = pageSize;
                                page.Content.AddImage(new Telerik.Windows.Documents.Fixed.Model.Resources.ImageSource(img, ImageQuality.High));
                                document.Pages.Add(page);
                            }
                            new PdfFormatProvider().Export(document, stream);
                        });
                        stream.Flush();
                    }
                }
                finally
                {
                    Unlock();
                }
            }
        }

        private static DependencyObject FindChild(DependencyObject dObject, Func<DependencyObject, bool> predicate)
        {
            var fElement = dObject as FrameworkElement;
            if (fElement != null)
            {
                int cCount = VisualTreeHelper.GetChildrenCount(fElement);
                for (int i = 0; i < cCount; i++)
                {
                    var child = VisualTreeHelper.GetChild(fElement, i);
                    if (predicate(child)) return child;
                    var v = FindChild(child, predicate);
                    if (v != null) return v;
                }
            }
            return null;
        }

        #endregion
    }

}