using System;
using GazRouter.Common.ViewModel;
using GazRouter.DataProviders.ObjectModel;
using GazRouter.DTO.ObjectModel.Entities;
using Microsoft.Practices.Prism.Commands;

namespace GazRouter.ObjectModel.Model.Dialogs.AddEdit.Comment
{
	public class AddEditCommentViewModel : DialogViewModel
    {
        private readonly Guid _entityId;
	    private string _comment;

	    public AddEditCommentViewModel(Action actionBeforeClosing, Guid entityId, string comment)
            : base(actionBeforeClosing)
        {
            _entityId = entityId;
	        _comment = comment;

            SaveCommand = new DelegateCommand(Save);
		}


	    public string Comment
	    {
	        get { return _comment; }
	        set
	        {
	            _comment = value;
	            OnPropertyChanged(() => Comment);
	        }
	    }


	    public DelegateCommand SaveCommand { get; private set; }


	    private async void Save()
	    {
	        try
	        {
                Behavior.TryLock();
	            await new ObjectModelServiceProxy().AddDescriptionAsync(
	                new AddDescriptionParameterSet
	                {
	                    EntityId = _entityId,
	                    Description = Comment
	                });
	        }
	        finally
	        {
	            Behavior.TryUnlock();
	            DialogResult = true;
	        }
	    }
    }
}