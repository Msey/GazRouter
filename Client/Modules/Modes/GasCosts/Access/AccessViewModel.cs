using System;
using System.Collections.Generic;
using System.Linq;
using GazRouter.Application;
using GazRouter.Common.ViewModel;
using GazRouter.DataProviders.GasCosts;
using GazRouter.DTO.GasCosts;
using Microsoft.Practices.Prism.Commands;

namespace GazRouter.Modes.GasCosts.Access
{
    public class AccessViewModel : DialogViewModel
	{
        public AccessViewModel(DateTime date, Action closeCallback)
            :base(closeCallback)
        {
            Date = date;

            AllowAllCommand = new DelegateCommand(() => Mark(true));
            AllowNormCommand = new DelegateCommand(() => AccessList.ForEach(a => a.Norm = true));
            AllowPlanCommand = new DelegateCommand(() => AccessList.ForEach(a => a.Plan = true));
            AllowFactCommand = new DelegateCommand(() => AccessList.ForEach(a => a.Fact = true));

            RestrictAllCommand = new DelegateCommand(() => Mark(false));
            RestrictNormCommand = new DelegateCommand(() => AccessList.ForEach(a => a.Norm = false));
            RestrictPlanCommand = new DelegateCommand(() => AccessList.ForEach(a => a.Plan = false));
            RestrictFactCommand = new DelegateCommand(() => AccessList.ForEach(a => a.Fact = false));

            SaveCommand = new DelegateCommand(UpdateAccessList);

            LoadAccessList();
        }


        public List<AccessRecord> AccessList { get; set; }

        public DateTime Date { get; set; }

        public DelegateCommand SaveCommand { get; set; }


        private async void LoadAccessList()
        {
            AccessList = new List<AccessRecord>();
            try
            {
                Behavior.TryLock();
                AccessList =
                    (await
                        new GasCostsServiceProxy().GetGasCostAccessListAsync(new GetGasCostAccessListParameterSet
                        {
                            EnterpriseId = UserProfile.Current.Site.Id,
                            Date = Date,
                            PeriodType = DTO.Dictionaries.PeriodTypes.PeriodType.Month
                        })).Select(a => new AccessRecord(a)).ToList();
                OnPropertyChanged(() => AccessList);
            }
            finally
            {
                Behavior.TryUnlock();
            }
        }

        private async void UpdateAccessList()
        {
            try
            {
                Behavior.TryLock();
                await new GasCostsServiceProxy().UpdateGasCostAccessListAsync(AccessList.Select(a => { a.Dto.PeriodType = DTO.Dictionaries.PeriodTypes.PeriodType.Month; return a.Dto; }).ToList());
                DialogResult = true;
            }
            finally
            {
                Behavior.TryUnlock();
            }
        }

        
        #region ACTIONS

        public DelegateCommand AllowAllCommand { get; set; }
        public DelegateCommand AllowNormCommand { get; set; }
        public DelegateCommand AllowPlanCommand { get; set; }
        public DelegateCommand AllowFactCommand { get; set; }
        
        public DelegateCommand RestrictAllCommand { get; set; }
        public DelegateCommand RestrictNormCommand { get; set; }
        public DelegateCommand RestrictPlanCommand { get; set; }
        public DelegateCommand RestrictFactCommand { get; set; }

        private void Mark(bool param)
        {
            foreach (var ac in AccessList)
            {
                ac.Fact = param;
                ac.Plan = param;
                ac.Norm = param;
            }
        }
        
        #endregion


        

	}


    public class AccessRecord : ViewModelBase
    {
        private readonly GasCostAccessDTO _dto;

        public AccessRecord(GasCostAccessDTO dto)
        {
            _dto = dto;
        }

        public GasCostAccessDTO Dto
        {
            get { return _dto; }
        }

        public string SiteName 
        {
            get { return _dto.SiteName; }
        }

        public bool Norm
        {
            get { return _dto.Norm; }
            set
            {
                _dto.Norm = value;
                OnPropertyChanged(() => Norm);
            }
        }

        public bool Plan
        {
            get { return _dto.Plan; }
            set
            {
                _dto.Plan = value;
                OnPropertyChanged(() => Plan);
            }
        }

        public bool Fact
        {
            get { return _dto.Fact; }
            set
            {
                _dto.Fact = value;
                OnPropertyChanged(() => Fact);
            }
        }
    }


    
}
