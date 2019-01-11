using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using GazRouter.Common;
using GazRouter.Common.ViewModel;
using GazRouter.DataProviders.ObjectModel;
using GazRouter.DataProviders.Repairs;
using GazRouter.DTO.Dictionaries.EntityTypes;
using GazRouter.DTO.Dictionaries.PlanTypes;
using GazRouter.DTO.Dictionaries.RepairExecutionMeans;
using GazRouter.DTO.Dictionaries.RepairTypes;
using GazRouter.DTO.ObjectModel;
using GazRouter.DTO.ObjectModel.Valves;
using GazRouter.DTO.Repairs.Plan;
using GazRouter.DTO.Repairs.RepairWorks;
using GazRouter.DTO.SeriesData.PropertyValues;
using Telerik.Windows.Diagrams.Core;
using GazRouter.DTO.Repairs.RepairReport;

namespace GazRouter.Repair.ReqWorks.Dialogs
{
    public class AddEditRepairReportViewModel : AddEditViewModelBase<RepairPlanBaseDTO, int>
    {
        int? repId = null;


        public AddEditRepairReportViewModel(Action<int> actionBeforeClosing, RepairPlanBaseDTO repair, RepairReportDTO report = null)    
            : base(actionBeforeClosing, repair)    
        {
            Id = repair.Id;

            if (report != null)
            {
                repId = report.Id;
                ReportingDate = report.ReportDate;
                Description = report.Comment;
                OnPropertyChanged(() => Description);
                IsAdd = false;
                IsEdit = true;

            }
            else
            {
                ReportingDate = DateTime.Now;
            IsAdd = true;
            IsEdit = false;


            }

            SetValidationRules();
            ValidateAll();
        }

       public AddEditRepairReportViewModel(Action<int> actionBeforeClosing)
            : base(actionBeforeClosing)
        {
            ReportingDate = DateTime.Now;
            //_startDate = new DateTime(year, 1, 1);
            //_endDate = _startDate.AddDays(1);
            //_partsDeliveryDate = new DateTime(year, 1, 1);
            
            SetValidationRules();
        }




        #region Dates

        private DateTime _reportDate = DateTime.Now;
        /// <summary>
        /// Плановая дата начала проведения ремонтных работ
        /// </summary>
        public DateTime ReportingDate
        {
            get { return _reportDate; }
            set
            {
                if (SetProperty(ref _reportDate, value))
                {
                    OnPropertyChanged(() => ReportingDate);
                    SaveCommand.RaiseCanExecuteChanged();
                }
            }
        }


        #endregion


        private string _description;
        /// <summary>
        /// Описание работ
        /// </summary>
        public string Description
        {
            get { return _description; }
            set
            {
                if (SetProperty(ref _description, value))
                {
                    SaveCommand.RaiseCanExecuteChanged();
                }
            }
        }

        private void SetValidationRules()
        {


            //AddValidationFor(() => StartDate)
            //    .When(() => SelectedEntity != null && StartDate.Date < PartsDeliveryDate.Date)
            //    .Show("Дата начала работ не может быть меньше даты поставки МТР");

            AddValidationFor(() => ReportingDate)
                .When(() => ReportingDate > DateTime.Now)
                .Show("Отчетная дата не может быть больше текущей");

            AddValidationFor(() => Description)
                .When(() => string.IsNullOrEmpty(Description))
                .Show("Введите описание работ");
        }


        

        private string _descriptionGtp;
        /// <summary>
        /// Комментарий ГТП
        /// </summary>
        public string DescriptionGtp
        {
            get { return _descriptionGtp; }
            set
            {
                if (SetProperty(ref _descriptionGtp, value))
                {
                    SaveCommand.RaiseCanExecuteChanged();
                }
            }
        }


        #region Save

        protected override bool OnSaveCommandCanExecute()
        {
            ValidateAll();
            return !HasErrors;
        }

        protected override Task UpdateTask
        {
            get
            {
                var paramSet = new RepairReportParametersSet
                {
                    Id = repId.Value,
                    RepairID = Id,
                    Comment = Description,
                    ReportDate = ReportingDate
                };
                return new RepairsServiceProxy().EditRepairReportAsync(paramSet);
            }
        }

        protected override Task<int> CreateTask
        {
            get
            {
                var paramSet = new RepairReportParametersSet
                {
                    RepairID = Id,
                    Comment = Description,
                    ReportDate = ReportingDate
                };
                return new RepairsServiceProxy().AddRepairReportAsync(paramSet);
            }
        }

        protected override string CaptionEntityTypeName
        {
            get
            {
                return "";
            }
        }

        #endregion
    }
}