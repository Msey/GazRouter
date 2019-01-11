using GazRouter.DTO.Dictionaries.PropertyTypes;

namespace GazRouter.Modes.GasCosts.GasCompessibility
{
    public class ComponentContent
    {
        /// <summary>
        /// Компонент газа, наименование
        /// </summary>
        public PropertyType Component { get; set; }

        /// <summary>
        /// Содержание компонента, мол. доля
        /// </summary>
        public double Concentration { get; set; }
    }
}