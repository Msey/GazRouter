namespace GazRouter.Balances.Commercial.BalanceDiagram
{
    public class BalanceDiagramValue
    {
        public BalanceDiagramValue(string name, double? value)
        {
            CategoryName = name;
            Value = value;
        }

        public string CategoryName { get; set; }

        public double? Value { get; set; }
    }
}