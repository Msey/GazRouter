using System.Collections.Generic;

namespace Utils.Extensions
{
    public static class DictionaryExtensions
    {
        public static TValue GetOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dic, TKey key)
        {
            return dic.ContainsKey(key) ? dic[key] : default(TValue);
        }

        public static TValue GetOrInsertNew<TKey, TValue>(this IDictionary<TKey, TValue> dic, TKey key) where TValue : new()
        {
            if (dic.ContainsKey(key)) return dic[key];
            
            var newObj = new TValue();
            dic[key] = newObj;
            return newObj;
        }
    }
}