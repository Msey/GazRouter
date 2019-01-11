using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using GazRouter.Application;
using GazRouter.Application.Helpers;
using GazRouter.DataProviders.ObjectModel;
using GazRouter.DTO;
using GazRouter.DTO.Dictionaries.PhisicalTypes;
using GazRouter.DTO.ObjectModel.CompStationCoolingRecomended;

namespace GazRouter.ObjectModel.Model.Tabs.CoolingRecomended
{
	public class CompStationCoolingRecomendedViewModel : DtoListViewModelBase<CoolingRecomendedWraper,int>
    {
		public CompStationCoolingRecomendedViewModel()
		{
			SeriesRows = new ObservableCollection<CoolingRecomendedWraper>();
		}

		public override string Header
		{
			get { return "Т после АВО"; }
		}

		protected async override void GetList()
		{
		    try
		    {
                Behavior.TryLock();
                var list = await new ObjectModelServiceProxy().GetCompStationCoolingRecomendedListAsync(ParentEntity.Id);
                SeriesRows = new ObservableCollection<CoolingRecomendedWraper>
                {
			        new CoolingRecomendedWraper(list, SaveChanges)
                };
		    }
		    finally 
		    {
		        Behavior.TryUnlock();
		    }
		    
		}

		

		private ObservableCollection<CoolingRecomendedWraper> _seriesRows;

		public ObservableCollection<CoolingRecomendedWraper> SeriesRows
		{
			get { return _seriesRows; }
			set
			{
				if (_seriesRows == value) return;
				_seriesRows = value;
				OnPropertyChanged(() => SeriesRows);
			}
		}

		public async void SaveChanges(int month, double? temperature)
		{
		    try
		    {
                Behavior.TryLock();
                await new ObjectModelServiceProxy().SetCompStationCoolingRecomendedAsync(
                    new SetCompStationCoolingRecomendedParameterSet
                    {
                        CompStationId = ParentEntity.Id,
                        Month = month,
                        Temperature = temperature
                    });
		    }
		    finally
		    {
		        Behavior.TryUnlock();
		    }
		    
		}
    }

    public class CoolingRecomendedWraper : IListItem<int>
    {
        private readonly Action<int, double?> _saveChanges;
		private readonly List<CompStationCoolingRecomendedDTO> _dtos;
		public CoolingRecomendedWraper(List<CompStationCoolingRecomendedDTO> dto, Action<int, double?> saveChanges)
        {
            _dtos = dto;
			_saveChanges = saveChanges;
        }

        public string Name => $"T после АВО, {UserProfile.UserUnitName(PhysicalType.Temperature)}";
        

		public int Id
		{
			get { return 1; }
		}

		public double? this[int month]
		{
			get
			{
                var t1 = _dtos.FirstOrDefault(t => t.Month == month);
				return t1 == null ? null : (double?)UserProfile.ToUserUnits(t1.Temperature, PhysicalType.Temperature);
			}
			set
			{
			    var v = value.HasValue ? (double?)UserProfile.ToServerUnits(value.Value, PhysicalType.Temperature) : null;

			    if (v.HasValue)
			    {
			        if (v.Value < ValueRangeHelper.OldTemperatureRange.Min || v.Value > ValueRangeHelper.OldTemperatureRange.Max)
			        {
			            throw (new Exception(ValueRangeHelper.OldTemperatureRange.ViolationMessage));
			        }
                    

                    var t1 = _dtos.FirstOrDefault(t => t.Month == month);
			        if (t1 == null)
			        {
                        t1 = new CompStationCoolingRecomendedDTO { Month = month };
                        _dtos.Add(t1);
			        }
			        t1.Temperature = v.Value;
			    }
                _saveChanges(month, v);
			}
		}
	}
}