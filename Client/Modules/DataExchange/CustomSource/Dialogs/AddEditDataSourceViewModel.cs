using System;
using System.Threading.Tasks;
using GazRouter.Common.ViewModel;
using GazRouter.DataProviders.DataExchange;
using GazRouter.DTO.DataExchange.DataSource;

namespace GazRouter.DataExchange.CustomSource.Dialogs
{
    public class AddEditDataSourceViewModel : AddEditViewModelBase<DataSourceDTO, int>
    {
        public AddEditDataSourceViewModel(Action<int> actionBeforeClosing)
            : base(actionBeforeClosing)
        {
            SetValidationRules();
        }


        public AddEditDataSourceViewModel(Action<int> actionBeforeClosing, DataSourceDTO dto)
            : base(actionBeforeClosing, dto)
        {
            SetValidationRules();

            Name = dto.Name;
            SysName = dto.SysName;
            Description = dto.Description;
        }


        private string _sysName;
        public string SysName
        {
            get { return _sysName; }
            set
            {
                if (SetProperty(ref _sysName, value))
                    SaveCommand.RaiseCanExecuteChanged();
            }
        }



        public string Description { get; set; }
        



        protected override bool OnSaveCommandCanExecute()
        {
            ValidateAll();
            return !HasErrors;
        }
        

        protected override Task<int> CreateTask
        {
            get
            {
                return new DataExchangeServiceProxy().AddDataSourceAsync(
                    new AddDataSourceParameterSet
                    {
                        Name = Name,
                        SysName = SysName,
                        Description = Description
                    });
            }
        }

        protected override Task UpdateTask
        {
            get
            {
                return new DataExchangeServiceProxy().EditDataSourceAsync(
                    new EditDataSourceParameterSet
                    {
                        Id = Model.Id,
                        Name = Name,
                        SysName = SysName,
                        Description = Description
                    });
            }
        }

        protected override string CaptionEntityTypeName
        {
            get { return "источника данных"; }
        }

        private void SetValidationRules()
        {
            AddValidationFor(() => Name)
                .When(() => string.IsNullOrEmpty(Name))
                .Show("Введите наименование источника данных");

            AddValidationFor(() => SysName)
                .When(() => string.IsNullOrEmpty(SysName))
                .Show("Введите системное наименование");
        }

        
    }
}