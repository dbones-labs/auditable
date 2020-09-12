namespace Auditable.Tests
{
    using System;
    using Writers;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;

    public static class ApplicationContainer
    {
        public static IServiceCollection Setup(this IServiceCollection serviceCollection, 
            Action<IServiceCollection> setup = null,
            Action<IServiceCollection> configureAuditable = null)
        {

            configureAuditable?.Invoke(serviceCollection);
            serviceCollection.AddSingleton<IWriter, TestWriter>();
            serviceCollection.AddSingleton(ctx => (TestWriter)ctx.GetService<IWriter>());
            serviceCollection.AddSingleton<IAuditIdGenerator, TestIdGen>();

            setup?.Invoke(serviceCollection);
            return serviceCollection;
        }

        public static IServiceProvider Build(
            Action<IServiceCollection> setup = null,
            Action<IServiceCollection> configureAuditable = null)
        {
            IServiceCollection serviceCollection = new ServiceCollection();
            serviceCollection.AddLogging(configure => configure.AddConsole());
            serviceCollection.Setup(setup, configureAuditable);
            return serviceCollection.BuildServiceProvider();
        }
    }


    //public class TestIn : IInitiatorCollector
}
