using System;
using GazRouter.Common.ViewModel;

namespace GazRouter.Flobus.Model
{
    public abstract class SchemeObject : PropertyChangedBase
    {
        public event Action NeedRefresh;
        protected void NotifyNeedRefresh()
        {
            NeedRefresh?.Invoke();
        }
    }
}