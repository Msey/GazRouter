using System;

namespace GazRouter.Common.ViewModel
{
    public abstract class DialogViewModel : DialogViewModel<Action>
    {
        protected DialogViewModel(Action closeCallback) : base(closeCallback)
        {
        }

        protected override void InvokeCallback(Action closeCallback)
        {
            closeCallback();
        }
    }
}