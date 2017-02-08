using System;
using System.Collections.Generic;
using System.Linq;

namespace Service
{
    public static class ApplicationCache<T>
    {
        private static List<T> _items;

        static ApplicationCache()
        {
            _items = new List<T>();
        }

        public static void FillCache(List<T> items)
        {
            _items = items;
        }

        public static List<T> GetCache()
        {
            return _items;
        }

        public static T GetCacheItem(Func<T, bool> predicate)
        {
            return _items.FirstOrDefault<T>(predicate);
        }

        public static List<T> GetCacheItems(Func<T, bool> predicate)
        {
            return _items.Where<T>(predicate).ToList();
        }

        public static void AddCacheItem(T item)
        {
            _items.Add(item);
        }

        public static void RemoveCacheItem(T item)
        {
            _items.Remove(item);
        }
    }
}
