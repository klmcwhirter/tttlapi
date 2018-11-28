#pragma warning disable CS1572, CS1573, CS1591
using Autofac;
using Microsoft.Extensions.Configuration;
using ServiceStack.Redis;
using ServiceStack.Redis.Generic;

namespace tttlapi.Utils
{
    /// <summary>
    /// Extension methods to register this library's types
    /// </summary>
    public static class AutofacExtensions
    {
        /// <summary>
        /// Register the Redis types
        /// </summary>
        /// <param name="builder">ContainerBuilder</param>
        /// <param name="configRoot">IConfigurationRoot</param>
        /// <returns>ContainerBuilder</returns>
        public static ContainerBuilder RegisterRedis(this ContainerBuilder builder, IConfigurationRoot configRoot)
        {
            var redisUrl = GetRedisUrl(configRoot);

            builder.Register<IRedisClientsManager>(c => new PooledRedisClientManager(redisUrl)).As<IRedisClientsManager>();
            builder.Register<IRedisClient>(c =>
            {
                var mgr = c.Resolve<IRedisClientsManager>();
                var client = mgr.GetClient();
                return client;
            }).As<IRedisClient>();

            return builder;
        }
        /// <summary>
        /// Gets the URL for the Redis master in this namespace
        /// </summary>
        /// <param name="configRoot">IConfigurationRoot</param>
        /// <returns>Redis connection string URL</returns>
        public static string GetRedisUrl(IConfigurationRoot configRoot)
        {
            var password64 = configRoot.GetValue<string>("REDIS_PASSWORD");
            var password = password64; // Encoding.UTF8.GetString(Convert.FromBase64String(password64));
            var redisHost = configRoot.GetValue<string>("REDIS_SERVICE_HOST");
            var redisPort = configRoot.GetValue<string>("REDIS_SERVICE_PORT");
            var rc = $"redis://{password}@{redisHost}:{redisPort}";
            return rc;
        }
    }
}