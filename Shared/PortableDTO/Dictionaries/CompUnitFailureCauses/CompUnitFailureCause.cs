
namespace GazRouter.DTO.Dictionaries.CompUnitFailureCauses
{
   /// <summary>
   /// Причина останова ГПА
   /// </summary>
    public enum CompUnitFailureCause 
    {
        // Не определена
        None = 0,

        // Эксплуатационные, связанные с нарушением ПТЭ
        Operational = 11,	
       
        //Ремонтные, связ. с наруш. тех. док-ции на ремонт
        Repair = 12,	

        //Заводские (констр. дефекты, дефекты изготовителя)
        Factory = 13
    }
}