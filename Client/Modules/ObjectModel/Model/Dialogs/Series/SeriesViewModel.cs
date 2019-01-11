using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Controls;
using GazRouter.Application;
using GazRouter.Common;
using GazRouter.Common.ViewModel;
using GazRouter.Controls;
using GazRouter.DataProviders.SeriesData;
using GazRouter.DTO.Dictionaries.PeriodTypes;
using GazRouter.DTO.Dictionaries.PhisicalTypes;
using GazRouter.DTO.Dictionaries.PropertyTypes;
using GazRouter.DTO.ObjectModel;
using GazRouter.DTO.SeriesData.PropertyValues;
using Microsoft.Practices.Prism.Commands;
using Utils.Extensions;

namespace GazRouter.ObjectModel.Model.Dialogs.Series
{
    public class SeriesViewModel : DialogViewModel
    {
        private CommonEntityDTO _entity;
        private PropertyTypeDTO _propertyType;

        public SeriesViewModel(Action actionBeforeClosing, CommonEntityDTO entity, PropertyType propertyTypeId)
            : base(actionBeforeClosing)
        {
            _entity = entity;
            _propertyType = ClientCache.DictionaryRepository.PropertyTypes.Single(pt => pt.PropertyType == propertyTypeId);

            _selectedPeriodType = PeriodTypeList.First();
            _selectedPeriodDates.EndDate = DateTime.Now.Date.ToLocalTime();
            _selectedPeriodDates.BeginDate = DateTime.Now.Date.AddDays(-3);

            ExportExcelCommand = new DelegateCommand(OnExcelExportCommandExecute, () => Items != null && Items.Count > 0);
            
            Refresh();
        }


        public List<BasePropertyValueDTO> Items { get; set; }
        
        
        public IEnumerable<PeriodTypeDTO> PeriodTypeList
        {
            get
            {
                yield return ClientCache.DictionaryRepository.PeriodTypes.Single(p => p.PeriodType == PeriodType.Twohours);
                yield return ClientCache.DictionaryRepository.PeriodTypes.Single(p => p.PeriodType == PeriodType.Day);
                yield return ClientCache.DictionaryRepository.PeriodTypes.Single(p => p.PeriodType == PeriodType.Month);
            }
        }
        
        private PeriodTypeDTO _selectedPeriodType;
        public PeriodTypeDTO SelectedPeriodType
        {
            get { return _selectedPeriodType; }
            set
            {
                if (SetProperty(ref _selectedPeriodType, value))
                    Refresh();
            }
        }
        

        private PeriodDates _selectedPeriodDates = new PeriodDates();
        public PeriodDates SelectedPeriodDates
        {
            get { return _selectedPeriodDates; }
            set
            {
                if (SetProperty(ref _selectedPeriodDates, value))
                    Refresh();
            }
        }

        

        private async void Refresh()
        {
            if (SelectedPeriodType == null) return;

            try
            {
                Behavior.TryLock();

                Items = (await new SeriesDataServiceProxy().GetPropertyValueListAsync(
                    new GetPropertyValueListParameterSet
                    {
                        EntityId = _entity.Id,
                        PropertyTypeId = _propertyType.PropertyType,
                        PeriodTypeId = SelectedPeriodType.PeriodType,
                        StartDate = SelectedPeriodDates.BeginDate.Value.ToLocal(),
                        EndDate = SelectedPeriodDates.EndDate.Value.ToLocal()
                    })).ToList();

                foreach (var val in Items.OfType<PropertyValueDoubleDTO>())
                {
                    val.Value = UserProfile.ToUserUnits(val.Value, _propertyType.PhysicalType.PhysicalType);
                }

                OnPropertyChanged(() => Items);
                ExportExcelCommand.RaiseCanExecuteChanged();
                
            }
            finally 
            {
                Behavior.TryUnlock();
            }
        }
        

        public string ValueColumnHeader => $"Значение, {UserProfile.UserUnitName(_propertyType.PhysicalType.PhysicalType)}";
            

        public string Caption
        {
            get { return "Данные"; }
        }

        public DelegateCommand ExportExcelCommand { get; private set; }

        private void OnExcelExportCommandExecute()
        {
            var dialog = new SaveFileDialog
            {
                DefaultExt = "xlsx",
                Filter = string.Format("Excel files (*.xlsx)|*.xlsx|All files (*.*)|*.*"),
                FilterIndex = 1
            };
            if (dialog.ShowDialog() == true)
            {
                var excelReport = new ExcelReport();
                excelReport.Write("C:").Write(SelectedPeriodDates.BeginDate.Value);
                excelReport.Write("по:").Write(SelectedPeriodDates.EndDate.Value);
                excelReport.NewRow();
                excelReport.Write("Период:").Write(SelectedPeriodType.Name).NewRow();
                excelReport.NewRow();
                excelReport.WriteHeader("Метка времени", 150);
                excelReport.WriteHeader(ValueColumnHeader, 150);
                excelReport.NewRow();
                foreach (var i in Items)
                {
                    excelReport.WriteCell(i.Date.ToDailyDateTimeString());

                    var propertyValueDoubleDTO = i as PropertyValueDoubleDTO;
                    if (propertyValueDoubleDTO != null)
                        excelReport.WriteCell(propertyValueDoubleDTO.Value);
                    else
                    {
                        var propertyValueDateDTO = i as PropertyValueDateDTO;
                        if (propertyValueDateDTO != null)
                        {
                            excelReport.WriteCell(propertyValueDateDTO.Value.ToString(CultureInfo.InvariantCulture));
                        }
                        else
                        {
                            var propertyValueStringDTO = i as PropertyValueStringDTO;
                            if (propertyValueStringDTO != null)
                            {
                                excelReport.WriteCell(propertyValueStringDTO.Value);
                            }
                        }
                    }
                    excelReport.NewRow();
                }

                using (var stream = dialog.OpenFile())
                {
                    excelReport.Save(stream);
                }
            }
        }
        
    }
}