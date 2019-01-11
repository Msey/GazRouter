namespace GazRouter.Controls.Trends
{
    public partial class TrendsWindow
    {
		public TrendsWindow()
        {
            InitializeComponent();
        }

		public new TrendsViewModel DataContext
		{
			get { return (TrendsViewModel)base.DataContext; }
			set
			{
				base.DataContext = value;
				TrendsControl.DataContext = (TrendsViewModel)base.DataContext;

			}
		}
    }
}