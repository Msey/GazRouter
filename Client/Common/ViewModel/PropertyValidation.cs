using System;

namespace GazRouter.Common.ViewModel
{
    public class PropertyValidation<TBindingModel>
     where TBindingModel : class
    {
        private Func<bool> _validationCriteria;
        private string _errorMessage;
        private readonly string _propertyName;

        public PropertyValidation(string propertyName)
        {
            _propertyName = propertyName;
        }

        public PropertyValidation<TBindingModel> When(Func<bool> validationCriteria)
        {
            if (_validationCriteria != null)
                throw new InvalidOperationException("You can only set the validation criteria once.");

            _validationCriteria = validationCriteria;
            return this;
        }

        public PropertyValidation<TBindingModel> Show(string errorMessage)
        {
            if (_errorMessage != null)
                throw new InvalidOperationException("You can only set the message once.");

            _errorMessage = errorMessage;
            return this;
        }

        public bool IsInvalid()
        {
            if (_validationCriteria == null)
                throw new InvalidOperationException(
                    "No criteria have been provided for this validation. (Use the 'When(..)' method.)");

            return _validationCriteria();
        }

        public string GetErrorMessage()
        {
            if (_errorMessage == null)
                throw new InvalidOperationException(
                    "No error message has been set for this validation. (Use the 'Show(..)' method.)");

            return _errorMessage;
        }

        public string PropertyName
        {
            get { return _propertyName; }
        }
    }
}