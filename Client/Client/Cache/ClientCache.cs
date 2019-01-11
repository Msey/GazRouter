using GazRouter.Common.Cache;

namespace GazRouter.Client.Cache
{
    public  class ClientCache : IClientCache
    {
        private static IDictionaryRepository _dictionaryRepository = new DictionaryRepositoryCache();

        public  IDictionaryRepository DictionaryRepository
        {
            get { return _dictionaryRepository; }
            set { _dictionaryRepository = value; }
        }
    }
}