using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using GazRouter.Common.ViewModel;
using Microsoft.Practices.Prism.Commands;
using Telerik.Windows.Media.Imaging;

namespace GazRouter.ManualInput.CompUnitTests.ChartDigitizer
{
    public class ChartDigitizerViewModel : DialogViewModel
    {
        public ChartDigitizerViewModel(Action callback)
            : base(callback)
        {
            InitStateManager();
        }



        #region УПРАВЛЕНИЕ ШАГАМИ ВИЗАРДА
        
        public WizardStateManager StateManager { get; set; }


        private void InitStateManager()
        {
            StateManager = new WizardStateManager();


            // Открытие файла
            StateManager.RegisterState("SelectFile",
                new DelegateCommand(() => {}, () => false),
                new DelegateCommand(GoNext, () => ImgSource != null));

            // Обрезание изображения
            StateManager.RegisterState("Crop",
                new DelegateCommand(GoPrev),
                new DelegateCommand(() =>
                {
                    GoNext();
                    CropImage();
                }));

            // Оцифровка
            StateManager.RegisterState("Adjust",
                new DelegateCommand(GoPrev),
                new DelegateCommand(GoNext));
        }


        public DelegateCommand NextCommand
        {
            get { return StateManager.CurrentState.GoNextCommand; }
        }

        public DelegateCommand PrevCommand
        {
            get { return StateManager.CurrentState.GoPrevCommand; }
        }

        // Обновить команды перехода
        private void UpdateStepCommands()
        {
            OnPropertyChanged(() => PrevCommand);
            OnPropertyChanged(() => NextCommand);
            //NextCommand.RaiseCanExecuteChanged();
            //PrevCommand.RaiseCanExecuteChanged();
        }

        /// <summary>
        /// Переход к следующему шагу
        /// </summary>
        private void GoNext()
        {
            StateManager.GoNext();
            UpdateStepCommands();
        }

        /// <summary>
        /// Переход к предыдущему шагу
        /// </summary>
        private void GoPrev()
        {
            StateManager.GoPrev();
            UpdateStepCommands();
        }

        #endregion

        

        #region ОТКРЫТИЕ ФАЙЛА

        public DelegateCommand OpenFileCommand
        {
            get { return new DelegateCommand(OpenFile); }
        }

        private void OpenFile()
        {
            var dlg = new OpenFileDialog
            {
                Multiselect = false,
                Filter = @"Изображения|*.jpeg;*.jpg;*.png;*.tif;*.tiff"
            };

            var result = dlg.ShowDialog();
            if (result.HasValue && result.Value)
            {
                FileName = dlg.File.Name;
                ImgSource = new BitmapImage();
                ImgSource.SetSource(dlg.File.OpenRead());
                GoNext();
                OnPropertyChanged(() => ImgSource);
                OnPropertyChanged(() => FileName);
            }
        }
        public BitmapImage ImgSource { get; set; }
        public string FileName { get; set; }

        #endregion



        #region ОБРЕЗАНИЕ ИЗОБРАЖЕНИЯ

        /// <summary>
        /// Границы по которым будет обрезаться изображение
        /// </summary>
        public Rect CropRect { get; set; }

        
        /// <summary>
        /// Обрезанное изображение
        /// </summary>
        public BitmapImage CroppedImage { get; set; }

        private void CropImage()
        {
            var src = new WriteableBitmap(ImgSource);
            var left = (int) CropRect.X;
            var top = (int) CropRect.Y;
            var width = (int) CropRect.Width;
            var height = (int) CropRect.Height;

            var dest = new WriteableBitmap(width, height);
            var i = 0;
            for (var y = top; y < top + height; y++)
            {
                Array.Copy(src.Pixels, src.PixelWidth * y + left, dest.Pixels, i, width);
                i += width;
            }

            CroppedImage = new BitmapImage();
            using (var stream = new MemoryStream())
            {
                var encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(dest));
                encoder.Save(stream);
                CroppedImage.SetSource(stream);
            }

            OnPropertyChanged(() => CroppedImage);
        }

        #endregion

    }






    public class WizardStateManager : PropertyChangedBase
    {
        private int _currentStateNum;
        private readonly List<WizardState> _states;

        public WizardStateManager()
        {
            _states = new List<WizardState>();
        }

        /// <summary>
        /// После создания, нужно зарегистрировать состояния с помощью этой функции
        /// Последовательность состояний, определяется последовательностью вызовов этой функции
        /// </summary>
        public void RegisterState(string name, DelegateCommand goPrevCmd = null, DelegateCommand goNextCmd = null)
        {
            _states.Add(new WizardState(name, goPrevCmd, goNextCmd));

            if (_states.Count == 1)
                _states[0].IsActive = true;
        }


        public WizardState this[string key]
        {
            get { return _states.SingleOrDefault(s => s.Name == key); }
        }

        /// <summary>
        /// Возвращает текущее состояние
        /// </summary>
        public WizardState CurrentState
        {
            get { return _states[_currentStateNum]; }
        }



        /// <summary>
        /// Является ли текущее состояние первым
        /// </summary>
        public bool IsFirst
        {
            get { return _currentStateNum == 0; }
        }

        /// <summary>
        /// Является ли текущее состояние последним
        /// </summary>
        public bool IsLast
        {
            get { return _currentStateNum == _states.Count - 1; }
        }
        
        /// <summary>
        /// Перейти к следущему состоянию
        /// </summary>
        public void GoNext()
        {
            ChangeState(_currentStateNum + 1);
        }


        /// <summary>
        /// Вернуться к предыдущему состоянию
        /// </summary>
        public void GoPrev()
        {
            ChangeState(_currentStateNum - 1);
        }


        private void ChangeState(int newStateNum)
        {
            if (_states.Count == 0 || newStateNum < 0 || newStateNum >= _states.Count)
                return;

            _states[_currentStateNum].IsActive = false;
            _states[newStateNum].IsActive = true;
            _currentStateNum = newStateNum;

            OnPropertyChanged(() => CurrentState);
            OnPropertyChanged(() => IsFirst);
            OnPropertyChanged(() => IsLast);
        }

    }
   

    public class WizardState : PropertyChangedBase
    {
        private readonly string _name;
        private readonly DelegateCommand _goNextCommand;
        private readonly DelegateCommand _goPrevCommand;

        public WizardState(string name, DelegateCommand goPrevCmd, DelegateCommand goNextCmd)
        {
            _name = name;
            _goPrevCommand = goPrevCmd;
            _goNextCommand = goNextCmd;
        }

        /// <summary>
        /// Наименование состояния
        /// </summary>
        public string Name
        {
            get { return _name; }
        }


        private bool _isActive;
        /// <summary>
        /// Является ли данное состояние активным в текущий момент
        /// </summary>
        public bool IsActive
        {
            get { return _isActive; }
            set { SetProperty(ref _isActive, value); }
        }


        /// <summary>
        /// Команда перехода к предыдущему состоянию
        /// </summary>
        public DelegateCommand GoPrevCommand
        {
            get { return _goPrevCommand; }
        }

        /// <summary>
        /// Команда перехода к следующему состоянию
        /// </summary>
        public DelegateCommand GoNextCommand
        {
            get { return _goNextCommand; }
        }
    }


    

    

}
