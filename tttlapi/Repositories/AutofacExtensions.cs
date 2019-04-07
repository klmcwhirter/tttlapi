using Autofac;
using Microsoft.Extensions.Configuration;
using tttlapi.Models;

namespace tttlapi.Repositories
{
    /// <summary>
    /// Extension methods to register types in this library
    /// </summary>
    public static class AutofacExtensions
    {
        /// <summary>
        /// Register Respositories types
        /// </summary>
        /// <param name="builder">ContainerBuilder</param>
        /// <param name="configRoot">IConfigurationRoot</param>
        /// <returns>ContainerBuilder</returns>
        public static ContainerBuilder RegisterRepositories(this ContainerBuilder builder, IConfigurationRoot configRoot)
        {
            if (configRoot.GetValue<string>("TTTLAPI_INMEMORY_REPO") != null)
            {
                builder.RegisterType<GamesRepository>().As<IGamesRepository>().SingleInstance();
            }
            else if (configRoot.ShouldUseRedis())
            {
                builder.RegisterType<StackExchangeRedisGamesRepository>().As<IGamesRepository>();
            }
            else
            {
                builder.RegisterType<MongodbGamesRepository>().As<IGamesRepository>();
            }

            return builder;
        }

        /// <summary>
        /// Should this configuration use Redis?
        /// </summary>
        /// <param name="configRoot"></param>
        /// <returns>bool</returns>
        public static bool ShouldUseRedis(this IConfigurationRoot configRoot) => configRoot.GetValue<string>("TTTLAPI_USE_REDIS") != null;
    }
}