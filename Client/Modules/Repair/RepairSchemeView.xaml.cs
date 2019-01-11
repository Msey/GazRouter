using GazRouter.Flobus;

namespace GazRouter.Repair
{
    public partial class RepairSchemeView
    {
        public RepairSchemeView()
        {
            InitializeComponent(); 
        }

        public Schema Schema => SchemaControl;
    }
}
