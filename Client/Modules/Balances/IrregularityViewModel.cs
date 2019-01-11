using System;
using System.Collections.Generic;
using System.Globalization;
using Common.ViewModel;
using DTO.Balances.Plan;
using Telerik.Windows.Controls.ScheduleView;

namespace Balances.Irregularity
{
	public class IrregularityViewModel : PropertyChangedBase
	{

		public IrregularityViewModel(DateTime firstDate)
		{
			_date = firstDate;
			GetIrregularityList();
		}

		private List<Irregularity> _list;

		public List<Irregularity> List
		{
			get { return _list; }
			set
			{
				_list = value;
				OnPropertyChanged(() => List);
			}
		}

		private DateTime _date;

		private void GetIrregularityList()
		{
			
		}
	}

	public class Irregularity : PropertyChangedBase,IAppointment
	{
		public Irregularity(IrregularityIntakeTransitCorrectionDto dto)
		{
			_dto = dto;
		}

		private readonly IrregularityIntakeTransitCorrectionDto _dto;

		public DateTime Start
		{
			get { return _dto.StartDate; }
			set
			{
				_dto.StartDate = value;
				OnPropertyChanged(() => Start);
			}
		}

		public DateTime End
		{
			get { return _dto.EndDate; }
			set
			{
				_dto.EndDate = value;
				OnPropertyChanged(() => End);
			}
		}

		public bool IsAllDayEvent { get; set; }

		public IRecurrenceRule RecurrenceRule { get; set; }

		public event EventHandler RecurrenceRuleChanged;

		public System.Collections.IList Resources
		{
			get { return null; }
		}

		public string Subject
		{
			get { return _dto.Value.ToString(CultureInfo.InvariantCulture); }
			set
			{
				_dto.Value = Convert.ToDouble(value);
				OnPropertyChanged(() => Subject);
			}
		}

		public TimeZoneInfo TimeZone {get;set;}

		public void BeginEdit()
		{
		}

		public void CancelEdit()
		{
		}

		public void EndEdit()
		{
		}

		public bool Equals(IAppointment other)
		{
			throw new NotImplementedException();
		}

		public IAppointment Copy()
		{
			throw new NotImplementedException();
		}

		public void CopyFrom(IAppointment other)
		{
			throw new NotImplementedException();
		}
	}
}
