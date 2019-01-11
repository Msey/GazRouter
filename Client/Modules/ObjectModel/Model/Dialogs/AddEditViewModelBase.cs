using System;
using GazRouter.DTO;
using GazRouter.DTO.ObjectModel;

namespace GazRouter.ObjectModel.Model.Dialogs
{
    public abstract class AddEditViewModelBase<TEnt, TKey> : Common.ViewModel.AddEditViewModelBase<TEnt, TKey>
        where TEnt : BaseDto<TKey>, new()
        where TKey : struct
    {
        protected AddEditViewModelBase(Action<TKey> closeCallback)
            : base(closeCallback)
        {
        }

        protected AddEditViewModelBase(Action<TKey> closeCallback, TEnt model)
            : base(closeCallback, model)
        {
        }
    }

    public abstract class AddEditViewModelBase<TEnt> : Common.ViewModel.AddEditViewModelBase<TEnt, Guid>
        where TEnt : CommonEntityDTO, new()
    {
        protected AddEditViewModelBase(Action<Guid> closeCallback)
            : base(closeCallback)
        {
        }

        protected AddEditViewModelBase(Action<Guid> closeCallback, TEnt model)
            : base(closeCallback, model)
        {
            Name = model.Name;
        }
    }
}