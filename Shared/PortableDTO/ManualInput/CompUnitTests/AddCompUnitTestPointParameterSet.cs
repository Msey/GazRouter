namespace GazRouter.DTO.ManualInput.CompUnitTests
{
    public class AddCompUnitTestPointParameterSet
    {
        /// <summary>
        /// Id типа нагнетателя
        /// </summary>
        public int CompUnitTestId { get; set; }

        /// <summary>
        /// Тип линии                                            
        /// </summary>
        public int LineType { get; set; }

        /// <summary>
        /// Координата по оси ОХ                                         
        /// </summary>
        public double X { get; set; }

        /// <summary>
        /// Координата по оси ОУ                                      
        /// </summary>
        public double Y { get; set; }
        
    }
}
