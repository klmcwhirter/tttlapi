#pragma warning disable CS1572, CS1573, CS1591
using Autofac;
using Microsoft.Extensions.Configuration;
using StackExchange.Redis;

namespace tttlapi.Utils
{
    /// <summary>
    /// Extension methods to register this library's types
    /// </summary>
    public static class StackExchangeAutofacExtensions
    {
        /// <summary>
        /// Register the Redis types
        /// </summary>
        /// <param name="builder">ContainerBuilder</param>
        /// <param name="configRoot">IConfigurationRoot</param>
        /// <returns>ContainerBuilder</returns>
        public static ContainerBuilder RegisterStackExchenageRedis(this ContainerBuilder builder, IConfigurationRoot configRoot)
        {
            var redisConfig = GetRedisConfiguration(configRoot);

            builder.Register<IDatabase>(c =>
            {
                var client = ConnectionMultiplexer.Connect(redisConfig).GetDatabase();
                return client;
            }).As<IDatabase>();

            return builder;
        }
        /// <summary>
        /// Gets the URL for the Redis master in this namespace
        /// </summary>
        /// <param name="configRoot">IConfigurationRoot</param>
        /// <returns>Redis connection string URL</returns>
        public static ConfigurationOptions GetRedisConfiguration(IConfigurationRoot configRoot)
        {
            var password64 = configRoot.GetValue<string>("REDIS_PASSWORD");
            var password = password64; // Encoding.UTF8.GetString(Convert.FromBase64String(password64));
            var redisHost = configRoot.GetValue<string>("REDIS_SERVICE_HOST");
            var redisPort = configRoot.GetValue<string>("REDIS_SERVICE_PORT");
            var appName = "tttlpi";
            var optsString = $"{redisHost}:{redisPort},password={password},name={appName}";
            var rc = ConfigurationOptions.Parse(optsString);
            return rc;
        }
    }
}
