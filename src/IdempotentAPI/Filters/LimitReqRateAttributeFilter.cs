using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Reflection;

namespace RMOHR.LimitReqRate.Filters
{
    public class LimitReqRateAttributeFilter : IActionFilter, IResultFilter
    {
        public bool Enabled { get; private set; }
        public int ExpireHours { get; private set; }
        public string DistributedCacheKeysPrefix { get; private set; }
        public int DistributedCacheTimeOutSeconds { get; private set; }

        public string ModelType { get; private set; }
        public string ModelArgumentName { get; private set; }
        public string ModelArgumentPropertyName { get; private set; }

        public int Times { get; private set; }


        private LimitReqRate? _idempotency = null;

        private readonly ILogger<LimitReqRate> _logger;

        public LimitReqRateAttributeFilter(
            ILoggerFactory loggerFactory,
            bool Enabled,
            int ExpireHours,
            string DistributedCacheKeysPrefix,
            int times,
            string modelType,
            string modelArgumentName,
            string modelArgumentPropertyName,
            int distributedCacheTimeOutSeconds)
        {
            this.Enabled = Enabled;
            this.ExpireHours = ExpireHours;
            this.DistributedCacheKeysPrefix = DistributedCacheKeysPrefix;
            this.Times = times;

            if (loggerFactory != null)
            {
                _logger = loggerFactory.CreateLogger<LimitReqRate>();
            }
            else
            {
                _logger = NullLogger<LimitReqRate>.Instance;
            }
            ModelType = modelType;
            ModelArgumentName = modelArgumentName;
            ModelArgumentPropertyName = modelArgumentPropertyName;
            DistributedCacheTimeOutSeconds = distributedCacheTimeOutSeconds;
        }

        /// <summary>
        /// Runs before the execution of the controller
        /// </summary>
        /// <param name="context"></param>
        public void OnActionExecuting(ActionExecutingContext context)
        {
            string typeName = ModelType; // 要转换的类型字符串
            Type type = Type.GetType(typeName);

            var model = context.ActionArguments[ModelArgumentName]; // 获取模型绑定的数据

            if (model.GetType() == type)
            {
                //((WebApi_3_1.DTOs.SimpleRequest)model).Message
                PropertyInfo propertyInfo = type.GetProperty(ModelArgumentPropertyName); // 获取 MyProperty 属性的 PropertyInfo 对象
                string value = (string)propertyInfo.GetValue(model); // 获取 MyProperty 的值
                DistributedCacheKeysPrefix = value;
            }

            //if(model is SimpleRequest)
            // If the Idempotency is disabled then stop
            if (!Enabled)
            {
                return;
            }

            // Initialize only on its null (in case of multiple executions):
            if (_idempotency == null)
            {
                _idempotency = new LimitReqRate(_logger, ExpireHours, DistributedCacheKeysPrefix, Times, DistributedCacheTimeOutSeconds);
            }

            _idempotency.ApplyPreHandle(context);
        }

        // NOT USED
        public void OnActionExecuted(ActionExecutedContext context)
        {

        }

        // NOT USED
        public void OnResultExecuting(ResultExecutingContext context)
        {

        }

        /// <summary>
        /// Runs after the results have been calculated
        /// </summary>
        /// <param name="context"></param>
        public void OnResultExecuted(ResultExecutedContext context)
        {

        }
    }
}
