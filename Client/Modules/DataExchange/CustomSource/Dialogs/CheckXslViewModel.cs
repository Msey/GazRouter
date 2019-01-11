using System;
using System.Threading.Tasks;
using GazRouter.Common.ViewModel;
using GazRouter.DataProviders.DataExchange;
using GazRouter.DTO.DataExchange.DataSource;
using GazRouter.DTO.DataExchange.ExchangeTask;
using GazRouter.DTO.DataExchange.Transformation;
using Microsoft.Practices.Prism.Commands;

namespace DataExchange.CustomSource.Dialogs
{
    public class CheckXslViewModel : DialogViewModel<Action<CheckXslViewModel>>
    {
        private readonly ExchangeTaskDTO _dto;
        public DelegateCommand CheckCommand { get; private set; }
        public CheckXslViewModel(ExchangeTaskDTO dto) : base(vm => { })
            
        {
            _dto = dto;
            CheckCommand = new DelegateCommand(OnCheck);
            FileNameMask = dto.FileNameMask;
            IsTransform = dto.IsTransform;
            Transformation = dto.Transformation;
            //AddValidationFor(() => FileName).When(() => string.IsNullOrEmpty(FileName));

        }

        private async void OnCheck()
        {
            var result = await new DataExchangeServiceProxy().TransformFileAsync(
                new ImportParams
                {
                    Task = _dto,
                    FileName = this.FileName,
                    Text = this.InputContent,
                    Transformation = this.Transformation
                });
            Result = result;
        }


        //public CheckXslViewModel(Action<int> actionBeforeClosing, ExchangeTaskDTO dto)

        //{
        //    FileNameMask = dto.FileNameMask;
        //    IsTransform = dto.IsTransform;
        //    Transformation = dto.Transformation;
        //    AddValidationFor(() => FileName).When(() => !string.IsNullOrEmpty(FileName));
        //}

        private string _fileNameMask;
        /// <summary>
        /// Маска файла
        /// </summary>
        public string FileNameMask
        {
            get { return _fileNameMask; }
            set
            {
                if (SetProperty(ref _fileNameMask, value))
                    UpdateCommands();
            }
        }


        private bool _isTransform;
        /// <summary>
        ///  Выполнять трансформацию файла (XSLT)
        /// </summary>
        public bool IsTransform
        {
            get { return _isTransform; }
            set
            {
                if (SetProperty(ref _isTransform, value))
                {
                    //if (!value) Transformation = "";
                    UpdateCommands();
                }
            }
        }

        private string _fileName;
        public string FileName
        {
            get { return _fileName; }
            set
            {
                if (SetProperty(ref _fileName, value))
                {
                    //if (!value) Transformation = "";
                    UpdateCommands();
                }
            }
        }

        private string _inputContent;
        public string InputContent
        {
            get { return _inputContent; }
            set
            {
                if (SetProperty(ref _inputContent, value))
                {
                    //if (!value) Transformation = "";
                    UpdateCommands();
                }
            }
        }

        private string _result;
        public string Result
        {
            get { return _result; }
            set
            {
                if (SetProperty(ref _result, value))
                {
                    //if (!value) Transformation = "";
                    UpdateCommands();
                }
            }
        }

        private string _transformation;
        /// <summary>
        /// Текст трансформации (XSLT)
        /// </summary>
        public string Transformation
        {
            get { return _transformation; }
            set
            {
                if (SetProperty(ref _transformation, value))
                    UpdateCommands();
            }
        }

        //protected override Task UpdateTask => new DataExchangeServiceProxy().TransformFileAsync(
        //    new ImportParams
        //    {
        //        Task = Model,
        //        FileName = this.FileName,
        //        Text = this.InputContent
        //    });


        private void UpdateCommands()
        {
            CheckCommand.RaiseCanExecuteChanged();
        }




        protected override void InvokeCallback(Action<CheckXslViewModel> closeCallback)
        {
            //closeCallback(this);
        }
    }
}