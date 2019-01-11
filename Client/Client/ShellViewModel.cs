using GazRouter.Client.Menu;

namespace GazRouter.Client
{
    internal static class ModulesNames
    {
        public const string ActionsRolesUsers = "ActionsRolesUsers";

        public const string Balances = "BalancesModelModel";

        public const string GasLeaks = "GasLeaksId";

        public const string ManualInput = "ManualInput";

        public const string Modes = "ModesModuleId";

        public const string ObjectModel = "ObjectModel";

        public const string RepairModel = "RepairModel";
    }

    public class ShellViewModel
    {
        public ShellViewModel()
        {
            Menu = new MainMenuViewModel();
            
        }
        
        public MainMenuViewModel Menu { get; }
       
    }
}