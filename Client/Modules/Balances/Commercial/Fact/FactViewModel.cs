using System.Linq;
using GazRouter.Balances.Commercial.Common;
using GazRouter.Balances.Commercial.Dialogs.DivideVolume;
using GazRouter.Balances.Commercial.Dialogs.Redistr;
using GazRouter.Balances.Commercial.Dialogs.Swap;
using GazRouter.Balances.Commercial.InputState;
using GazRouter.Balances.Commercial.SwapSummary;
using GazRouter.DataProviders.Balances;
using GazRouter.DTO.Balances.MonthAlgorithms;
using GazRouter.DTO.Balances.Swaps;
using GazRouter.DTO.Dictionaries.Targets;
using Microsoft.Practices.Prism.Regions;
using DelegateCommand = Microsoft.Practices.Prism.Commands.DelegateCommand;

namespace GazRouter.Balances.Commercial.Fact
{
    [RegionMemberLifetime(KeepAlive = false)]
    public class FactViewModel : PlanFactViewModelBase
    {
        public FactViewModel()
            : base(Target.Fact)
        {
            _itemActions.RedistrAction = Redistr;
            _itemActions.SwapAction = Swap;
            _itemActions.UnswapAction = Unswap;

            MakeBalanceByMeasuringsCommand = new DelegateCommand(MakeBalanceByMeasurings, () => _isEditPermission && !IsFinal);
            MoveGasSupplyCommand = new DelegateCommand(MoveGasSupply, () => _isEditPermission && !IsFinal);
            OwnersDaySumCommand = new DelegateCommand(OwnersDaySum, () => _isEditPermission && !IsFinal);

            Load();
        }


        public InputStateViewModel InputState { get; set; }

        public SwapSummaryViewModel SwapSummary { get; set; }

        protected override void UpdateCommands()
        {
            base.UpdateCommands();
            MakeBalanceByMeasuringsCommand.RaiseCanExecuteChanged();
            MoveGasSupplyCommand.RaiseCanExecuteChanged();
            OwnersDaySumCommand.RaiseCanExecuteChanged();
        }

        protected override void OnLoadComplete()
        {
            InputState = new InputStateViewModel(_data);
            OnPropertyChanged(() => InputState);
            SwapSummary = new SwapSummaryViewModel(_data);
            OnPropertyChanged(() => SwapSummary);
        }
        
        #region ФОРМИРОВАНИЕ БАЛАНСА ГАЗА

        public DelegateCommand MakeBalanceByMeasuringsCommand { get; set; }

        private void MakeBalanceByMeasurings()
        {
            if (!CheckChanges()) return;

            var vm =
                new DivideVolumeViewModel(
                    _data.Owners.Where(o => o.SystemList.Any(s => s == SelectedSystem.Id)).ToList(), null,
                    _data.FactContract.Id, Load);
            var v = new DivideVolumeView { DataContext = vm };
            v.ShowDialog();
        }


        public DelegateCommand OwnersDaySumCommand { get; set; }

        private void OwnersDaySum()
        {
            if (!CheckChanges()) return;

            Lock();
            new BalancesServiceProxy().RunOwnersDaySumAlgorithmAsync(
                new OwnersDaySumAlgorithmParameterSet {ContractId = _data.FactContract.Id});
            Unlock();

            Load();
        }


        #endregion


        #region  ПЕРЕНОС ЗАПАСА ГАЗА

        public DelegateCommand MoveGasSupplyCommand { get; set; }

        private async void MoveGasSupply()
        {
            if (!CheckChanges()) return;

            Lock();
            await new BalancesServiceProxy().RunMoveGasSupplyAlgorithmAsync(_data.FactContract.Id);
            Unlock();

            Load();
        }

        #endregion


        #region ПЕРЕРАСПРЕДЕЛЕНИЕ ОБЪЕМА ГАЗА

        private void Redistr(OwnerItem srcOwnerItem)
        {
            if (srcOwnerItem == null) return;

            var ownerList =
                _data.GetVisibleOwnerList(srcOwnerItem.Entity.Id, srcOwnerItem.BalItem)
                    .Where(o => o.Id != srcOwnerItem.Owner.Id)
                    .ToList();

            var vm = new RedistrViewModel(
                ownerList,
                srcOwnerItem.FactBase ?? 0,
                result =>
                {
                    foreach (var o in result)
                    {
                        var destOwnerItem = _treeBuilder.GetOwnerItem(srcOwnerItem.Entity.Id, o.Key, srcOwnerItem.BalItem);
                        destOwnerItem.FactBase = (destOwnerItem.FactBase ?? 0) + o.Value;
                    }
                    srcOwnerItem.FactBase = srcOwnerItem.FactBase - result.Sum(i => i.Value);

                });
            var v = new RedistrView { DataContext = vm };
            v.ShowDialog();
        }

        #endregion


        #region ЗАМЕЩЕНИЕ ГАЗА
        
        private void Swap(OwnerItem srcOwnerItem)
        {
            if (srcOwnerItem == null) return;

            var ownerList =
                _data.GetVisibleOwnerList(srcOwnerItem.Entity.Id, srcOwnerItem.BalItem)
                    .Where(o => o.Id != srcOwnerItem.Owner.Id)
                    .ToList();

            var vm = new SwapViewModel(
                ownerList,
                srcOwnerItem.FactBase ?? 0,
                (ownerId, vol) =>
                {
                    var destOwnerItem = _treeBuilder.GetOwnerItem(srcOwnerItem.Entity.Id, ownerId, srcOwnerItem.BalItem);
                    if (destOwnerItem != null)
                    {
                        srcOwnerItem.FactCorrected = srcOwnerItem.FactBase - vol;
                        destOwnerItem.FactBase = destOwnerItem.FactBase ?? 0;
                        destOwnerItem.FactCorrected = (destOwnerItem.FactBase ?? 0) + vol;

                        _data.SwapList.Add(new SwapDTO
                        {
                            EntityId = srcOwnerItem.Entity.Id,
                            EntityType = srcOwnerItem.Entity.EntityType,
                            EntityName = srcOwnerItem.Entity.ShortPath,
                            BalItem = srcOwnerItem.BalItem,
                            SrcOwnerId = srcOwnerItem.Owner.Id,
                            SrcOwnerName = srcOwnerItem.Owner.Name,
                            DestOwnerName = destOwnerItem.Owner.Name,
                            DestOwnerId = destOwnerItem.Owner.Id,
                            Volume = vol
                        });

                        srcOwnerItem.UpdateActions();

                        SwapSummary = new SwapSummaryViewModel(_data);
                        OnPropertyChanged(() => SwapSummary);
                    }
                    
                });
            var v = new SwapView {DataContext = vm};
            v.ShowDialog();
        }


        private void Unswap(OwnerItem ownerItem)
        {
            if (ownerItem == null) return;

            var swap =
                _data.SwapList.SingleOrDefault(
                    s =>
                        s.EntityId == ownerItem.Entity.Id && s.BalItem == ownerItem.BalItem &&
                        (s.SrcOwnerId == ownerItem.Owner.Id || s.DestOwnerId == ownerItem.Owner.Id));

            if (swap != null)
            {
                var srcOwnerItem = _treeBuilder.GetOwnerItem(swap.EntityId, swap.SrcOwnerId, swap.BalItem);
                var destOwnerItem = _treeBuilder.GetOwnerItem(swap.EntityId, swap.DestOwnerId, swap.BalItem);
                _data.SwapList.Remove(swap);
                srcOwnerItem.FactCorrected = null;
                destOwnerItem.FactCorrected = null;

                srcOwnerItem.UpdateActions();
                destOwnerItem.UpdateActions();

                SwapSummary = new SwapSummaryViewModel(_data);
                OnPropertyChanged(() => SwapSummary);
            }
        }

        #endregion

    }
}
