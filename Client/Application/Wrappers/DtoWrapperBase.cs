
namespace GazRouter.Application.Wrappers
{
    public class DtoWrapperBase<T>
    {
        public DtoWrapperBase(T dto)
        {
            Dto = dto;
        }

        public T Dto { get; }
    }
}
