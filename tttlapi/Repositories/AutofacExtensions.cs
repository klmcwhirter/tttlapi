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
            else
            {
                builder.RegisterType<StackExchangeRedisGamesRepository>().As<IGamesRepository>();
            }

            return builder;
        }
    }
}