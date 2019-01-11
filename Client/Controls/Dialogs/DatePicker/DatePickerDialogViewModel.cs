using System;
using GazRouter.Common.ViewModel;
using Microsoft.Practices.Prism.Commands;

namespace GazRouter.Controls.Dialogs.DatePicker
{
	public class DatePickerDialogViewModel : DialogViewModel<Action<DateTime>>
	{
        public DatePickerDialogViewModel(Action<DateTime> callback, DateTime date)
			: base(callback)
        {
            SelectedDate = new DateTime(date.Year, date.Month, 1);
            SelectCommand = new DelegateCommand(OnSelectCommandExecuted);
        }

        public DelegateCommand SelectCommand { get; private set; }

        private void OnSelectCommandExecuted()
        {
            DialogResult = true;
        }

        #region SelectedDate

        private DateTime? _selectedDate;

        public DateTime? SelectedDate
        {
            get { return _selectedDate; }
            set
            {
                _selectedDate = value;
                OnPropertyChanged(() => SelectedDate);
            }
        }

        #endregion

	    protected override void InvokeCallback(Action<DateTime> closeCallback)
	    {
            closeCallback(SelectedDate.Value);
	    }
	}
}
