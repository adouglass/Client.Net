using System;
using System.Runtime.Caching;

namespace Subscrio.Client
{
    public enum MyCachePriority
    {
        Default,
        NotRemovable
    }
    public class Cache
        {
            // Gets a reference to the default MemoryCache instance. 
            private static readonly ObjectCache cache = MemoryCache.Default;
            private readonly int StandardExpirationSeconds = 300;

            private CacheEntryRemovedCallback callback;
            private CacheItemPolicy policy;

            public void Add(String CacheKeyName, Object CacheItem, MyCachePriority MyCacheItemPriority)
            {
                // 
                callback = RemovedCallback;
                policy = new CacheItemPolicy();
                policy.Priority = (MyCacheItemPriority == MyCachePriority.Default)
                    ? CacheItemPriority.Default
                    : CacheItemPriority.NotRemovable;
                policy.AbsoluteExpiration = DateTimeOffset.Now.AddSeconds(StandardExpirationSeconds);
                policy.RemovedCallback = callback;

                // Add inside cache 
                cache.Set(CacheKeyName, CacheItem, policy);
            }

            public void Add(String CacheKeyName, Object CacheItem, MyCachePriority MyCacheItemPriority, int Seconds)
            {
                // 
                callback = RemovedCallback;
                policy = new CacheItemPolicy();
                policy.Priority = (MyCacheItemPriority == MyCachePriority.Default)
                    ? CacheItemPriority.Default
                    : CacheItemPriority.NotRemovable;
                policy.AbsoluteExpiration = DateTimeOffset.Now.AddSeconds(Seconds);
                policy.RemovedCallback = callback;

                // Add inside cache 
                cache.Set(CacheKeyName, CacheItem, policy);
            }

            public Object Get(String CacheKeyName)
            {
                return cache[CacheKeyName];
            }

            public void Remove(String CacheKeyName)
            {
                if (cache.Contains(CacheKeyName))
                {
                    cache.Remove(CacheKeyName);
                }
            }

            private void RemovedCallback(CacheEntryRemovedArguments arguments)
            {
                // Log these values from arguments list 
                String strLog = String.Concat("Reason: ", arguments.RemovedReason.ToString(), " | Key-Name: ",
                    arguments.CacheItem.Key, " | Value-Object: ",
                    arguments.CacheItem.Value.ToString());
            }
        }

}
