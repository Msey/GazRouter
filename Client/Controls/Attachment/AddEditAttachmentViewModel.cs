using System;
using System.Windows.Controls;
using GazRouter.Common.ViewModel;
using GazRouter.DTO.Attachments;
using Microsoft.Practices.Prism.Commands;


namespace GazRouter.Controls.Attachment
{
    public class AddEditAttachmentViewModel : DialogViewModel<Action<object>>
	{
        public AddEditAttachmentViewModel(Action<object> callback)
            : base(callback)
        {

            BrowseCommand = new DelegateCommand(OnBrowseCommandExecuted);
            SaveCommand = new DelegateCommand(() => DialogResult = true);
        }
        

        public AddEditAttachmentViewModel(Action<object> callback, AttachmentBaseDTO dto)
            : base(callback)
        {
            BrowseCommand = new DelegateCommand(OnBrowseCommandExecuted);
            SaveCommand = new DelegateCommand(() => DialogResult = true);

            _description = dto.Description;
            _fileName = dto.FileName;
            _fileSize = dto.DataLength;

            IsEditMode = true;
        }
        
        

        
        private bool _isFileLoading;
        public bool IsEditMode { get; set; }


        private string _fileName;
        /// <summary>
        /// Имя файла
        /// </summary>
        public string FileName => _fileName;

        private long _fileSize;
        /// <summary>
        /// Размер файла, kb
        /// </summary>
        public long FileSize => _fileSize;

        private byte[] _fileData;
        /// <summary>
        /// Сам файл (данные)
        /// </summary>
        public byte[] FileData => _fileData;

        public bool IsFileSelected => FileSize > 0;

        private string _description;
        /// <summary>
        /// Описание
        /// </summary>
        public string Description
		{
			get { return _description; }
			set
			{
                _description = value;
                OnPropertyChanged(() => Description);
				RefreshCommands();
			}
		}

		

		public bool IsFileLoading
		{
			get { return _isFileLoading; }
			set
			{
				_isFileLoading = value;
                OnPropertyChanged(() => IsFileLoading);
			}
		}
		
		public DelegateCommand BrowseCommand { get; private set; }
        public DelegateCommand SaveCommand { get; private set; }
        
		
		private void RefreshCommands()
        {
            SaveCommand.RaiseCanExecuteChanged();
        }

        private void OnBrowseCommandExecuted()
        {
            var dlg = new OpenFileDialog
            {
                Multiselect = false,
                Filter = @" Все(*.*)|*.*|Изображения(*.jpeg;*.jpg;*.png)|*.jpeg;*.jpg;*.png|Документы MS Word(*.doc)|*.doc|Файлы PDF(*.pdf)|*.pdf"
            };

            var result = dlg.ShowDialog();
            if (result.HasValue && result.Value)
            {
                _fileName = dlg.File.Name;
                _fileSize = dlg.File.Length/1024;

                IsFileLoading = true;
                using (var fileStream = dlg.File.OpenRead())
                {
                    _fileData = new byte[fileStream.Length];
                    fileStream.Read(_fileData, 0, _fileData.Length);
                }
                IsFileLoading = false;
            }

            OnPropertyChanged(() => FileName);
            OnPropertyChanged(() => FileSize);
            OnPropertyChanged(() => IsFileSelected);

            RefreshCommands();
        }

        public string ConfirmButtonCaption => IsEditMode ? "Изменить" : "Добавить";


        protected override void InvokeCallback(Action<object> closeCallback)
        {
            if (closeCallback != null)
                closeCallback(this);
        }
	}
}
