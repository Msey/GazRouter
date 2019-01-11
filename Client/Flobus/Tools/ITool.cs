namespace GazRouter.Flobus.Tools
{
    public interface ITool
    {
        bool DeactivateTool();
        bool IsEnabled { get; set; }
        bool IsActive { get; }
        string Name { get; }
        bool ActivateTool();
    }
}