using System;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace RMOHR.LimitReqRate.Filters
{
    /// <summary>
    /// Use Idempotent operations on POST and PATCH HTTP methods
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class LimitReqRateAttribute : Attribute, IFilterFactory
    {
        public bool IsReusable => false;

        public bool Enabled { get; set; } = true;

        public int ExpireSeconds { get; set; } = 60;

        public int Times { get; set; } = 50;

        public string DistributedCacheKeysPrefix { get; set; } = "IdempAPI_";
        public int RedisLockTimeOutSeconds { get; set; } = 120;

        public string ModelType { get; set; }
        public string ModelArgumentName { get; set; }
        public string ModelArgumentPropertyName { get; set; }

        public IFilterMetadata CreateInstance(IServiceProvider serviceProvider)
        {
            var loggerFactory = (ILoggerFactory)serviceProvider.GetService(typeof(ILoggerFactory));

            LimitReqRateAttributeFilter limitReqRateAttributeFilter = new LimitReqRateAttributeFilter(
                loggerFactory,
                Enabled,
                ExpireSeconds,
                DistributedCacheKeysPrefix,
                Times,
                ModelType,
                ModelArgumentName,
                ModelArgumentPropertyName,
                RedisLockTimeOutSeconds);

            return limitReqRateAttributeFilter;
        }
    }
}
