#pragma warning disable CS1572, CS1573, CS1591
using Autofac;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace tttlapi.Utils
{
    /// <summary>
    /// Extension methods to register this library's types
    /// </summary>
    public static class MongodbAutofacExtensions
    {
        /// <summary>
        /// Register the Mongodb types
        /// </summary>
        /// <param name="builder">ContainerBuilder</param>
        /// <param name="configRoot">IConfigurationRoot</param>
        /// <returns>ContainerBuilder</returns>
        public static ContainerBuilder RegisterMongodb(this ContainerBuilder builder, IConfigurationRoot configRoot, ILogger logger)
        {
            var config = GetMongodbConfiguration(configRoot);
            logger.LogInformation(config);

            builder.Register<IMongoDatabase>(c =>
            {
                var dbName = configRoot.GetValue<string>("MONGODB_DATABASE");
                logger.LogInformation(dbName);

                var db = new MongoClient(config).GetDatabase(dbName);
                return db;
            }).As<IMongoDatabase>();

            return builder;
        }
        /// <summary>
        /// Gets the URL for the Mongodb service in this namespace
        /// </summary>
        /// <param name="configRoot">IConfigurationRoot</param>
        /// <returns>Mongodb connection string URL</returns>
        public static string GetMongodbConfiguration(IConfigurationRoot configRoot)
        {
            var user = configRoot.GetValue<string>("MONGODB_USER");
            var password64 = configRoot.GetValue<string>("MONGODB_PASSWORD");
            var password = password64; // Encoding.UTF8.GetString(Convert.FromBase64String(password64));
            var host = configRoot.GetValue<string>("MONGODB_SERVICE_HOST");
            var port = configRoot.GetValue<string>("MONGODB_SERVICE_PORT");
            var dbName = configRoot.GetValue<string>("MONGODB_DATABASE");
            var appName = "tttlapi";
            var rc = $"mongodb://{user}:{password}@{host}:{port}/{dbName}?appName={appName}";
            return rc;
        }
    }
}
