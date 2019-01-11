namespace GazRouter.DataExchange.ASUTP
{
    public class GroupItem : ItemBase
    {
        private readonly string _name;

        public GroupItem(string name)
        {
            _name = name;
        }

        public override string Name => _name;
    }
}