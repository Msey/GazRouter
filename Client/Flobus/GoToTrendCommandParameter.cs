using GazRouter.DTO.Dictionaries.PropertyTypes;
using GazRouter.DTO.ObjectModel;

namespace GazRouter.Flobus
{
    public class GoToTrendCommandParameter
    {
        #region Constructors and Destructors

        public GoToTrendCommandParameter(CommonEntityDTO entity, PropertyType propertyType)
        {
            Entity = entity;
            PropertyType = propertyType;
        }

        #endregion

        #region Public Properties

        public CommonEntityDTO Entity { get; }

        public PropertyType PropertyType { get; }

        #endregion
    }
}