using GazRouter.DTO.Dictionaries.PropertyTypes;

namespace GazRouter.Modes.GasCosts.GasCompessibility
{
    public class ComponentsBinaryInteraction
    {
        /// <summary>
        /// 
        /// </summary>
        public PropertyType Component1 { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public PropertyType Component2 { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public double Eij { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public double Vij { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public double Kij { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public double Gij { get; set; }
    }
}