namespace Auditable.Configuration
{
    using System;
    using Collectors;
    using Collectors.EntityId;
    using Collectors.Environment;
    using Collectors.Initiator;
    using Collectors.Request;
    using Delta;
    using Infrastructure;
    using Parsing;
    using Writers;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Writers.Console;

    public static class SetupAuditable
    {
        /// <summary>
        /// Setup Auditable with default setup.
        /// Consider using the <seealso cref="ConfigureAuditable"/>, which provides better integration with the <see cref="IHostBuilder"/>
        /// </summary>
        /// <param name="services">the setup</param>
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

        /// <summary>
        /// setup Auditable with the defaults
        /// </summary>
        /// <param name="builder">this <see cref="IHostBuilder"/> of the application</param>
        /// <param name="conf">allows you to provides extensions and overrides</param>
        public static IHostBuilder ConfigureAuditable(this IHostBuilder builder, Action<AuditableExtension> conf = null)
        {
            var setup = new AuditableExtension();
            conf?.Invoke(setup);

            builder.ConfigureServices((ctx, services) =>
            {
                //setup the base
                services.AddAuditable();

                //setup mods
                setup.RegisterServices(services);
            });

            return builder;
        }
    }
}