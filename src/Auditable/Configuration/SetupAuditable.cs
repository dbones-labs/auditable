namespace Auditable.Configuration
{
    using System;
    using Collectors;
    using Parsing;
    using Writers;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;

    public static class SetupAuditable
    {
        public static IServiceCollection AddAuditable(this IServiceCollection services)
        {
            //core
            services.AddTransient<IAuditable, Auditable>();
            services.AddScoped<IInternalAuditableContext, AuditableContext>();
            services.AddScoped<Func<IInternalAuditableContext>>(svc => svc.GetService<IInternalAuditableContext>);
            services.AddSingleton<IDifferenceEngine, DifferenceEngine>();
            services.AddSingleton<JsonSerializer>();
            services.AddSingleton<IAuditIdGenerator, AuditIdGenerator>();

            //collectors
            services.AddSingleton<IEnvironmentCollector, DefaultEnvironmentCollector>();
            services.AddScoped<IInitiatorCollector, PrincipalInitiatorCollector>();
            services.AddScoped<IRequestContextCollector, NullRequestContextCollector>();
            services.AddSingleton<IEntityIdCollector, EntityIdCollector>();

            //parse and write
            services.AddScoped<IParser, DefaultParser>();
            services.AddSingleton<IWriter, ConsoleWriter>();


            return services;
        }

        //public static IHostBuilder ConfigureAllAboard(this IHostBuilder builder, Action<HostSetup> conf)
        //{
        //    var setup = new HostSetup();
        //    conf(setup);

        //    builder.ConfigureServices((ctx, services) =>
        //    {
        //        var baseFactory = new Factory();
        //        baseFactory.RegisterServices(services);

        //        setup.DataStoreProvider.RegisterServices(services);
        //        setup.MessagingProvider.RegisterServices(services);
        //    });

        //    return builder;
        //}
    }


}