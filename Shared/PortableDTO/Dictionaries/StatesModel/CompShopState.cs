
namespace GazRouter.DTO.Dictionaries.StatesModel
{
    public enum CompShopState
    {
        WorkingOnOwnPipeline = 0,
        OnPass = 1,
        WorkingViaIntershopJumpers = 2,
        Reserve = 3,
        Failure = 4,
        Repair = 5,
        Undefined = 9
    }
}
