using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using Utils.Extensions;

namespace GazRouter.Common.ViewModel
{
    public abstract class ValidationViewModel : LockableViewModel, INotifyDataErrorInfo
    {
        private readonly List<PropertyValidation<ValidationViewModel>> _validations =
            new List<PropertyValidation<ValidationViewModel>>();
        private Dictionary<string, List<string>> _errorMessages = new Dictionary<string, List<string>>();
        protected ValidationViewModel()
        {
            PropertyChanged += (s, e) =>
            {
                if (!ValidateExclude(e.PropertyName))
                {
                    ValidateProperty(e.PropertyName);
                }
            };
        }
        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;
        public List<string> Errors
        {
            get { return _errorMessages.SelectMany(errorMessage => errorMessage.Value).ToList(); }
        }
        public bool HasErrors => _errorMessages.Count > 0;
        public IEnumerable GetErrors(string propertyName)
        {
            if (_errorMessages.ContainsKey(propertyName))
            {
                return _errorMessages[propertyName];
            }

            return new string[0];
        }
        public void ClearValidations()
        {
            _validations.Clear();
        }
        protected virtual bool ValidateExclude(string propertyName)
        {
            switch (propertyName)
            {
                case nameof(HasErrors):
                case nameof(Errors):

                    return true;
                default:
                    return false;
            }
        }
        protected void ValidateProperty(string propertyName)
        {
            _errorMessages.Remove(propertyName);

            _validations.Where(v => v.PropertyName == propertyName).ToList().ForEach(PerformValidation);
            OnErrorsChanged(propertyName);
            OnPropertyChanged(() => HasErrors);
        }
        protected PropertyValidation<ValidationViewModel> AddValidationFor<TProperty>(
            Expression<Func<TProperty>> property)
        {
            var validation = new PropertyValidation<ValidationViewModel>(property.GetMemberInfo().Name);
            _validations.Add(validation);
            return validation;
        }
        protected void ValidateAll()
        {
            var propertyNamesWithValidationErrors = _errorMessages.Keys;
            _errorMessages = new Dictionary<string, List<string>>();
            _validations.ForEach(PerformValidation);
            var propertyNamesThatMightHaveChangedValidation =
                _errorMessages.Keys.Union(propertyNamesWithValidationErrors).ToList();
            foreach (var propertyName in propertyNamesThatMightHaveChangedValidation)
                OnErrorsChanged(propertyName);
            OnPropertyChanged(() => HasErrors);
            OnPropertyChanged(() => Errors);
        }
        private void OnErrorsChanged(string propertyName)
        {
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
        }
        private void PerformValidation(PropertyValidation<ValidationViewModel> validation)
        {
            if (validation.IsInvalid())
            {
                AddErrorMessageForProperty(validation.PropertyName, validation.GetErrorMessage());
            }
        }
        private void AddErrorMessageForProperty(string propertyName, string errorMessage)
        {
            if (_errorMessages.ContainsKey(propertyName))
            {
                _errorMessages[propertyName].Add(errorMessage);
            }
            else
            {
                _errorMessages.Add(propertyName, new List<string> {errorMessage});
            }
        }
    }
}