using System.Collections.Generic;
using System.Linq;
using System.Windows;
using GazRouter.Application;
using GazRouter.Common.Cache;
using GazRouter.Controls.Tree;
using GazRouter.DTO.ObjectModel;
using GazRouter.DTO.ObjectModel.Inconsistency;
using GazRouter.ObjectModel.Model.Dialogs;
using GazRouter.ObjectModel.Model.Dialogs.Errors;
using Microsoft.Practices.ServiceLocation;

namespace GazRouter.ObjectModel.Model.Pipelines
{
    public class ValidatedEntityNode : EntityNode
    {
		public List<InconsistencyWrap> Errors { get { return _validationErrors; } }
		private readonly List<InconsistencyWrap> _validationErrors;
        public ValidatedEntityNode(CommonEntityDTO entity, List<InconsistencyDTO> validationErrors) : base(entity)
        {
			_validationErrors = validationErrors.Select(t => new InconsistencyWrap(t)
			{
				Inconsistency =
					ClientCache.DictionaryRepository.InconsistencyTypes.First(
					p =>p.InconsistencyType == t.InconsistencyTypeId)
			}).ToList();
            ErrImageVisibility = _validationErrors.Any() ? Visibility.Visible : Visibility.Collapsed;
        }
        private static IClientCache ClientCache => ServiceLocator.Current.GetInstance<IClientCache>();

        public Visibility ErrImageVisibility { get; private set; }

	    public string ErrorsToolTip
	    {
			get { return Errors.Aggregate("", (current, error) => current + (string.IsNullOrEmpty(current) ? "" : System.Environment.NewLine) + (error.Inconsistency.Name)); }
	    }
    }
}