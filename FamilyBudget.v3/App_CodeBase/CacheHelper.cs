using System;
using System.Web;
using System.Web.Caching;

namespace FamilyBudget.v3.App_CodeBase
{
    public static class CacheHelper
    {
        /// <summary>
        ///     Insert value into the cache using
        ///     appropriate name/value pairs
        /// </summary>
        /// <typeparam name="T">Type of cached item</typeparam>
        /// <param name="o">Item to be cached</param>
        /// <param name="key">Name of item</param>
        public static void Add<T>(T o, string key) where T : class
        {
            // NOTE: Apply expiration parameters as you see fit.
            // In this example, I want an absolute 
            // timeout so changes will always be reflected 
            // at that time. Hence, the NoSlidingExpiration.  
            HttpContext.Current.Cache.Insert(key, o, null, DateTime.Now.AddMinutes(20), Cache.NoSlidingExpiration);
        }

        /// <summary>
        ///     Remove item from cache
        /// </summary>
        /// <param name="key">Name of cached item</param>
        public static void Clear(string key)
        {
            HttpContext.Current.Cache.Remove(key);
        }

        /// <summary>
        ///     Check for item in cache
        /// </summary>
        /// <param name="key">Name of cached item</param>
        /// <returns></returns>
        public static bool Exists(string key)
        {
            return HttpContext.Current.Cache[key] != null;
        }

        /// <summary>
        ///     Retrieve cached item
        /// </summary>
        /// <typeparam name="T">Type of cached item</typeparam>
        /// <param name="key">Name of cached item</param>
        /// <returns>Cached item as type</returns>
        public static T Get<T>(string key) where T : class
        {
            try
            {
                return (T) HttpContext.Current.Cache[key];
            }
            catch
            {
                return null;
            }
        }
    }
}