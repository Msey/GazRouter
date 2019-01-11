using System.ComponentModel.DataAnnotations;

namespace GazRouter.Modes.GasCosts.Dialogs.Model
{
    public class SealingType
    {
        #region Public Properties

        [Display(Name = "Наименование")]
        public string Name { get; set; }

        [Display(Name = "Идентификатор")]
        public int Id { get; set; }

        #endregion

        public override string ToString()
        {
            return Name;
        }

        protected bool Equals(SealingType other)
        {
            return string.Equals(Name, other.Name) && Id.Equals(other.Id);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }
            if (ReferenceEquals(this, obj))
            {
                return true;
            }
            if (obj.GetType() != GetType())
            {
                return false;
            }
            return Equals((SealingType)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Name != null ? Name.GetHashCode() : 0) * 397) ^ Id.GetHashCode();
            }
        }
    }
}