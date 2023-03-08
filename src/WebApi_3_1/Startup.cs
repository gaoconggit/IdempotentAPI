using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdempotentAPI.Cache.DistributedCache.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace WebApi_3_1
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Register an implementation of IDistributedCache such as Memory Cache, SQL Server cache, Redis cache, etc.).
            // For this example, we are using a Memory Cache.
            //services.AddDistributedMemoryCache();

            // Register an implementation of the IDistributedCache.
            // For this example, we are using a Memory Cache.
            //services.AddDistributedMemoryCache();

            AddCSRedisCoreCache(services);

            // Register the IdempotentAPI.Cache.DistributedCache.
            //services.AddIdempotentAPIUsingDistributedCache();

            services.AddControllers();
        }

        private static void AddCSRedisCoreCache(IServiceCollection services)
        {
            var csredis = new CSRedis.CSRedisClient("10.136.0.206:6379,password=,defaultDatabase=10,poolsize=50,ssl=false,writeBuffer=10240,prefix=idempotent:");
            RedisHelper.Initialization(csredis);
            services.AddSingleton<IDistributedCache>(new Microsoft.Extensions.Caching.Redis.CSRedisCache(RedisHelper.Instance));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
