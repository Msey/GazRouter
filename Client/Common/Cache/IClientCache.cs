namespace GazRouter.Common.Cache
{
    public interface IClientCache
    {
        IDictionaryRepository DictionaryRepository { get; set; }
    }
}