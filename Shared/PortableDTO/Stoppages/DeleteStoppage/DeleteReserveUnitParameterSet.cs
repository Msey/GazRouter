using System;

namespace GazRouter.DTO.Stoppages.DeleteStoppage
{
   public class DeleteReserveUnitParameterSet
    {
       public int StoppageId { get; set; }
       public Guid ReserveGpaId { get; set; }
    }
}
