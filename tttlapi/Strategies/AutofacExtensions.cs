using Autofac;

namespace tttlapi.Strategies
{
    /// <summary>
    /// Extension methods to register types in this library
    /// </summary>
    public static class AutofacExtensions
    {
        /// <summary>
        /// Register Strategies types
        /// </summary>
        /// <param name="builder">ContainerBuilder</param>
        /// <returns>ContainerBuilder</returns>
        public static ContainerBuilder RegisterStrategies(this ContainerBuilder builder)
        {
            // Register PlayerKind strategies - registered in order of Playerkind enum
            builder.RegisterType<HumanPlayerStrategy>().As<IPlayerStrategy>();
            builder.RegisterType<RulesPlayerStrategy>().As<IPlayerStrategy>();
            builder.RegisterType<LearningPlayerStrategy>().As<IPlayerStrategy>();
            builder.RegisterType<MinimaxPlayerStrategy>().As<IPlayerStrategy>();
            builder.RegisterType<RandomPlayerStrategy>().As<IPlayerStrategy>();

            return builder;
        }
    }
}