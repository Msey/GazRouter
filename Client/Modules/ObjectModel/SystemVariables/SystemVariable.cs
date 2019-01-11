using GazRouter.Common.ViewModel;
using GazRouter.DTO.SystemVariables;

namespace GazRouter.ObjectModel.SystemVariables
{
    public class SystemVariable : PropertyChangedBase
    {

        public SystemVariable(IusVariableDTO variable, SystemVariablesViewModel vm)
        {
            Name = variable.Name;
            Vm = vm;
            Description = variable.Description;
            Value = variable.Value;
        }

        public SystemVariable()
        {
        }

        public string Description { get; set; }

        private string _value;

        public string Value
        {
            get { return _value; }
            set
            {
                if (SetProperty(ref _value, value))
                    Vm.Save(this);
            }
        }

        public string Name { get;  }

        public SystemVariablesViewModel Vm { get; }

        public bool Equals(SystemVariable other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.Name, Name);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (SystemVariable)) return false;
            return Equals((SystemVariable) obj);
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }
    }
}