using System;
using GazRouter.Common.ViewModel;
using Microsoft.Practices.Prism.Commands;

namespace GazRouter.Controls.Dialogs.ConfirmSave
{
	public class ConfirmSaveDialogViewModel : DialogViewModel
	{
        public ConfirmSaveDialogViewModel(Action<SaveType> callback)
			: base(null)
		{
            SaveCommand = new DelegateCommand(() =>
            {
                callback(SaveType.Replace);
                DialogResult = true;
            });

            DoNotSaveCommand = new DelegateCommand(() =>
            {
                callback(SaveType.DoNotSave);
                DialogResult = true;
            });
        }

        public DelegateCommand SaveNewCommand { get; private set; }
        public DelegateCommand SaveCommand { get; private set; }
        public DelegateCommand DoNotSaveCommand { get; private set; }

		public string Caption
		{
			get { return "Сохранение"; }
		}
	}
}
