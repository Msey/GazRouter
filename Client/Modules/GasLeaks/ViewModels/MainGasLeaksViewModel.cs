using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using GazRouter.Common.ViewModel;
using GazRouter.Controls;
using GazRouter.DataProviders.GasLeaks;
using GazRouter.DTO.GasLeaks;
using GazRouter.GasLeaks.Views;
using Microsoft.Practices.Prism;
using Microsoft.Practices.Prism.Regions;
using Telerik.Windows.Controls;
using Utils.Extensions;
using DelegateCommand = Microsoft.Practices.Prism.Commands.DelegateCommand;

namespace GazRouter.GasLeaks.ViewModels
{
    [RegionMemberLifetime(KeepAlive = false)]
    public class MainGasLeaksViewModel : MainViewModelBase
    {
        private ExtLeakDTO _selectedLeak;

        public MainGasLeaksViewModel()
        {
            GasLeakList = new ObservableCollection<ExtLeakDTO>();

            AddCommand = new DelegateCommand(AddLeak, CanAddLeak);
            EditCommand = new DelegateCommand(EditLeak, CanEditLeak);
            DeleteCommand = new DelegateCommand(DeleteLeak, CanDeleteLeak);
            RefreshCommand = new DelegateCommand(Refresh);
        }

        public ObservableCollection<ExtLeakDTO> GasLeakList { get; set; }


        public DelegateCommand RefreshCommand { get; set; }

        public DelegateCommand EditCommand { get; set; }

        public ExtLeakDTO SelectedLeak
        {
            get { return _selectedLeak; }
            set
            {
                _selectedLeak = value;
                OnPropertyChanged(() => SelectedLeak);
                RefreshCommands();
            }
        }

        private PeriodDates _selectedPeriodDates = new PeriodDates
        {
            BeginDate = new DateTime(DateTime.Now.Year, 1, 1, 0, 0, 0, DateTimeKind.Local),
            EndDate = DateTime.Now.AddDays(1)
        };
        public PeriodDates SelectedPeriodDates
        {
            get { return _selectedPeriodDates; }
            set
            {
                _selectedPeriodDates = value;
                OnPropertyChanged(() => SelectedPeriodDates);
                Refresh();
            }
        }
        
        public DelegateCommand AddCommand { get; private set; }
        public DelegateCommand DeleteCommand { get; private set; }

        private bool CanEditLeak()
        {
            return SelectedLeak != null;
        }

        private void EditLeak()
        {
            var viewModel = new AddEditGasLeakViewModel(id => Refresh(), SelectedLeak);
            var view = new AddEditGasLeakView {DataContext = viewModel};
            view.ShowDialog();
        }

        private void DeleteLeak()
        {
            MessageBoxProvider.Confirm("Удалить утечку", confirmed =>
                        {
                            if (confirmed)
                            {
                                Delete();
                            }
                        },
                    cancelButtonText: "Отмена"
                );
        }

        private async void Delete()
        {
            Behavior.TryLock();
            await new GasLeaksServiceProxy().DeleteLeakAsync(SelectedLeak.Id);
            Refresh();
            Behavior.TryUnlock();
        }

        private bool CanDeleteLeak()
        {
            return SelectedLeak != null;
        }

        private bool CanAddLeak()
        {
            return true;
        }

        private void AddLeak()
        {
            var viewModel = new AddEditGasLeakViewModel(id => Refresh());
            RadWindow view = new AddEditGasLeakView {DataContext = viewModel};
            view.ShowDialog();
        }


        public async void Refresh()
        {
            Behavior.TryLock();
            var leakList =
                await
                    new GasLeaksServiceProxy().GetLeaksAsync(new GetLeaksParameterSet
                    {
                        StartDate = SelectedPeriodDates.BeginDate.Value.ToLocal(),
                        EndDate = SelectedPeriodDates.EndDate.Value.ToLocal()
                    });

            GasLeakList.Clear();
            GasLeakList.AddRange(leakList.Select(c => new ExtLeakDTO(c)));
            OnPropertyChanged(() => GasLeakList);
            OnPropertyChanged(() => VolumeDaySummary);
            OnPropertyChanged(() => VolumeTotalSummary);
            OnPropertyChanged(() => YVolumeDayMaximumValue);
            OnPropertyChanged(() => YVolumeTotalMaximumValue);

            Behavior.TryUnlock();
        }
        
        private void RefreshCommands()
        {
            AddCommand.RaiseCanExecuteChanged();
            DeleteCommand.RaiseCanExecuteChanged();
            EditCommand.RaiseCanExecuteChanged();
        }

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            Refresh();
        }

       
		#region Charing

	    public List<LeaksSummary> VolumeDaySummary
	    {
		    get
		    {
			    return
				    GasLeakList.GroupBy(p => p.SiteName)
				               .Select(p => new LeaksSummary {SiteName = p.Key, Total = p.Sum(t => t.VolumeDay)}).ToList();
		    }
	    }

		public List<LeaksSummary> VolumeTotalSummary
		{
			get
			{
				return
					GasLeakList.GroupBy(p => p.SiteName)
							   .Select(p => new LeaksSummary { SiteName = p.Key, Total = p.Sum(t => t.VolumeTotal) }).ToList();
			}
		}

		public int YVolumeDayMaximumValue
		{
			get
			{
				var eMax = VolumeDaySummary.Count == 0 ? 0 : VolumeDaySummary.Max(s => s.Total);
				return (int)eMax + Math.Max(10, (int)eMax / 20);
			}
		}

		public int YVolumeTotalMaximumValue
		{
			get
			{
				var eMax = VolumeTotalSummary.Count == 0 ? 0 : VolumeTotalSummary.Max(s => s.Total);
				return (int)eMax + Math.Max(20, (int)eMax / 10);
			}
		}

		#endregion
    }

	public class LeaksSummary
    {
        public string SiteName { get; set; }
        public double Total { get; set; }
    }
}