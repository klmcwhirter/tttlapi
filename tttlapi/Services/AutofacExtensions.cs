using Autofac;
using tttlapi.Models;

namespace tttlapi.Services
{
    /// <summary>
    /// Extension methods to register types in this library
    /// </summary>
    public static class AutofacExtensions
    {
        /// <summary>
        /// Register Services types
        /// </summary>
        /// <param name="builder">ContainerBuilder</param>
        /// <returns>ContainerBuilder</returns>
        public static ContainerBuilder RegisterServices(this ContainerBuilder builder)
        {
            builder.RegisterType<JsonFromStringTransformer<Game>>().As<ITransformer<string, Game>>();
            builder.RegisterType<JsonToStringTransformer<Game>>().As<ITransformer<Game, string>>();

            return builder;
        }
    }
}