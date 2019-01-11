namespace GazRouter.Common.ViewModel
{
    public abstract class AsyncViewModelBase : PropertyChangedBase
    {
        public  IClientBehavior Behavior { get; protected set; } = new StandardBehavior();
    }
}