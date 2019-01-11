namespace Balances.Irregularity
{
    public partial class IrregularityWindow
    {
		public IrregularityWindow()
        {
            InitializeComponent();
        }

		public new IrregularityViewModel DataContext
		{
			get { return (IrregularityViewModel)base.DataContext; }
			set
			{
				base.DataContext = value;
			}
		}
    }
}