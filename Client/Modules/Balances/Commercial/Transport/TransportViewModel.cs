using System.Collections.Generic;
using System.Linq;
using System.Windows;
using GazRouter.Balances.Commercial.Common;
using GazRouter.Common.ViewModel;
using GazRouter.DataProviders.Balances;
using GazRouter.DTO.Balances.Transport;
using GazRouter.DTO.Dictionaries.BalanceItems;
using GazRouter.DTO.Dictionaries.Targets;

namespace GazRouter.Balances.Commercial.Transport
{ 
    public class TransportViewModel : ViewModelBase
    {
        public TransportViewModel()
        {
            
        }


        public async void LoadData(Target target, BalanceDataBase data, double coef)
        {
            ValueFormat = coef == 1 ? "#,0.000" : "#,0";

            var transportList = await new BalancesServiceProxy().GetTransportListAsync(
                new HandleTransportListParameterSet { ContractId = data.GetContract(target).Id });

            Items = new List<TransportItem>();

            foreach (var owner in data.Owners)
            {
                var ownerItem = new TransportGroup {Name = owner.Name};
                var lookup = transportList.Where(t => t.OwnerId == owner.Id).ToLookup(t => t.InletId, t => t);
                foreach (var transportGroup in lookup)
                {
                    var inletItem = new TransportGroup { Name = transportGroup.First().InletName };
                    ownerItem.Childs.Add(inletItem);

                    foreach (var transport in transportGroup)
                    {
                        inletItem.Childs.Add(
                            new TransportItem
                            {
                                Name = transport.OutletName,
                                BalItem = transport.BalanceItem,
                                Volume = transport.Volume * coef,
                                Length = transport.Length
                            });
                    }
                }

                if (ownerItem.Childs.Any())
                    Items.Add(ownerItem);
            }

            
            OnPropertyChanged(() => Items);
        }


        public List<TransportItem> Items { get; set; }


        public string ValueFormat { get; set; }

        public string DeltaFormat => $"+{ValueFormat};-{ValueFormat};#";

    }

    public class TransportItem
    {
        public TransportItem()
        {
            Childs = new List<TransportItem>();
        }

        public string Name { get; set; }

        public virtual double Volume { get; set; }

        public double? Length { get; set; }

        public virtual double Transport => Volume * (Length ?? 0) / 100;

        public BalanceItem? BalItem { get; set; }

        public List<TransportItem> Childs { get; set; }

        public virtual FontStyle FontStyle => FontStyles.Italic;
    }
    
    public class TransportGroup : TransportItem
    {
        public override double Volume => Childs.Sum(c => c.Volume);
        
        public override double Transport => Childs.Sum(c => c.Transport);

        public override FontStyle FontStyle => FontStyles.Normal;
    }

}
