using GazRouter.Common;

namespace GazRouter.Balances.Common.TreeGroupType
{
    public class BalanceGroupTypes
    {
        public static TreeGroupType MeasStationGroupType
        {
            get { return IsolatedStorageManager.Get<TreeGroupType?>("BalanceMeasStationsGroupType") ?? TreeGroupType.ByEnterprise; }
            set { IsolatedStorageManager.Set("BalanceMeasStationsGroupType", value); }
        }

        public static TreeGroupType DistrStationGroupType
        {
            get { return IsolatedStorageManager.Get<TreeGroupType?>("BalanceDistrStationsGroupType") ?? TreeGroupType.ByRegionAndSite; }
            set { IsolatedStorageManager.Set("BalanceDistrStationsGroupType", value); }
        }

        public static TreeGroupType OperConsumerGroupType
        {
            get { return IsolatedStorageManager.Get<TreeGroupType?>("BalanceOperConsumerGroupType") ?? TreeGroupType.ByRegionAndSite; }
            set { IsolatedStorageManager.Set("BalanceOperConsumerGroupType", value); }
        }
    }
}