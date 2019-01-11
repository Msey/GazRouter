using GazRouter.DAL.Core;

namespace GazRouter.DAL.ObjectModel.CompShops
{
    public class DeleteCompShopCommand : DeleteEntityCommand
    {
        public DeleteCompShopCommand(ExecutionContext context)
            : base(context)
        {
			IntegrityConstraints.Add("(RDI.FK_VALVES_SHOP)", "Компрессорный цех не может быть удален так как содержит краны");
        }

        protected override string Package
        {
            get { return "P_COMP_SHOP"; }
        }
    }
}