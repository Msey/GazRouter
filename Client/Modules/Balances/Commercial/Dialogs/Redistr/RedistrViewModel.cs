using System;
using System.Collections.Generic;
using System.Linq;
using GazRouter.Common.ViewModel;
using GazRouter.DTO.Balances.GasOwners;
using DelegateCommand = Microsoft.Practices.Prism.Commands.DelegateCommand;

namespace GazRouter.Balances.Commercial.Dialogs.Redistr
{
    public class RedistrViewModel : DialogViewModel<Action<Dictionary<int, double?>>> 
    {
        private readonly double _maxVolume;
        
        public RedistrViewModel(List<GasOwnerDTO> ownerList, double maxVolume, Action<Dictionary<int, double?>> actionBeforeClosing)
            : base(actionBeforeClosing)
        {
            Items = new List<RedistrItem>();

            foreach (var owner in ownerList)
            {
                var item = new RedistrItem {Owner = owner};
                item.PropertyChanged += (obj, args) =>
                {
                    OnPropertyChanged(() => CanRedistr);
                    OnPropertyChanged(() => WarnMessage);
                    SwapCommand.RaiseCanExecuteChanged();
                };
                Items.Add(item);
            }

            _maxVolume = maxVolume;
            SwapCommand = new DelegateCommand(() => DialogResult = true, () => CanRedistr);
        }
        
        public List<RedistrItem> Items { get; set; }
        
        public DelegateCommand SwapCommand { get; set; }

        private double TotalVolume => Items?.Sum(i => i.Volume) ?? 0;

        public bool CanRedistr => TotalVolume <= _maxVolume;

        public string WarnMessage
            =>
                !CanRedistr
                    ? $"Недопустимо!!! Распределенный объем ({TotalVolume:#,0.###}) превышает распределяемый ({_maxVolume:#,0.###})."
                    : "";

        protected override void InvokeCallback(Action<Dictionary<int, double?>> closeCallback)
        {
            closeCallback?.Invoke(Items.Where(i => i.Volume.HasValue).ToDictionary(i => i.Owner.Id, i => i.Volume));
        }
    }


    public class RedistrItem : PropertyChangedBase
    {
        private double? _volume;
        public GasOwnerDTO Owner { get; set; }

        public double? Volume
        {
            get { return _volume; }
            set { SetProperty(ref _volume, value); }
        }
    }
}
