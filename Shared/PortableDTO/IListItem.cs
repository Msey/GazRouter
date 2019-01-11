namespace GazRouter.DTO
{
    public interface IListItem<out TId>
    {
        TId Id { get; }
    }
}
