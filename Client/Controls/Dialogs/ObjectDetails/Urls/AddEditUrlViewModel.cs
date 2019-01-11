using System;
using GazRouter.Common.ViewModel;
using GazRouter.DataProviders.ObjectModel;
using GazRouter.DTO.ObjectModel.Entities.Urls;
using Microsoft.Practices.Prism.Commands;


namespace GazRouter.Controls.Dialogs.ObjectDetails.Urls
{
    public class AddEditUrlViewModel : DialogViewModel
    {
        private EntityUrlDTO _dto;

        public AddEditUrlViewModel(Action callback, Guid entityId)
            : base(callback)
        {
            _dto = new EntityUrlDTO
            {
                EntityId = entityId,
                Url = "http://"
            };

            SaveCommand = new DelegateCommand(Save, () => !HasErrors);

            SetValidationRules();
        }
        

        public AddEditUrlViewModel(Action callback, EntityUrlDTO dto)
            : base(callback)
        {
            SaveCommand = new DelegateCommand(Save);
            IsEditMode = true;

            _dto = dto;

            SetValidationRules();
        }
        
        
        public bool IsEditMode { get; set; }

        
        /// <summary>
        /// Url
        /// </summary>
        public string Url
        {
            get { return _dto.Url; }
            set
            {
                _dto.Url = value;
                OnPropertyChanged(() => Url);
                RefreshCommands();
            }
        }

        

        /// <summary>
        /// Описание
        /// </summary>
        public string Description
		{
			get { return _dto.Description; }
			set
			{
			    _dto.Description = value;
                OnPropertyChanged(() => Description);
                RefreshCommands();
            }
		}
        
        public DelegateCommand SaveCommand { get; }

        private async void Save()
        {
            if (!IsEditMode)
            {
                await new ObjectModelServiceProxy().AddEntityUrlAsync(
                    new AddEntityUrlParameterSet
                    {
                        EntityId = _dto.EntityId,
                        Description = _dto.Description,
                        Url = _dto.Url
                    });
            }
            else
            {
                await new ObjectModelServiceProxy().EditEntityUrlAsync(
                    new EditEntityUrlParameterSet
                    {
                        UrlId = _dto.UrlId,
                        EntityId = _dto.EntityId,
                        Description = _dto.Description,
                        Url = _dto.Url
                    });
            }
            DialogResult = true;
        }
		
		private void RefreshCommands()
        {
            ValidateAll();
            SaveCommand.RaiseCanExecuteChanged();
        }

        private void SetValidationRules()
        {
            AddValidationFor(() => Url).When(() => string.IsNullOrEmpty(Url)).Show("Введите ссылку");
            AddValidationFor(() => Description).When(() => string.IsNullOrEmpty(Description)).Show("Введите описание");
        }
        
        public string Header
        {
            get { return (!IsEditMode ? "Добавление" : "Редактирование") + " ссылки"; } 
        }
	}
}
