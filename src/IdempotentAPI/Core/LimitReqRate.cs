#nullable enable
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;

namespace RMOHR.LimitReqRate.Filters
{
    public class LimitReqRate
    {
        private readonly string _distributedCacheKeysPrefix;
        private readonly int _expireSeconds;
        private readonly int _times;
        private readonly int _redisKeyTimeOutSeconds;

        private string DistributedCacheKey
        {
            set
            {
                _idempotencyKey = value;
            }
            get
            {
                return _distributedCacheKeysPrefix + _idempotencyKey;
            }
        }


        private string _idempotencyKey = string.Empty;


        public LimitReqRate(
            ILogger<LimitReqRate> logger,
            int expireSeconds,
            string distributedCacheKeysPrefix,
            int times,
            int redisKeyTimeOutSeconds)
        {
            _expireSeconds = expireSeconds;
            _times = times;
            _distributedCacheKeysPrefix = distributedCacheKeysPrefix;
            _redisKeyTimeOutSeconds = redisKeyTimeOutSeconds;
        }

        private bool TryGetIdempotencyKey(HttpRequest httpRequest, out string idempotencyKey)
        {
            idempotencyKey = httpRequest.Path.ToString();
            return true;
        }

        /// <summary>
        /// Return the cached response based on the provided idempotencyKey
        /// </summary>
        /// <param name="context"></param>
        public void ApplyPreHandle(ActionExecutingContext context)
        {
            // Try to get the IdempotencyKey value from header:
            if (!TryGetIdempotencyKey(context.HttpContext.Request, out _idempotencyKey))
            {
                context.Result = null;
                return;
            }

            using (var redisLock = RedisHelper.Lock(DistributedCacheKey+"_csredis_lock", _redisKeyTimeOutSeconds))
            {

                if (redisLock == null)
                {
                    context.Result = null;
                    return;
                }

                string key = DistributedCacheKey + "_count";

                long keyCount = RedisHelper.IncrBy(key);
                RedisHelper.Expire(key, _expireSeconds);

                Console.WriteLine($"{key}: {keyCount} ttl: {RedisHelper.Ttl(key)}");


                if (keyCount > _times)
                {
                    context.Result = new BadRequestObjectResult($"请求太频繁");
                    return;
                }

            }
        }
    }
}
