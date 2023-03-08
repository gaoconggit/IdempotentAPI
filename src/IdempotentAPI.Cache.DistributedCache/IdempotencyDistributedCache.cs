//using System;
//using System.Threading;
//using IdempotentAPI.Cache.DistributedCache.Lockers;
//using Microsoft.Extensions.Caching.Distributed;

//namespace IdempotentAPI.Cache.DistributedCache
//{
//    public class IdempotencyDistributedCache : IIdempotencyCache
//    {
//        private readonly IDistributedCache _distributedCache;
//        private readonly string lockPrefix = "csredis_lock:";

//        public IdempotencyDistributedCache(IDistributedCache distributedCache)
//        {
//            _distributedCache = distributedCache;
//        }

//        /// <returns>An object of type <see cref="DistributedCacheEntryOptions"/>.</returns>
//        /// <inheritdoc/>
//        public object CreateCacheEntryOptions(int expireSeconds)
//        {
//            return new DistributedCacheEntryOptions()
//            {
//                AbsoluteExpirationRelativeToNow = new TimeSpan(0, 0, expireSeconds)
//            };
//        }


//        /// <inheritdoc/>
//        public byte[] GetOrDefault(
//            string key,
//            byte[] defaultValue,
//            object? options = null,
//            CancellationToken token = default)
//        {
//            if (key is null)
//            {
//                throw new ArgumentNullException(nameof(key));
//            }

//            if (options is not null && options is not DistributedCacheEntryOptions)
//            {
//                throw new ArgumentNullException(nameof(options));
//            }

//            using (var redisLock = RedisHelper.Lock(lockPrefix + key, 300))
//            {
//                if (redisLock == null)
//                {
//                    return defaultValue;
//                }

//                byte[] cachedData = _distributedCache.Get(key);
//                return cachedData is null ? defaultValue : cachedData;
//            }
//        }

//        /// <inheritdoc/>
//        public long GetOrSet(
//            string key,
//           long defaultValue,
//            object? options = null,
//            CancellationToken token = default)
//        {
//            if (key is null)
//            {
//                throw new ArgumentNullException(nameof(key));
//            }

//            if (options is not null && options is not DistributedCacheEntryOptions)
//            {
//                throw new ArgumentNullException(nameof(options));
//            }

//            using (var redisLock = RedisHelper.Lock(lockPrefix + key, 300))
//            {
//                if (redisLock == null)
//                {
//                    return defaultValue;
//                }

//                return RedisHelper.Get<long>(key);
//            }

//        }

//        public void IncrBy(string key, long value = 1, int expireSeconds = 1)
//        {
//            if (key is null)
//            {
//                throw new ArgumentNullException(nameof(key));
//            }

//            RedisHelper.IncrBy(key + "_count",value);
//        }

//        /// <inheritdoc/>
//        /// <exception cref="ArgumentNullException"></exception>
//        public void Set(
//            string key,
//            byte[] value,
//            object? options = null,
//            CancellationToken token = default)
//        {
//            if (key is null)
//            {
//                throw new ArgumentNullException(nameof(key));
//            }

//            if (options is not null && options is not DistributedCacheEntryOptions)
//            {
//                throw new ArgumentNullException(nameof(options));
//            }

//            using (var redisLock = RedisHelper.Lock(lockPrefix + key, 300))
//            {
//                if (redisLock == null)
//                {
//                    return;
//                }

//                _distributedCache.Set(key, value, (DistributedCacheEntryOptions?)options);
//            }
//        }

//        public void SetKeyExpire(string key, int expireSeconds = 1)
//        {
//            RedisHelper.Expire(key + "_count", expireSeconds);
//        }
//    }
//}
