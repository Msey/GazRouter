namespace GazRouter.Flobus.Visuals
{
    public abstract class ShapeWidgetBase : WidgetBase
    {
        protected ShapeWidgetBase(Schema schema) : base(schema)
        {
            BindVisibilityToVirtualizationVisibility();
        }

        public double RotationAngle => 0;

    }
}