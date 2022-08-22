﻿using System;
using IdempotentAPI.Cache;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace IdempotentAPI.Filters
{
    /// <summary>
    /// Use Idempotent operations on POST and PATCH HTTP methods
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class IdempotentAttribute : Attribute, IFilterFactory
    {
        public bool IsReusable => false;

        public bool Enabled { get; set; } = true;

        public int ExpireSeconds { get; set; } = 3600;

        public string DistributedCacheKeysPrefix { get; set; } = "IdempAPI_";

        public string HeaderKeyName { get; set; } = "IdempotencyKey";

        public IFilterMetadata CreateInstance(IServiceProvider serviceProvider)
        {
            var distributedCache = (IIdempotencyCache)serviceProvider.GetService(typeof(IIdempotencyCache));
            var loggerFactory = (ILoggerFactory)serviceProvider.GetService(typeof(ILoggerFactory));

            IdempotencyAttributeFilter idempotencyAttributeFilter = new IdempotencyAttributeFilter(distributedCache, loggerFactory, Enabled, ExpireSeconds, HeaderKeyName, DistributedCacheKeysPrefix);
            return idempotencyAttributeFilter;
        }
    }
}
